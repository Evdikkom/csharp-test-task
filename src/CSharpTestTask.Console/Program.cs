using CSharpTestTask.Task3;

namespace CSharpTestTask.ConsoleApp;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("Usage: dotnet run --project src/CSharpTestTask.Console -- <input.txt> <output.txt> [problems.txt]");
                return 1;
            }

            string inputPath = args[0];
            string outputPath = args[1];
            string problemsPath = args.Length == 3
                ? args[2]
                : GetDefaultProblemsPath(outputPath);

            ProcessingResult result = LogStandardizer.ProcessFile(inputPath, outputPath, problemsPath);

            Console.WriteLine($"Processed: {result.TotalLines}");
            Console.WriteLine($"Valid:     {result.ValidLines}");
            Console.WriteLine($"Problems:  {result.ProblemLines}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }
    }

    private static string GetDefaultProblemsPath(string outputPath)
    {
        string fullOutputPath = Path.GetFullPath(outputPath);
        string? directory = Path.GetDirectoryName(fullOutputPath);

        if (string.IsNullOrEmpty(directory))
        {
            directory = Environment.CurrentDirectory;
        }

        return Path.Combine(directory, "problems.txt");
    }
}
