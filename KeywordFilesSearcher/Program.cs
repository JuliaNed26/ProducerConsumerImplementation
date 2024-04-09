using System.Diagnostics;
using KeywordFilesSearcher;

Console.Write("Enter the keyword you wish to search: ");
string keyword = Console.ReadLine()!;
var textFiles = TextFilePathsRetriever.GetFromPath(@"C:\\Users").ToList();

var stopWatch = new Stopwatch();

stopWatch.Start();
SearchWithProducerConsumerThreadPool();
stopWatch.Stop();

var withThreadPoolTime = stopWatch.Elapsed;
stopWatch.Reset();

stopWatch.Start();
SearchWithProducerConsumerThreadPerProducer();
stopWatch.Stop();

var withThreadPerProducersConsumersTime = stopWatch.Elapsed;
stopWatch.Reset();

stopWatch.Start();
SearchWithProducerConsumerParallelLibrary();
stopWatch.Stop();

var withParallelLibrary = stopWatch.Elapsed;
stopWatch.Reset();

stopWatch.Start();
await SearchWithProducerConsumerPartitionAsync().ConfigureAwait(false);
stopWatch.Stop();

var withPartitionTime = stopWatch.Elapsed;
stopWatch.Reset();

stopWatch.Start();
SearchWithDefinedCountOfThreads();
stopWatch.Stop();

var withDefinedNumberOfThreadsTime = stopWatch.Elapsed;
stopWatch.Reset();

stopWatch.Start();
SearchSynchronously();
stopWatch.Stop();

var synchronousTime = stopWatch.Elapsed;

Console.BackgroundColor = ConsoleColor.DarkRed;
Console.WriteLine($"\nSearch with producer/consumer and thread pool time: {withThreadPoolTime}");
Console.WriteLine($"\nSearch with producer/consumer and thread per producers consumer time:" +
                  $" {withThreadPerProducersConsumersTime}");
Console.WriteLine($"\nSearch with producer/consumer and parallel library time: {withParallelLibrary}");
Console.WriteLine($"\nSearch with producer/consumer and partition async time: {withPartitionTime}");
Console.WriteLine($"\nSearch with producer/consumer and defined count of threads time: {withDefinedNumberOfThreadsTime}");
Console.ResetColor();
void SearchWithProducerConsumerThreadPool()
{
    var keywordSearcher = new ProducerConsumerWithThreadPoolImplementation();

    keywordSearcher.ProduceAsync(keyword, textFiles);

    var consumer = Task.Run(() => keywordSearcher.Consume());
    consumer.Wait();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nProducer-consumer search with thread pool was finished\n");
    Console.ResetColor();
}

void SearchWithProducerConsumerThreadPerProducer()
{
    var keywordSearcher = new ProducerConsumerThreadPerProducerImplementation();

    keywordSearcher.ProduceAsync(keyword, textFiles);

    var consumer = new Thread(() => keywordSearcher.Consume());
    consumer.Start();
    consumer.Join();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nProducer-consumer search with thread per producers and consumer was finished\n");
    Console.ResetColor();
}

void SearchWithProducerConsumerParallelLibrary()
{
    var keywordSearcher = new ProducerConsumerTPLImplementation();

    keywordSearcher.ProduceAsync(keyword, textFiles);

    var consumer = Task.Run(() => keywordSearcher.Consume());
    consumer.Wait();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nProducer-consumer search with parallel library implementation was finished\n");
    Console.ResetColor();
}

async Task SearchWithProducerConsumerPartitionAsync()
{
    var keywordSearcher = new ProducerConsumerPartitionAsync();

    await keywordSearcher.ProduceAsync(keyword, textFiles).ConfigureAwait(false);

    var consumer = Task.Run(() => keywordSearcher.Consume());
    consumer.Wait();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nProducer-consumer search with partition tasks was finished\n");
    Console.ResetColor();
}


async void SearchWithDefinedCountOfThreads()
{
    var keywordSearcher = new ProducerConsumerWithDefinedNumberOfThreadsImplementation();

    keywordSearcher.ProduceAsync(keyword, textFiles);

    var consumer = new Thread(() => keywordSearcher.Consume());
    consumer.Start();
    consumer.Join();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nProducer-consumer search with partition into 12 threads was finished\n");
    Console.ResetColor();
}

void SearchSynchronously()
{
    var keywordSearcher = new SynchronousImplementation();
    keywordSearcher.SearchForFiles(keyword, textFiles);
    keywordSearcher.ShowFiles();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\nSynchronous search was finished\n");
    Console.ResetColor();
}