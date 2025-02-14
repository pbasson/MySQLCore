namespace MySQLCore.Core.CoreHelpers;

public static class ObjectExtension
{
    public static bool NullChecker(this object? ob)
    {
        if (ob != null) { return true; }
        return false;
    }

    public static bool ZeroCheck(this int ob)
    {
        if (ob > 0) { return true; }
        return false;
    }

}
