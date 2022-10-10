using Aspose.NLP.Core.Helpers;

namespace Aspose.NLP.Core.Tests
{
    public class UriHelperTests
    {
        [Fact]
        public void UrlRegexpTest()
        {
            var valid = UriHelper.IsUrlValid("http://www.google.com");
            Assert.True(valid);

            valid = UriHelper.IsUrlValid("https://www.google.com");
            Assert.True(valid);

            valid = UriHelper.IsUrlValid("htps://www.google.com");
            Assert.True(!valid);

            valid = UriHelper.IsUrlValid("https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/records");
            Assert.True(valid);

            valid = UriHelper.IsUrlValid("https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/anonymous-records");
            Assert.True(valid);

            valid = UriHelper.IsUrlValid("https://learn.microsoft.com/en-us/d$!otnet@/fsharp/language-reference/anonymous-records");
            Assert.True(!valid);

            valid = UriHelper.IsUrlValid("htt://www.google.com");
            Assert.False(valid);

            valid = UriHelper.IsUrlValid("://www.google.com");
            Assert.False(valid);

            valid = UriHelper.IsUrlValid("www.google.com");
            Assert.False(valid);
        }
    }
}
