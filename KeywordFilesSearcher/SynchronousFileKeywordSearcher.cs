﻿
namespace KeywordFilesSearcher;

internal class SynchronousFileKeywordSearcher
{
    private List<string> _filesWithKeyword = new();

    public void SearchForFiles(string keyword)
    {
        string[] textFiles = TextFilePathsRetriever.GetFromPath(@"C:\");
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
                Console.WriteLine(ex.Message);
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