using System.Collections.Concurrent;
using NUnit.Framework;
using WordCounterApp.Services;

namespace WordCounterApp.Tests;

[TestFixture]
public class FileReaderTests
{
    private string _testDirectoryPath;
    private string _testFilePath;

    [SetUp]
    public void SetUpFilesForTests()
    {
        // Files will be created in projectroot/bin/test directory
        _testDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "test");
        _testFilePath = Path.Combine(_testDirectoryPath, "test.txt");

        // create test directory if it doesn't exist
        if (!Directory.Exists(_testDirectoryPath))
            Directory.CreateDirectory(_testDirectoryPath);

        // create test file
        using (var writer = new StreamWriter(_testFilePath))
        {
            writer.WriteLine("The quick brown fox");
            writer.WriteLine("jumps over the lazy dog");
            writer.WriteLine("fox dog");
        }
    }

    [Test]
    public void GetExcludedWords_ReturnsOnlyExcludedWords()
    {
        // Arrange
        var fileReader = new FileReader(_testDirectoryPath);

        // Act
        var excludedWords = fileReader.GetExcludedWords();

        // Assert
        Assert.That(excludedWords.Contains("the"), Is.True);
        Assert.That(excludedWords.Contains("over"), Is.True);
        Assert.That(excludedWords.Count() == 2, Is.True);
    }

    [Test]
    public void GetExcludedWords_WhenFileExists_ReturnsExcludedWords()
    {
        // Arrange
        var excludeFilePath = Path.Combine(_testDirectoryPath, "exclude.txt");
        var excludedWordsList = new List<string> { "the", "over" };
        using (var writer = new StreamWriter(excludeFilePath))
        {
            foreach (var word in excludedWordsList)
            {
                writer.WriteLine(word);
            }
        }

        var fileReader = new FileReader(_testDirectoryPath);

        // Act
        var excludedWords = fileReader.GetExcludedWords();

        // Assert
        Assert.AreEqual(excludedWordsList, excludedWords);
    }

    [Test]
    public void CountWordsInFile_WhenFileExists_CountsWords()
    {
        // Arrange
        var fileReader = new FileReader(_testDirectoryPath);
        var wordsCount = new ConcurrentDictionary<string, int>();
        var excludedWords = new List<string> { "the", "over" };
        var excludedWordsCount = 0;
        var test = wordsCount.Keys.Contains("dog");

        // Act
        fileReader.CountWordsInFile(_testFilePath, wordsCount, excludedWords, ref excludedWordsCount);

        // Assert
        Assert.That(wordsCount.Count, Is.EqualTo(6));
        Assert.Multiple(() =>
        {
            Assert.That(wordsCount["quick"], Is.EqualTo(1));
            Assert.That(wordsCount["brown"], Is.EqualTo(1));
            Assert.That(wordsCount["fox"], Is.EqualTo(2));
            Assert.That(wordsCount["jumps"], Is.EqualTo(1));
            Assert.That(wordsCount["lazy"], Is.EqualTo(1));
            Assert.That(wordsCount["dog"], Is.EqualTo(2));
        });
        Assert.Multiple(() =>
        {
            Assert.That(wordsCount.Keys.Contains("the"), Is.False);
            Assert.That(wordsCount.Keys.Contains("over"), Is.False);
            Assert.That(excludedWordsCount, Is.EqualTo(3));
        });
    }

    [Test]
    public void CountWordsInFile_WhenFileDoesNotExist_DoesNotThrowException()
    {
        // Arrange
        var fileReader = new FileReader(_testDirectoryPath);
        var wordsCount = new ConcurrentDictionary<string, int>();
        var excludedWords = new List<string> { "the", "over" };
        var excludedWordsCount = 0;

        // Act
        fileReader.CountWordsInFile("nonexistent.txt", wordsCount, excludedWords, ref excludedWordsCount);

        // Assert
        // no exception thrown
    }

    [TearDown]
    public void TearDown()
    {
        // Delete the temporary file and directory created for the test
        File.Delete(_testFilePath);
    }
}