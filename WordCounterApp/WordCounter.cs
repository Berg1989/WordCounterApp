using System.Collections.Concurrent;
using WordCounterApp.Services;

namespace WordCounterApp
{
    class WordCounter
    {
        private static async Task Main()
        {
            var directoryPath = GetDirectoryPathFromUserInput();

            var fileReader = new FileReader(directoryPath);
            var excludedWords = fileReader.GetExcludedWords();

            var wordsCount =
                new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var excludedWordsCount = 0;
            var tasks = new List<Task>();


            foreach (var filePath in Directory.EnumerateFiles(directoryPath))
            {
                if (filePath.Contains("exclude.txt", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                tasks.Add(Task.Run(() => fileReader.CountWordsInFile(filePath, wordsCount, excludedWords, ref excludedWordsCount)));
            }

            await Task.WhenAll(tasks);

            var fileWriter = new FileWriter(directoryPath);
            await fileWriter.WriteWordCountsToFileAsync(wordsCount);
            await fileWriter.WriteExcludedWordsCountToFileAsync(excludedWordsCount);

            Console.WriteLine($"Total non-excluded word count: {wordsCount.Values.Sum()}");
            Console.WriteLine($"Excluded word count: {excludedWordsCount}");
        }

        private static string GetDirectoryPathFromUserInput()
        {
            string directoryPath;
            Console.Write("Enter directory path: ");
            while (string.IsNullOrWhiteSpace(directoryPath = Console.ReadLine()))
                Console.WriteLine("Please provide ");
            return directoryPath;
        }
    }
}