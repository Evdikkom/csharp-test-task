namespace CSharpTestTask.Task3;

public sealed class StandardLogRecord
{
    public StandardLogRecord(string date, string time, string level, string method, string message)
    {
        Date = date;
        Time = time;
        Level = level;
        Method = method;
        Message = message;
    }

    public string Date { get; }
    public string Time { get; }
    public string Level { get; }
    public string Method { get; }
    public string Message { get; }

    public string ToOutputLine()
    {
        return string.Join("\t", Date, Time, Level, Method, Message);
    }
}
