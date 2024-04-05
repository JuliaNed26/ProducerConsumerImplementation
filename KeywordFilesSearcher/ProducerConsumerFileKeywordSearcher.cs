using System.Collections.Concurrent;

namespace KeywordFilesSearcher;

internal class ProducerConsumerFileKeywordSearcher
{
    private readonly BlockingCollection<string> _filesWithKeyword = new();

    public void ProduceAsync(string keyword)
    {
        string[] textFiles = TextFilePathsRetriever.GetFromPath(@"C:\");

        var readFileTasks = new List<Task>();
        foreach (var file in textFiles)
        {
            if (_filesWithKeyword.IsAddingCompleted)
            {
                break;
            }

            readFileTasks.Add(ReadAndSearchForKeywordAsync(keyword, file));
        }

        if (!_filesWithKeyword.IsAddingCompleted)
        {
            Task.WaitAll(readFileTasks.ToArray());
            StopSearch();
        }
    }

    public void Consume()
    {
        foreach (string file in _filesWithKeyword.GetConsumingEnumerable())
        {
            Console.WriteLine($"Found keyword in: {file}");
        }
    }

    public void StopSearch() => _filesWithKeyword.CompleteAdding();

    private Task ReadAndSearchForKeywordAsync(string keyword, string filePath)
    {
        return Task.Run(() =>
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
                Console.WriteLine(ex.Message);
            }
        });
    }
}
