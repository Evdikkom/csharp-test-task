using System.Text;

namespace CSharpTestTask.Task1;

/// <summary>
/// Задача 1. Сжимает строку по группам одинаковых букв и восстанавливает её обратно.
/// Например, строка "aaabbcccdde" превращается в "a3b2c3d2e".
/// </summary>
public static class StringCompression
{
    public static string Compress(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Length == 0)
        {
            return string.Empty;
        }

        var result = new StringBuilder(source.Length);
        char current = source[0];
        ValidateLowercaseLatin(current, nameof(source), 0);

        int count = 1;

        for (int i = 1; i < source.Length; i++)
        {
            char symbol = source[i];
            ValidateLowercaseLatin(symbol, nameof(source), i);

            if (symbol == current)
            {
                count++;
            }
            else
            {
                AppendGroup(result, current, count);
                current = symbol;
                count = 1;
            }
        }

        AppendGroup(result, current, count);
        return result.ToString();
    }

    public static string Decompress(string compressed)
    {
        ArgumentNullException.ThrowIfNull(compressed);

        if (compressed.Length == 0)
        {
            return string.Empty;
        }

        var result = new StringBuilder(compressed.Length);
        int i = 0;

        while (i < compressed.Length)
        {
            char symbol = compressed[i];
            ValidateLowercaseLatin(symbol, nameof(compressed), i);
            i++;

            int countStart = i;
            long count = 0;

            while (i < compressed.Length && IsAsciiDigit(compressed[i]))
            {
                count = count * 10 + (compressed[i] - '0');

                if (count > int.MaxValue)
                {
                    throw new FormatException("Group count is too large.");
                }

                i++;
            }

            if (countStart == i)
            {
                count = 1;
            }
            else
            {
                if (compressed[countStart] == '0')
                {
                    throw new FormatException("Group count cannot start with zero.");
                }

                if (count <= 0)
                {
                    throw new FormatException("Group count must be positive.");
                }
            }

            result.Append(symbol, (int)count);
        }

        return result.ToString();
    }

    private static void AppendGroup(StringBuilder result, char symbol, int count)
    {
        result.Append(symbol);

        if (count > 1)
        {
            result.Append(count);
        }
    }

    private static void ValidateLowercaseLatin(char symbol, string argumentName, int index)
    {
        if (!IsLowercaseLatin(symbol))
        {
            throw new ArgumentException(
                $"Only lowercase latin letters are allowed. Invalid symbol '{symbol}' at index {index}.",
                argumentName);
        }
    }

    private static bool IsLowercaseLatin(char symbol)
    {
        return symbol >= 'a' && symbol <= 'z';
    }

    private static bool IsAsciiDigit(char symbol)
    {
        return symbol >= '0' && symbol <= '9';
    }
}
