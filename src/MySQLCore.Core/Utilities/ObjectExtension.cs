namespace MySQLCore.Core.Utilities;

public static class ObjectExtension
{
    public static bool IsNotNull(this object? ob)
    {
        if (ob != null) { return true; }
        return false;
    }

    public static bool IsNull(this object? ob)
    {
        if (ob == null) { return true; }
        return false;
    }

    public static bool IsZero(this int ob)
    {
        if (ob > 0) { return true; }
        return false;
    }

}
