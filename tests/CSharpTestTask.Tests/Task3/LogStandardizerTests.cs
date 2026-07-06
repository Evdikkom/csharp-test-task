using CSharpTestTask.Task3;

namespace CSharpTestTask.Tests.Task3;

public sealed class LogStandardizerTests
{
    [Fact]
    public void TryNormalizeLine_ShouldNormalizeFirstFormat()
    {
        const string input = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";
        const string expected = "10-03-2025\t15:14:49.523\tINFO\tDEFAULT\tВерсия программы: '3.4.0.48729'";

        bool parsed = LogStandardizer.TryNormalizeLine(input, out string actual);

        Assert.True(parsed);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryNormalizeLine_ShouldNormalizeSecondFormat()
    {
        const string input = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";
        const string expected = "10-03-2025\t15:14:51.5882\tINFO\tMobileComputer.GetDeviceId\tКод устройства: '@MINDEO-M40-D-410244015546'";

        bool parsed = LogStandardizer.TryNormalizeLine(input, out string actual);

        Assert.True(parsed);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("2025-99-10 15:14:51.5882| INFO|11|Method| Message")]
    [InlineData("10.03.2025 15:14:49.523 TRACE Message")]
    [InlineData("")]
    [InlineData("not a valid log line")]
    public void TryNormalizeLine_ShouldRejectInvalidLines(string input)
    {
        bool parsed = LogStandardizer.TryNormalizeLine(input, out _);

        Assert.False(parsed);
    }

    [Fact]
    public void ProcessFile_ShouldWriteValidLinesAndProblemsToSeparateFiles()
    {
        string directory = Path.Combine(Path.GetTempPath(), "CSharpTestTask_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            string inputPath = Path.Combine(directory, "input.txt");
            string outputPath = Path.Combine(directory, "output.txt");
            string problemsPath = Path.Combine(directory, "problems.txt");

            File.WriteAllLines(inputPath, new[]
            {
                "10.03.2025 15:14:49.523 WARNING Test warning",
                "not a valid log line"
            });

            ProcessingResult result = LogStandardizer.ProcessFile(inputPath, outputPath, problemsPath);

            Assert.Equal(2, result.TotalLines);
            Assert.Equal(1, result.ValidLines);
            Assert.Equal(1, result.ProblemLines);

            string[] outputLines = File.ReadAllLines(outputPath);
            string[] problemLines = File.ReadAllLines(problemsPath);

            Assert.Single(outputLines);
            Assert.Equal("10-03-2025\t15:14:49.523\tWARN\tDEFAULT\tTest warning", outputLines[0]);

            Assert.Single(problemLines);
            Assert.Equal("not a valid log line", problemLines[0]);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
