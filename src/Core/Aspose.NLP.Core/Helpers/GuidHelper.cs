namespace Aspose.NLP.Core.Helpers;

public class GuidHelper
{
    public static bool IsGuid(string value)
    {
        return Guid.TryParse(value, out _);
    }
}
