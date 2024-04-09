using System.Collections.Concurrent;

namespace KeywordFilesSearcher;

internal class ProducerConsumerPartitionAsync
{
    private readonly BlockingCollection<string> _filesWithKeyword = new();

    public async Task ProduceAsync(string keyword, IEnumerable<string> textFiles)
    {
        await Task.WhenAll(Partitioner.Create(textFiles).GetPartitions(100).AsParallel().Select(ReadAndSearchForKeyword));
        StopSearch();

        async Task ReadAndSearchForKeyword(IEnumerator<string> filePaths)
        {
            using (filePaths)
            {
                while (filePaths.MoveNext())
                {
                    await Task.Run(() =>
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
                    });
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
