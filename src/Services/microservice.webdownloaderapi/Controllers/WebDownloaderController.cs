using Dapr.Client;
using microservice.common;
using Microsoft.AspNetCore.Mvc;
using microservice.dto.webdata;
using microservice.weburlapi.State;
using Microsoft.IdentityModel.Tokens;

namespace microservice.weburlapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class WebDownloaderController : ControllerBase
    {
        private ILogger<WebDownloaderController> _logger;

        public WebDownloaderController(ILogger<WebDownloaderController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Method for download html by url.
        /// </summary>
        /// <param name="daprClient">State client to interact with Dapr runtime.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> UrlAsync([FromBody] string url, [FromServices] DaprClient daprClient)
        {
            var correctUrl = Base64UrlEncoder.Decode(url);

            _logger.LogInformation($"Processing url data - {correctUrl}");

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(correctUrl);

            _logger.LogInformation($"Response html data - {response.Length}");

            var correlationId = Guid.NewGuid();

            var state = await daprClient.GetStateEntryAsync<WebUrlState>(StoreNames.WebDownloaderStoreName, correlationId.ToString());
            state.Value ??= new WebUrlState() { CreatedOn = DateTime.UtcNow, CorrelationId = correlationId, Url = url };

            await state.SaveAsync();

            state.Value ??= new WebUrlState() { CreatedOn = DateTime.UtcNow, CorrelationId = correlationId, Url = url };
            var result = new WebUriData(DateTime.UtcNow, correlationId, url, response, 5);

            await daprClient.PublishEventAsync(PubSubs.PubSub, Topics.WebUrlDataTopicName, result).ConfigureAwait(false);

            return Ok(result.CorrelationId);
        }
    }
}