using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpTestTask.Task3;

/// <summary>
/// Задача 3. Основная логика для приведения строк лог-файла к единому формату.
/// На вход подаётся файл с логами, а на выходе получаются файл с корректными строками
/// и отдельный файл с записями, которые не удалось распознать.
/// </summary>
public static class LogStandardizer
{
    public const string DefaultMethod = "DEFAULT";

    // В задании есть разночтение: в тексте указан формат DD-MM-YYYY,
    // а в примере дата показана как yyyy-MM-dd. Оставляю формат из текстового требования.
    private const string OutputDateFormat = "dd-MM-yyyy";

    private static readonly Regex Format1Regex = new(
        @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}(?:\.\d+)?)\s+(?<level>[A-Za-z]+)\s+(?<message>.*)$",
        RegexOptions.CultureInvariant);

    private static readonly Regex Format2Regex = new(
        @"^(?<date>\d{4}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}:\d{2}(?:\.\d+)?)\|\s*(?<level>[A-Za-z]+)\s*\|\s*(?<thread>[^|]+)\s*\|\s*(?<method>[^|]*)\s*\|\s*(?<message>.*)$",
        RegexOptions.CultureInvariant);

    public static ProcessingResult ProcessFile(string inputPath, string outputPath, string problemsPath)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            throw new ArgumentException("Input path is required.", nameof(inputPath));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path is required.", nameof(outputPath));
        }

        if (string.IsNullOrWhiteSpace(problemsPath))
        {
            throw new ArgumentException("Problems path is required.", nameof(problemsPath));
        }

        var result = new ProcessingResult();
        var utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        using var writer = new StreamWriter(outputPath, append: false, encoding: utf8WithoutBom);
        using var problemWriter = new StreamWriter(problemsPath, append: false, encoding: utf8WithoutBom);

        foreach (string line in File.ReadLines(inputPath, Encoding.UTF8))
        {
            result.TotalLines++;

            if (TryNormalizeLine(line, out string normalizedLine))
            {
                writer.WriteLine(normalizedLine);
                result.ValidLines++;
            }
            else
            {
                problemWriter.WriteLine(line);
                result.ProblemLines++;
            }
        }

        return result;
    }

    public static bool TryNormalizeLine(string line, out string normalizedLine)
    {
        normalizedLine = string.Empty;

        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        if (TryParseFormat1(line, out StandardLogRecord? record) || TryParseFormat2(line, out record))
        {
            if (record is null)
            {
                return false;
            }

            normalizedLine = record.ToOutputLine();
            return true;
        }

        return false;
    }

    private static bool TryParseFormat1(string line, out StandardLogRecord? record)
    {
        record = null;
        Match match = Format1Regex.Match(line);

        if (!match.Success)
        {
            return false;
        }

        if (!DateTime.TryParseExact(
            match.Groups["date"].Value,
            "dd.MM.yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime date))
        {
            return false;
        }

        if (!TryNormalizeLevel(match.Groups["level"].Value, out string level))
        {
            return false;
        }

        record = new StandardLogRecord(
            date.ToString(OutputDateFormat, CultureInfo.InvariantCulture),
            match.Groups["time"].Value,
            level,
            DefaultMethod,
            match.Groups["message"].Value);

        return true;
    }

    private static bool TryParseFormat2(string line, out StandardLogRecord? record)
    {
        record = null;
        Match match = Format2Regex.Match(line);

        if (!match.Success)
        {
            return false;
        }

        if (!DateTime.TryParseExact(
            match.Groups["date"].Value,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime date))
        {
            return false;
        }

        if (!TryNormalizeLevel(match.Groups["level"].Value, out string level))
        {
            return false;
        }

        string method = match.Groups["method"].Value.Trim();

        if (string.IsNullOrEmpty(method))
        {
            method = DefaultMethod;
        }

        record = new StandardLogRecord(
            date.ToString(OutputDateFormat, CultureInfo.InvariantCulture),
            match.Groups["time"].Value,
            level,
            method,
            match.Groups["message"].Value);

        return true;
    }

    private static bool TryNormalizeLevel(string rawLevel, out string normalizedLevel)
    {
        normalizedLevel = string.Empty;
        string level = rawLevel.Trim().ToUpperInvariant();

        switch (level)
        {
            case "INFO":
            case "INFORMATION":
                normalizedLevel = "INFO";
                return true;

            case "WARN":
            case "WARNING":
                normalizedLevel = "WARN";
                return true;

            case "ERROR":
                normalizedLevel = "ERROR";
                return true;

            case "DEBUG":
                normalizedLevel = "DEBUG";
                return true;

            default:
                return false;
        }
    }
}
