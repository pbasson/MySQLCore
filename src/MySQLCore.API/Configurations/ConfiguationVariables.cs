using System;

namespace MySQLCore.API.Configurations;

public class ConfiguationVariables
{

}

public readonly struct AppSettings
{
    public static readonly string API_KEY = "X-API-KEY";
    public static readonly string DB_Host = "DB_HOST";
    public static readonly string DB_Port = "DB_PORT";
    public static readonly string MySQL_Database = "MYSQL_DATABASE";
    public static readonly string MySQL_User = "MYSQL_USER";
    public static readonly string MySQL_Password = "MYSQL_PASSWORD";        
    public static readonly string MySQL_Root_User = "MYSQL_ROOT_USER";
    public static readonly string MySQL_Root_Password = "MYSQL_ROOT_PASSWORD";
}

