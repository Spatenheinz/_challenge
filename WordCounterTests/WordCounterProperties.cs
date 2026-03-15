using System.Text;
using Xunit;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using CodeChallenge.WordCounter;
using static CodeChallenge.WordCounter.DelimiterFns;
using static tests.SharedWordStringGen;

namespace tests;

public class WordStreamProperties {
    [Property(Arbitrary = new[] { typeof(WordStringGen) })]
    public Property WordsNeverContainDelimiter(string input) {
        return new WordStream(input, IsWhitespaceOrPunctuation).All(word => !word.Any(IsWhitespaceOrPunctuation)).ToProperty();
    }

    [Property(Arbitrary = new[] { typeof(WordStringGen) })]
    public Property NoEmptyWords(string input) {
        return new WordStream(input, IsWhitespaceOrPunctuation).All(word => word != string.Empty).ToProperty();
    }

    [Property(Arbitrary = new[] { typeof(WordStringGen) })]
    public Property WordsAreSubstringsOfInput(string input) {
        return new WordStream(input, IsWhitespaceOrPunctuation).All(word => input.Contains(word)).ToProperty();
    }

    [Property(Arbitrary = new[] { typeof(CountedWordStringGen) })]
    public Property WordCountsAreCorrect(CountedWordString input) {
        var wordStream = new WordStream(input.Value, IsWhitespaceOrPunctuation);
        var counts = WordCounter.CaseSensitiveOccurences(wordStream);
        return input.WordCounts.All(kvp => counts.TryGetValue(kvp.Key, out var count) && count == kvp.Value).ToProperty();
    }
}

public static class SharedWordStringGen {
    public static Gen<char> DelimiterGen =>
        Gen.Elements(' ', '\t', '\n', ',', '.', '!', '?', '(', ')');

    public static Gen<char> LetterGen =>
        Gen.Elements("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());

    public static Gen<string> WordGen =>
        Gen.Choose(1, 10).SelectMany(len =>
            Gen.ArrayOf(LetterGen, len).Select(chars => new string(chars.ToArray()))
        );

    public static Gen<string> DelimiterStringGen =>
        Gen.NonEmptyListOf(DelimiterGen).Select(chars => new string(chars.ToArray()));
}

public class WordStringGen {

    private static Gen<string> Generate =>
        Gen.NonEmptyListOf(WordGen)
            .SelectMany(words =>
              Gen.ArrayOf(DelimiterStringGen, words.Count)
                .Select(delimiters =>
                  words.Zip(delimiters, (w, d) => w + d).Aggregate((acc, s) => acc + s)
            ));

    private static Arbitrary<string> Arbitrary() =>
        Arb.From(Generate);
}

public record CountedWordString(string Value, Dictionary<string, int> WordCounts);

public class CountedWordStringGen {

    private static CountedWordString MakeCountedString(string[] words, int[] counts) {
        var sb = new StringBuilder();
        var wordCounts = new Dictionary<string, int>();
        for (var i = 0; i < words.Length; i++) {
            var word = words[i];
            var count = counts[i];
            sb.Append(string.Concat(Enumerable.Repeat(word + " ", count)));
            wordCounts[word] = count;
        }
        return new CountedWordString(sb.ToString(), wordCounts);
    }

    private static Gen<CountedWordString> Generate =>
        from words in Gen.NonEmptyListOf(WordGen).Select(w => w.Distinct().ToList())
        from counts in Gen.ArrayOf(Gen.Choose(1, 7), words.Count)
        select MakeCountedString(words.ToArray(), counts);

    private static Arbitrary<CountedWordString> Arbitrary() =>
        Arb.From(Generate);
}


