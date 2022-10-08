
using microservice.dto.webdata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Aspose.NLP.Core.Grammar;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace microservice.gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class GatewayController : ControllerBase
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly ServicesOptions _serviceOptions;
        private const char endUrlTrimChar = '/';

        public GatewayController(ILogger<GatewayController> logger, IOptionsMonitor<ServicesOptions> serviceOptions)
        {
            _logger = logger;
            _serviceOptions = serviceOptions.CurrentValue;
        }

        [HttpGet("{corellationId}")]
        public async Task<IEnumerable<WordItemSummary>> GetAsync([Required] string corellationId, [FromQuery] bool excludedGrammar = false)
        {
            _logger.LogInformation($"{nameof(GetAsync)} corellationId: {corellationId}");

            var helper = new ExcludeGrammarHelper();

            using var client = new HttpClient();

            var url = _serviceOptions.WordSummaryUrl.TrimEnd(endUrlTrimChar);

            var response = await client.GetFromJsonAsync<IEnumerable<WordItemSummary>>($"{url}/{corellationId}");
            var emptyList = new List<WordItemSummary>();

            return excludedGrammar ? await helper.ExcludeGrammarWords(response ?? emptyList) : response ?? emptyList;
        }

        [HttpPost]
        public async Task<Guid> ProcessingUrlAsync([FromBody]string url)
        {
            _logger.LogInformation($"{nameof(ProcessingUrlAsync)} url: {url}");

            using var client = new HttpClient();
            var serviceUrl = _serviceOptions.WebDownloaderUrl.TrimEnd(endUrlTrimChar);

            var response = await client.PostAsJsonAsync(serviceUrl, Base64UrlEncoder.Encode(url));
            response.EnsureSuccessStatusCode();
            var corellationId = await response.Content.ReadAsAsync<Guid>();

            return corellationId;
        }
    }
}