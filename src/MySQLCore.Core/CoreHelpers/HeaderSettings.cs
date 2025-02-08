
namespace MySQLCore.Core.CoreHelpers;

public class HeaderSettings
{

}

public class PageSettings
{
    public int PageSize = 5; 

    public int SkipCount(int page)
    {
        return  (page - 1 ) * PageSize;
    } 
}


