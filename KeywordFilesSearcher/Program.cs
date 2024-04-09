using System.Diagnostics;
using KeywordFilesSearcher;

Console.Write("Enter the keyword you wish to search: ");
string keyword = Console.ReadLine()!;
var textFiles = TextFilePathsRetriever.GetFromPath(@"C:\\Users").ToList();

var stopWatch = new Stopwatch();

stopWatch.Start();
SearchWithProducerConsumer();
stopWatch.Stop();

var simultaneousTime = stopWatch.Elapsed;

stopWatch.Start();
SearchSynchronously();
stopWatch.Stop();

var synchronousTime = stopWatch.Elapsed;

Console.BackgroundColor = ConsoleColor.DarkRed;
Console.WriteLine($"\nSearch with producer/consumer time: {simultaneousTime}");
Console.WriteLine($"\nSearch with synchronous code time: {synchronousTime}");
Console.ResetColor();
void SearchWithProducerConsumer()
{
    var keywordSearcher = new ProducerConsumerFileKeywordSearcher();

    keywordSearcher.ProduceAsync(keyword, textFiles);

    var consumer = Task.Run(() => keywordSearcher.Consume());
    consumer.Wait();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nSimultaneous search was finished\n");
    Console.ResetColor();
}

void SearchSynchronously()
{
    var keywordSearcher = new SynchronousFileKeywordSearcher();
    keywordSearcher.SearchForFiles(keyword, textFiles);
    keywordSearcher.ShowFiles();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nSynchronous search was finished\n");
    Console.ResetColor();
}