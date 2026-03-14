using System.Diagnostics;
using CodeChallenge.WordCounter;

if (args.Length == 0) {
    Console.Error.WriteLine("usage: wordcounter <file1>...");
    Environment.Exit(1);
}

var sw = Stopwatch.StartNew();
var maps = FileWordCounter.ParallelCountWordsInFiles(args);
foreach (var map in maps) {
    foreach (var (word, count) in map.OrderByDescending(x => x.Value)
    ) {
        Console.WriteLine($"{word}: {count}");
    }
}
sw.Stop();
Console.WriteLine($"Processing took {sw.Elapsed}");
