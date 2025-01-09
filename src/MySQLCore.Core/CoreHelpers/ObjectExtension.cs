namespace MySQLCore.Core.CoreHelpers;

public static class ObjectExtension
{
    public static bool NullChecker(this object? ob)
    {
        if (ob != null) { return true; }
        
        return false;
    }

    // public static bool NullListChecker(this List<object>? list)
    // {
    //     if (list != null && list.Any())
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

}
