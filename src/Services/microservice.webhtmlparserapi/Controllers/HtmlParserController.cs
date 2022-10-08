using Aspose.NLP.Core.Html;
using Dapr;
using Dapr.Client;
using microservice.common;
using microservice.dto.webdata;
using Microsoft.AspNetCore.Mvc;
using WebHtmlParserDataApi.State;

namespace WebHtmlParserDataApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HtmlParserController : ControllerBase
    {
        private ILogger<HtmlParserController> _logger;

        public HtmlParserController(ILogger<HtmlParserController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Method for shipping order.
        /// </summary>
        /// <param name="orderShipment">Shipment info.</param>
        /// <param name="daprClient">State client to interact with Dapr runtime.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Topic(PubSubs.PubSub, Topics.WebUrlDataTopicName)]
        [HttpPost]
        public async Task<ActionResult<Guid>> processing(WebUriData webData, [FromServices] DaprClient daprClient)
        {
            _logger.LogInformation($"Web Html Parser Processing RequestId - {webData.CorrelationId}, Url - { webData.Url}");

            var state = await daprClient.GetStateEntryAsync<WebDataState>(StoreNames.HtmlParserStoreName, webData.CorrelationId.ToString());
            state.Value ??= new WebDataState() { CreatedOn = DateTime.UtcNow, CorellationId = webData.CorrelationId, Html = webData.Data };

            await state.SaveAsync();

            // return corellation Id
            var result = state.Value.CorellationId;

            _logger.LogInformation($"Web Html Parser WebDataState saved {state.Value.CreatedOn} completed with CorellationId {state.Value.CorellationId}");

            var helper = new HtmlToTextConvertor();

            var text = helper.ConvertHtmlToText(webData.Data);

            var data = new WebUriData(DateTime.UtcNow, webData.CorrelationId, webData.Url, text, webData.NWord);

            await daprClient.PublishEventAsync(PubSubs.PubSub, Topics.WebTextDataTopicName, data);

            return Ok(data.CorrelationId);
        }
    }
}