using System.Collections.Concurrent;

namespace WordCounterApp.Services
{
    public class FileWriter
    {
        private readonly string _directoryPath;

        public FileWriter(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public async Task WriteWordCountsToFileAsync(ConcurrentDictionary<string, int> wordCounts)
        {
            // Create a dictionary to store the writers for each letter
            var writers = new Dictionary<char, StreamWriter>();
            foreach (var letter in "abcdefghijklmnopqrstuvwxyzæøå")
            {
                var filePath = Path.Combine(_directoryPath + "/Results", $"FILE_{letter}.txt");
                var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
                var writer = new StreamWriter(stream);
                writers.Add(letter, writer);
            }

            // Write the word counts to the appropriate file based on their starting letter
            foreach (var pair in wordCounts)
            {
                var firstLetter = pair.Key[0];
                if (writers.TryGetValue(firstLetter, out var writer))
                {
                    await writer.WriteLineAsync($"{pair.Key} {pair.Value}");
                }
            }

            // Close all the writers
            foreach (var writer in writers.Values)
            {
                await writer.DisposeAsync();
            }
        }

        public async Task WriteExcludedWordsCountToFileAsync(int excludedWordsCount)
        {
            var excludedWordsCountFilePath = Path.Combine(_directoryPath + "/Results", "excluded_count.txt");
            using var stream = new FileStream(excludedWordsCountFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            using var writer = new StreamWriter(stream);

            await writer.WriteLineAsync($"Number of occurrences of excluded words: {excludedWordsCount}");
        }
    }
}