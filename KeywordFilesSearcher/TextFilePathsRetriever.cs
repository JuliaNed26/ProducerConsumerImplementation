namespace KeywordFilesSearcher;

internal static class TextFilePathsRetriever
{
    public static string[] GetFromPath(string path)
    {
        string[] textFiles = Directory.GetDirectories(path).SelectMany(dir =>
        {
            var files = Array.Empty<string>();
            try
            {
                files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                    .Where(filePath => filePath.EndsWith(".txt") 
                                       || filePath.EndsWith(".doc") 
                                       || filePath.EndsWith(".docx"))
                    .ToArray();
            }
            catch (UnauthorizedAccessException uae)
            {
            }

            return files;
        }).ToArray();

        return textFiles;
    }
}
