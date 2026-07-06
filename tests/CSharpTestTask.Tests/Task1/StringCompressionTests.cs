using CSharpTestTask.Task1;

namespace CSharpTestTask.Tests.Task1;

public sealed class StringCompressionTests
{
    [Theory]
    [InlineData("aaabbcccdde", "a3b2c3d2e")]
    [InlineData("abc", "abc")]
    [InlineData("aaaaaaaaaaaab", "a12b")]
    [InlineData("", "")]
    public void Compress_ShouldReturnExpectedResult(string source, string expected)
    {
        string actual = StringCompression.Compress(source);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("a3b2c3d2e", "aaabbcccdde")]
    [InlineData("abc", "abc")]
    [InlineData("a12b", "aaaaaaaaaaaab")]
    [InlineData("", "")]
    public void Decompress_ShouldReturnExpectedResult(string compressed, string expected)
    {
        string actual = StringCompression.Decompress(compressed);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Decompress_AfterCompress_ShouldRestoreSourceString()
    {
        const string source = "zzzzzzzzzzaabcccccccccccc";

        string compressed = StringCompression.Compress(source);
        string decompressed = StringCompression.Decompress(compressed);

        Assert.Equal(source, decompressed);
    }

    [Fact]
    public void Compress_ShouldRejectNonLowercaseLatinLetters()
    {
        Assert.Throws<ArgumentException>(() => StringCompression.Compress("abcD"));
    }

    [Fact]
    public void Decompress_ShouldRejectInvalidSymbols()
    {
        Assert.Throws<ArgumentException>(() => StringCompression.Decompress("a3#"));
    }

    [Fact]
    public void Decompress_ShouldRejectCountWithLeadingZero()
    {
        Assert.Throws<FormatException>(() => StringCompression.Decompress("a03"));
    }
}
