using System.Collections.Concurrent;

namespace KeywordFilesSearcher;

internal class ProducerConsumerThreadPerProducerImplementation
{
    private readonly BlockingCollection<string> _filesWithKeyword = new();

    public void ProduceAsync(string keyword, IEnumerable<string> textFiles)
    {
        var readFileTasks = new List<Thread>();
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
            Parallel.ForEach(readFileTasks, thread => thread.Join());
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

    private Thread ReadAndSearchForKeywordAsync(string keyword, string filePath)
    {
        var thread = new Thread(() =>
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
        });
        thread.Start();
        return thread;
    }
}
