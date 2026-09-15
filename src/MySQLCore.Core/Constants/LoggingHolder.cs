namespace MySQLCore.Core.Constants;

public readonly struct LoggingHolder
{
    public readonly string Class = "{class}";
    public readonly string Function = "{function}";

    public LoggingHolder(string className, string functionName)
    {
        Class = className;
        Function = functionName;
    }
}