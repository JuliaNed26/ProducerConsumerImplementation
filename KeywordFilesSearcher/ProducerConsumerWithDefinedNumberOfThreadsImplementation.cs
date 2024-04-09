using System.Collections.Concurrent;

namespace KeywordFilesSearcher;

internal class ProducerConsumerWithDefinedNumberOfThreadsImplementation
{
    private readonly BlockingCollection<string> _filesWithKeyword = new();

    public void ProduceAsync(string keyword, IEnumerable<string> textFiles)
    {
        var threads = Partitioner.Create(textFiles).GetPartitions(11)
            .Select(filePaths =>
            {
                var thread = new Thread(() => ReadAndSearchForKeyword(filePaths));
                thread.Start();
                return thread;
            });

        Parallel.ForEach(threads, thread => thread.Join());
        StopSearch();


        void ReadAndSearchForKeyword(IEnumerator<string> filePaths)
        {
            using (filePaths)
            {
                while (filePaths.MoveNext())
                {
                    try
                    {
                        using StreamReader sr = new StreamReader(filePaths.Current);
                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                            {
                                _filesWithKeyword.Add(filePaths.Current);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
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
}
