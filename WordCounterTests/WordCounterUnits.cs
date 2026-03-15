using Xunit;
using CodeChallenge.WordCounter;
using static CodeChallenge.WordCounter.DelimiterFns;

namespace tests;

public class WordCounterTest {

    public static IEnumerable<object[]> CaseSensitiveTestData => new List<object[]>
    {
        new object[] { new[] { "simcorp", "engineer", "simcorp" }, new[] { ("simcorp", 2), ("engineer", 1) } },
        new object[] { new[] { "SimCorp", "world", "SIMCORP" }, new[] { ("SimCorp", 1), ("world", 1), ("SIMCORP", 1) } }
    };

    [Theory]
    [MemberData(nameof(CaseSensitiveTestData))]
    public void TestWordCounter(string[] input, (string word, int count)[] expected) {
        var result = WordCounter.CaseSensitiveOccurences(input);
        Assert.Equal(expected.Length, result.Count);
        foreach (var (word, count) in expected) {
            Assert.True(result.TryGetValue(word, out var actualCount));
            Assert.Equal(count, actualCount);
        }
    }

    public static IEnumerable<object[]> CaseInsensitiveTestData => new List<object[]>
    {
        new object[] { new[] { "SimCorp", "corpSim", "SimCorp" }, new[] { ("SimCorp", 2), ("corpSim", 1) } },
        new object[] { new[] { "SimCorp", "corpSim", "SIMCORP" }, new[] { ("SIMCORP", 2), ("corpSim", 1) } }
    };

    [Theory]
    [MemberData(nameof(CaseInsensitiveTestData))]
    public void TestCaseInsensitiveWordCounter(string[] input, (string word, int count)[] expected) {
        var result = WordCounter.CaseInsensitiveOccurences(input);
        Assert.Equal(expected.Length, result.Count);
        foreach (var (word, count) in expected) {
            Assert.True(result.TryGetValue(word, out var actualCount));
            Assert.Equal(count, actualCount);
        }
    }
}

public class TempFile {
    public static string Write(string content) {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, content);
        return path;
    }
}

public class WordStreamTest {

    [Theory]
    [InlineData("many test good", new[] { "many", "test", "good" })]
    [InlineData("  many test   good  ", new[] { "many", "test", "good" })]
    [InlineData("many test,good!", new[] { "many", "test", "good" })]
    [InlineData("many test(good)", new[] { "many", "test", "good" })]
    [InlineData("many test, good! (test", new[] { "many", "test", "good", "test" })]
    public void TestWordStream(string input, string[] expected) {
        var result = new WordStream(input, IsWhitespaceOrPunctuation).ToList();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestEmptyInput() {
        var result = new WordStream(string.Empty, IsWhitespaceOrPunctuation).ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void TestReadFile() {
        var path = TempFile.Write("many test good\nright?");
        var result = WordStream.FromFile(path, IsWhitespaceOrPunctuation).ToList();
        Assert.Equal(new[] { "many", "test", "good", "right" }, result);
    }

    [Fact]
    public void TestThrowsOnMissingFile() {
        Assert.Throws<FileNotFoundException>(() => WordStream.FromFile("missing_file.txt", IsWhitespace).ToList());
    }
}

public class FileWordCounterTest {

    [Fact]
    public void TestCountFileWords() {
        var path = TempFile.Write("many test good\nright? many test, no??? bad!");
        var result = WordCounter.CaseSensitiveOccurences(WordStream.FromFile(path, IsWhitespaceOrPunctuation));
        Assert.Equal(6, result.Count);
        Assert.True(result.TryGetValue("many", out var countMany));
        Assert.Equal(2, countMany);
        Assert.True(result.TryGetValue("test", out var countTest));
        Assert.Equal(2, countTest);
        Assert.True(result.TryGetValue("good", out var countGood));
        Assert.Equal(1, countGood);
        Assert.True(result.TryGetValue("no", out var countRight));
        Assert.Equal(1, countRight);
        Assert.True(result.TryGetValue("no", out var countNo));
        Assert.Equal(1, countNo);
        Assert.True(result.TryGetValue("bad", out var countBad));
        Assert.Equal(1, countBad);
    }
}
