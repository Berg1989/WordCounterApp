using System.Collections.Concurrent;

namespace WordCounterApp.Services
{
    public class FileReader
    {
        private static string _directoryPath;

        public FileReader(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public IEnumerable<string> GetExcludedWords()
        {
            var excludedWords = new List<string>();

            try
            {
                var excludeFilePath = Path.Combine(_directoryPath, "exclude.txt");
                if (!File.Exists(excludeFilePath))
                    return excludedWords;

                using var stream = File.OpenRead(excludeFilePath);
                using var reader = new StreamReader(stream);

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    excludedWords.AddRange(words);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading excluded words file: {e.Message}");
            }

            return excludedWords;
        }

        public void CountWordsInFile(string filePath, ConcurrentDictionary<string, int> wordsCount,
            IEnumerable<string> excludedWords, ref int excludedWordsCount)
        {
            try
            {
                using var stream = File.OpenRead(filePath);
                using var reader = new StreamReader(stream);

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var words = line.Split(new char[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        var normalizedWord = word.Trim().ToLower();
                        if (!excludedWords.Contains(normalizedWord))
                            wordsCount.AddOrUpdate(normalizedWord, 1, (_, count) => count + 1);
                        else
                            Interlocked.Increment(ref excludedWordsCount);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error counting words in file {filePath}: {e.Message}");
            }
        }
    }
}