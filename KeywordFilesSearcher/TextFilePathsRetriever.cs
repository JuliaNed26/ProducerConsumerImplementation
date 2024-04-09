namespace KeywordFilesSearcher;

internal static class TextFilePathsRetriever
{
    public static IEnumerable<string> GetFromPath(string path)
    {
        var fileEntries = Enumerable.Empty<string>();
        IEnumerable<string> dirEntries = Enumerable.Empty<string>();

        try
        {
            dirEntries = Directory.EnumerateDirectories(path);
            fileEntries = Directory.EnumerateFiles(path, "*.*").Where(file => file.EndsWith(".txt") ||
                                                                              file.EndsWith(".doc") ||
                                                                              file.EndsWith(".docx"));
        }
        catch (UnauthorizedAccessException)
        {
        }

        foreach (var dirPath in dirEntries)
        {
            fileEntries = fileEntries.Concat(GetFromPath(dirPath));
        }

        return fileEntries;
    }
}
