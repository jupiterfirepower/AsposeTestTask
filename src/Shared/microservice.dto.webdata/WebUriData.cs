namespace microservice.dto.webdata
{
    public record WebUriData(DateTime CreatedOn, Guid CorrelationId, string Url, string Data, int NWord, bool excludedGrammar = false);
}