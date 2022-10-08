using Aspose.NLP.Core.Html;

namespace Aspose.NLP.Core.Tests
{
    public class HtmlToTextConvertorUnitTests
    {
        [Fact]
        public void ConvertHtmlToTextSimpleTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<div>html text</div>");
            Assert.Equal("html text", text);
        }

        [Fact]
        public void ConvertHtmlToTextSecondTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<html><div>html text</div></html>");
            Assert.Equal("html text", text);
        }

        [Fact]
        public void ConvertHtmlToTextThirdTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<html><body><div>html text</div></body></html>");
            Assert.Equal("html text", text);
        }

        [Fact]
        public void ConvertHtmlToTextFourTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<html><body><div>html text</div></html>");
            Assert.Equal("html text", text);
        }

        [Fact]
        public void ConvertHtmlToTextFiveTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<html><body><div>html text</div><body></html>");
            Assert.Equal("html text", text);
        }

        [Fact]
        public void ConvertHtmlToTextSixTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<html><body><div>html text</div></ body ></html>");
            Assert.Equal("html text", text);
        }

        [Fact]
        public void ConvertHtmlToTextSevenTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<html><body ><div>html text</div></ body ></html>");
            Assert.Equal("html text", text);
        }

        [Fact]
        public void ConvertHtmlToTextLastTest()
        {
            var helper = new HtmlToTextConvertor();
            var text = helper.ConvertHtmlToText("<html><body ><div>html <-- ddd --> text</div></ body ></html>");
            Assert.Equal("html text", text);
        }
    }
}