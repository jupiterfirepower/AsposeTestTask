using Aspose.NLP.Core.Contracts;

namespace Aspose.NLP.Core.Grammar
{
    public class ExcludeGrammarHelper
    {
        private async Task<IEnumerable<string>> GetGrammarData()
        {
            var readData = async (string path) => await File.ReadAllLinesAsync(path);

            var getPath = () =>
            {
                var currentDir = Path.GetDirectoryName(this.GetType().Assembly.Location);
                return Path.Combine(currentDir, "grammar.txt");
            };
            
            return await readData(getPath());
        }

        public async Task<IEnumerable<T>> ExcludeGrammarWords<T>(IEnumerable<T> data) where T : IWordItemSummary
        {
            var grammarWords = await GetGrammarData();
            return data.AsParallel()
                   .Where(x => !grammarWords.Any(w => x.Word.ToLower().Contains(w.ToLower())))
                   .OrderBy(x => -x.Count);
        }
    }
}