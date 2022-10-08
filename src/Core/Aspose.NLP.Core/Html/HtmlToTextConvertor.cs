namespace Aspose.NLP.Core.Html;

using System.Text.RegularExpressions;
using System.Text;

public class HtmlToTextConvertor
{
    private const string OneSpace = " ";

    private string stripHtml(string htmlText)
    {
        var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
        return reg.Replace(htmlText, string.Empty);
    }

    /// <summary>
    /// Remove new lines since they are not visible in HTML
    /// Remove tab spaces
    /// </summary>
    /// <param name="htmlText"></param>
    /// <returns></returns>
    private string convertTabs(string html) => html.Replace("\n", OneSpace).Replace("\t", OneSpace);

    /// <summary>
    /// Remove multiple white spaces from HTML
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    private string convertMultipleWhiteSpaces(string html) => Regex.Replace(html, "\\s+", OneSpace);

    /// <summary>
    /// Remove HEAD tag
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    private string removeHeadTag(string html) => Regex.Replace(html, "<head.*?</head>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

    /// <summary>
    /// Remove any JavaScript tag
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    private string removeAnyJavaScriptTags (string html) => Regex.Replace(html, "<script.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

    /// <summary>
    /// Remove Code tags
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    private string removeCodeTags(string html) => Regex.Replace(html, "<code.*?</code>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

    /// <summary>
    /// Replace special characters like &, <, >, " etc.
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    private string replaceSpecialCharacters(string html)
    {
        // Replace special characters like &, <, >, " etc.
        var sbHtml = new StringBuilder(html);
        // Note: There are many more special characters, these are just
        // most common. You can add new characters in this arrays if needed
        string[] OldWords = { "&nbsp;", "&amp;", "&quot;", "&lt;", "&gt;", "&reg;", "&copy;", "&bull;", "&trade;", "&#39;" };
        string[] NewWords = { " ", "&", "\"", "<", ">", "®", "©", "•", "™", "\'" };
        for (int i = 0; i < OldWords.Length; i++)
        {
            sbHtml.Replace(OldWords[i], NewWords[i]);
        }
        return sbHtml.ToString();
    }

    private string convertLineBreaks(string html)
    {
        var sbHtml = new StringBuilder(html);
        // Check if there are line breaks (<br>) or paragraph (<p>)
        sbHtml.Replace("<br>", "\n<br>");
        sbHtml.Replace("<br ", "\n<br ");
        sbHtml.Replace("<p ", "\n<p ");

        return sbHtml.ToString();
    }

    public string ConvertHtmlToText(string html)
    {
        // Remove new lines since they are not visible in HTML
        // Remove tab spaces

        var htmlCode = convertTabs(html);

        // Remove multiple white spaces from HTML
        htmlCode = convertMultipleWhiteSpaces(htmlCode);

        // Remove HEAD tag
        htmlCode = removeHeadTag(htmlCode);

        // Remove any JavaScript tag
        htmlCode = removeAnyJavaScriptTags(htmlCode);

        // Remove code tags from html
        htmlCode = removeCodeTags(htmlCode);

        // Replace special characters like &, <, >, " etc.
        htmlCode = replaceSpecialCharacters(htmlCode);

        // Check if there are line breaks(< br >) or paragraph(< p >)
        htmlCode = convertLineBreaks(htmlCode);

        // strip html tags
        htmlCode = stripHtml(htmlCode);

        // Remove multiple white spaces from HTML
        htmlCode = convertMultipleWhiteSpaces(htmlCode);

        // return plain text
        return htmlCode;
    }
}
