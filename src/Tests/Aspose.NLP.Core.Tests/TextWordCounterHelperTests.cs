using Aspose.NLP.Core.Text;

namespace Aspose.NLP.Core.Tests
{
    public class TextWordCounterHelperTests
    {
        [Fact]
        public void TextWordCounterHelperSimpleFirstTest()
        {
            var helper = new TextWordCounterHelper();
            var sentenses = new string[] { "one two three", "one two" };
            var words = new string[] { "one", "two", "three" };

            var dict = helper.FastWordCounterParallel(sentenses, words);
            Assert.True(dict.ContainsKey("one"));
            Assert.True(dict.ContainsKey("two"));
            Assert.True(dict.ContainsKey("three"));
            Assert.True(dict.Keys.Count == 3);
            Assert.True(dict["one"]==2);
            Assert.True(dict["two"] == 2);
            Assert.True(dict["three"] == 1);
        }

        [Fact]
        public void TextWordCounterHelperSimpleSecondTest()
        {
            var helper = new TextWordCounterHelper();
            var sentenses = new string[] { "one two three", "one two" };
            var words = new string[] { "one two", "two three" };

            var dict = helper.FastWordCounterParallel(sentenses, words);
            Assert.True(dict.ContainsKey("one two"));
            Assert.True(dict.ContainsKey("two three"));
            Assert.True(dict.Keys.Count == 2);
            Assert.True(dict["one two"] == 2);
            Assert.True(dict["two three"] == 1);
        }

        [Fact]
        public void TextWordCounterHelperSimpleThirdTest()
        {
            var helper = new TextWordCounterHelper();
            var sentenses = new string[] { "one two three", "one two", "one", "two three" };
            var words = new string[] { "one two", "two three" };

            var dict = helper.FastWordCounterParallel(sentenses, words);
            Assert.True(dict.ContainsKey("one two"));
            Assert.True(dict.ContainsKey("two three"));
            Assert.True(dict.Keys.Count == 2);
            Assert.True(dict["one two"] == 2);
            Assert.True(dict["two three"] == 2);
        }

        [Fact]
        public void TextWordCounterHelperSimpleFourTest()
        {
            var helper = new TextWordCounterHelper();
            var sentenses = new string[] { "one two three", "one two", "one", "two three" };
            var words = new string[] { "one two", "two three", "1" };

            var dict = helper.FastWordCounterParallel(sentenses, words);
            Assert.True(dict.ContainsKey("one two"));
            Assert.True(dict.ContainsKey("two three"));
            Assert.True(dict.ContainsKey("1"));
            Assert.True(dict.Keys.Count == 3);
            Assert.True(dict["one two"] == 2);
            Assert.True(dict["two three"] == 2);
            Assert.True(dict["1"] == 0);
        }
    }
}
