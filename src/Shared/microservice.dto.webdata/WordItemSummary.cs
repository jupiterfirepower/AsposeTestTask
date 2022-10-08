using Aspose.NLP.Core.Contracts;

namespace microservice.dto.webdata
{
    public record WordItemSummary(string Word, string Summary, int NWords) : IWordItemSummary;
}
