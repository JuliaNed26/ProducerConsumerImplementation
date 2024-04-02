using System.Diagnostics;
using System.Runtime.CompilerServices;
using KeywordFilesSearcher;
using TextEncryptor;

Console.InputEncoding = System.Text.Encoding.Unicode;
Console.OutputEncoding = System.Text.Encoding.Unicode;

Console.Write("Enter the keyword you wish to search: ");
string keyword = Console.ReadLine()!;

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

    keywordSearcher.Produce(keyword);

    var consumer = Task.Run(() => keywordSearcher.Consume());
    consumer.Wait();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\nSimultaneous search was finished\n");
    Console.ResetColor();
}

void SearchSynchronously()
{
    var keywordSearcher = new SynchronousFileKeywordSearcher();
    keywordSearcher.SearchForFiles(keyword);
    keywordSearcher.ShowFiles();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\nSynchronous search was finished\n");
    Console.ResetColor();
}