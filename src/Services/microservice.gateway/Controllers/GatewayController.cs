
using microservice.dto.webdata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Aspose.NLP.Core.Grammar;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Aspose.NLP.Core.Helpers;
using System.Net.Mime;
using microservice.common;
using Newtonsoft.Json;

namespace microservice.gateway.Controllers;

[ApiController]
[Route("[controller]")]
//[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class GatewayController : ControllerBase
{
    private readonly ILogger<GatewayController> _logger;
    private readonly ServicesOptions _serviceOptions;
    private readonly INotificationHub _notifService;

    private const char endUrlTrimChar = '/';

    public GatewayController(ILogger<GatewayController> logger, IOptionsMonitor<ServicesOptions> serviceOptions, INotificationHub notificationHub)
    {
        _logger = logger;
        _serviceOptions = serviceOptions.CurrentValue;
        _notifService = notificationHub;
    }

    [HttpGet("{corellationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WordItemSummary>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<WordItemSummary>>> GetAsync([Required] string corellationId, [FromQuery] bool excludedGrammar = false)
    {
        _logger.LogInformation($"{nameof(GetAsync)} corellationId: {corellationId}");

        var isValid = GuidHelper.IsGuid(corellationId);

        if (!isValid)
            return BadRequest();

        var helper = new ExcludeGrammarHelper();

        using var client = new HttpClient();

        var url = _serviceOptions.WordSummaryUrl.TrimEnd(endUrlTrimChar);
        var emptyList = new List<WordItemSummary>();

        try
        {
            var response = await client.GetFromJsonAsync<IEnumerable<WordItemSummary>>($"{url}/{corellationId}");

            if(response != null && response.Count() == 0)
                return NotFound();

            return Ok(excludedGrammar ? await helper.ExcludeGrammarWords(response) : response);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost("sendnotif")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<ActionResult<string>> SendNotification(Notification notif)
    {
        _logger.LogInformation($"{nameof(SendNotification)} CorellationId - {notif.CorellationId}");

        CancellationToken token = new CancellationTokenSource().Token;

        await _notifService.PushDataAsync(notif, token);

        return Ok(notif.CorellationId);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> ProcessingUrlAsync([FromBody] [Required] string url)
    {
        _logger.LogInformation($"{nameof(ProcessingUrlAsync)} url: {url}");

        var isValid = UriHelper.IsUrlValid(url);

        if (!isValid)
            return BadRequest();

        using var client = new HttpClient();
        var serviceUrl = _serviceOptions.WebDownloaderUrl.TrimEnd(endUrlTrimChar);

        var response = await client.PostAsJsonAsync(serviceUrl, Base64UrlEncoder.Encode(url));
        response.EnsureSuccessStatusCode();
        var corellationId = await response.Content.ReadAsAsync<Guid>();


        _logger.LogInformation($" url : {url} , corellationId - {corellationId}");

        return Ok(corellationId);
    }
}