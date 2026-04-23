namespace MySQLCore.Core.Constants;

public static class APIConstants
{
    public static string APIKey_NotFound { get; set; } = "API Key Not Found!";
    public static string APIKey_Invalid { get; set; } = "API Key Is Not Valid";
    public static string UnauthorizedClient { get; set; } = "Unauthorized Client";
    public static string CertificateLoaded { get; set; } = "✅ HTTPS Certificate loaded successfully!";
    public static string CertificateMissing { get; set; } = "⚠️ Certificate path or password is missing. Running without HTTPS.";
}

public readonly struct AppSettings
{
    public static readonly string API_KEY = "X-API-KEY";
    public static readonly string DB_Host = "DB_HOST";
    public static readonly string DB_Port = "DB_PORT";
    public static readonly string CERTIFICATE_FILE = "ASPNETCORE_Kestrel__Certificates__Default__Path";
    public static readonly string CERTIFICATE_PASSWORD = "ASPNETCORE_Kestrel__Certificates__Default__Password";
    public static readonly string MySQL_Database = "MYSQL_DATABASE";
    public static readonly string MySQL_User = "MYSQL_USER";
    public static readonly string MySQL_Password = "MYSQL_PASSWORD";        
    public static readonly string MySQL_Root_User = "MYSQL_ROOT_USER";
    public static readonly string MySQL_Root_Password = "MYSQL_ROOT_PASSWORD";
}