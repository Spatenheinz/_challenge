using System.Text;
using System.Collections;

namespace CodeChallenge.WordCounter;

using WordMap = IReadOnlyDictionary<string, int>;
using DelimFn = Func<char, bool>;
public static class WordCounter {
    public static WordMap Occurences(IEnumerable<string> source, StringComparer comparer) {
        var counts = new Dictionary<string, int>(comparer);

        foreach (var word in source) {
            if (counts.TryGetValue(word, out var count)) {
                counts[word] = count + 1;
            }
            else {
                counts[word] = 1;
            }
        }
        return counts;
    }

    public static WordMap CaseSensitiveOccurences(IEnumerable<string> source) =>
        Occurences(source, StringComparer.Ordinal);

    public static WordMap CaseInsensitiveOccurences(IEnumerable<string> source) =>
        Occurences(source, StringComparer.OrdinalIgnoreCase);
}

public static class DelimiterFns {
    public static DelimFn IsWhitespace = char.IsWhiteSpace;
    public static DelimFn IsPunctuation = char.IsPunctuation;
    public static DelimFn IsWhitespaceOrPunctuation = c => char.IsWhiteSpace(c) || char.IsPunctuation(c);
    public static DelimFn IsParenthesis = c => c == '(' || c == ')';
    public static DelimFn IsWhitespaceParenthesisOrPunctation = c => IsWhitespaceOrPunctuation(c) || IsParenthesis(c);
}


public class WordStream : IEnumerable<string> {

    private readonly IEnumerable<char> _chars;
    private readonly DelimFn _delimiter_fn;
    public WordStream(IEnumerable<char> chars, DelimFn delimiter_fn) {
        _chars = chars;
        _delimiter_fn = delimiter_fn;
    }

    public static IEnumerable<string> FromFile(string path, DelimFn delimiter_fn) {
        return new WordStream(new FileCharStream(path), delimiter_fn);
    }

    public IEnumerator<string> GetEnumerator() {
        var sb = new StringBuilder();
        foreach (var c in _chars) {
            if (_delimiter_fn(c)) {
                if (sb.Length > 0) {
                    yield return sb.ToString();
                    sb.Clear();
                }
            }
            else {
                sb.Append(c);
            }
        }
        if (sb.Length > 0) {
            yield return sb.ToString();
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public class ChunkedWordStream : IEnumerable<string> {

    private readonly IEnumerable<string> _chars;
    private readonly DelimFn _delimiter_fn;
    public ChunkedWordStream(IEnumerable<string> chars, DelimFn delimiter_fn) {
        _chars = chars;
        _delimiter_fn = delimiter_fn;
    }

    public static IEnumerable<string> FromFile(string path, DelimFn delimiter_fn) {
        return new ChunkedWordStream(new FileChunkStream(path), delimiter_fn);
    }

    public IEnumerator<string> GetEnumerator() {
        var sb = new StringBuilder();
        foreach (var chunk in _chars) {
            foreach (var c in chunk) {
                if (_delimiter_fn(c)) {
                    if (sb.Length > 0) {
                        yield return sb.ToString();
                        sb.Clear();
                    }
                }
                else {
                    sb.Append(c);
                }
            }
        }
        if (sb.Length > 0) {
            yield return sb.ToString();
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class FileChunkStream : IEnumerable<string> {
    private readonly string _path;
    private readonly int _buffer_size;
    private readonly char[] _buffer;
    public FileChunkStream(string path, int bufferSize = 1024) {
        _path = path;
        _buffer_size = bufferSize;
        _buffer = new char[_buffer_size];
    }
    public IEnumerator<string> GetEnumerator() {
        using var reader = new StreamReader(_path);
        int read;
        while ((read = reader.Read(_buffer, 0, _buffer_size)) > 0) {
            yield return new string(_buffer, 0, read);
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class FileCharStream : IEnumerable<char> {
    private readonly string _path;
    public FileCharStream(string path) {
        _path = path;
    }
    public IEnumerator<char> GetEnumerator() {
        using var reader = new StreamReader(_path);
        while (!reader.EndOfStream) {
            yield return (char)reader.Read();
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public static class FileWordCounter {

    public static WordMap CountWordsInFile(string path) {
        // var stream = WordStream.FromFile(path, DelimiterFns.IsWhitespaceOrPunctuation);
        var stream = ChunkedWordStream.FromFile(path, DelimiterFns.IsWhitespaceOrPunctuation);
        return WordCounter.CaseSensitiveOccurences(stream);
    }

    public static IEnumerable<WordMap> CountWordsInFiles(IEnumerable<string> paths) =>
        paths.Select(path => CountWordsInFile(path));

    public static IEnumerable<WordMap> ParallelCountWordsInFiles(IEnumerable<string> paths) =>
        paths
            .AsParallel()
            .Select(path => CountWordsInFile(path));

    public static WordMap CountWordsAcrossFiles(IEnumerable<string> paths) {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        return ParallelCountWordsInFiles(paths)
            .Aggregate(counts, (acc, fileCounts) => {
                foreach (var (key, val) in fileCounts) {
                    if (acc.TryGetValue(key, out var count)) {
                        acc[key] = count + val;
                    }
                    else {
                        acc[key] = val;
                    }
                }
                return acc;
            });
    }
}
