using System.Collections.Concurrent;

namespace KeywordFilesSearcher;

internal class ProducerConsumerTPLImplementation
{
    private readonly BlockingCollection<string> _filesWithKeyword = new();

    public void ProduceAsync(string keyword, IEnumerable<string> textFiles)
    {
        Parallel.ForEach(textFiles.ToArray(), new ParallelOptions()
        {
            MaxDegreeOfParallelism = 100
        },
            (file, state, _) =>
        {
            if (_filesWithKeyword.IsAddingCompleted)
            {
                state.Break();
            }

            ReadAndSearchForKeyword(keyword, file);
        });

        StopSearch();
    }

    public void Consume()
    {
        foreach (string file in _filesWithKeyword.GetConsumingEnumerable())
        {
            Console.WriteLine($"Found keyword in: {file}");
        }
    }

    public void StopSearch() => _filesWithKeyword.CompleteAdding();

    private void ReadAndSearchForKeyword(string keyword, string filePath)
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
