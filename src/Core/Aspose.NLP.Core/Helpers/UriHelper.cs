using System.Text.RegularExpressions;

namespace Aspose.NLP.Core.Helpers;

public class UriHelper
{
    public static bool IsUrlValid(string url)
    {
        var validateDateRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        var isValid = validateDateRegex.IsMatch(url);

        Uri uriResult;
        bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        return result && isValid;
    }
}
