
namespace KeywordFilesSearcher;

internal class SynchronousFileKeywordSearcher
{
    private readonly List<string> _filesWithKeyword = new();

    public void SearchForFiles(string keyword, IEnumerable<string> textFiles)
    {
        foreach (var filePath in textFiles)
        {
            try
            {
                using StreamReader sr = new StreamReader(filePath);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _filesWithKeyword.Add(filePath);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public void ShowFiles()
    {
        foreach (string file in _filesWithKeyword)
        {
            Console.WriteLine($"Found keyword in: {file}");
        }
    }
}
