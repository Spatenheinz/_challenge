# Coding challenges

This is my attempt at solving 2 your two challenges.

## Counting words

run the following from within the CodeChallenge folder.
``` shell
$ dotnet run --project WordCounter -- textfiles/file1.txt textfiles/file2.txt
```

### Benchmarking

I have not spend too much time Benchmarking, but generating some large files via:

``` shell
$ yes repeat | head -c 1G > textfiles/bigfile.txt
```

and running

``` shell
$ dotnet build -c Release
$ dotnet WordCounter/bin/Release/net8.0/WordCounter.dll textfiles/bigfile.txt
```

Takes ~ 12.5 seconds on my own laptop. This can be improved a lot, but maybe its ok.

### Testing

simply do

``` shell
$ dotnet test
```

will run the tests for both exercises.

# Triangles

Nothing to run for triangles Except the tests.
