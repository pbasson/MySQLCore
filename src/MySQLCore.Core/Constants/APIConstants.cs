namespace MySQLCore.Core.Constants;

public static class APIConstants
{
    public const string APIKey_NotFound = "API Key Not Found!";
    public const string APIKey_Invalid = "API Key Is Not Valid";
    public const string UnauthorizedClient = "Unauthorized Client";
    public const string CertificateLoaded = "✅ HTTPS Certificate loaded successfully!";
    public const string CertificateMissing = "⚠️ Certificate path or password is missing. Running without HTTPS.";
}

public readonly struct AppSettings
{
    public const string API_KEY = "X-API-KEY";
    public const string DB_Host = "DB_HOST";
    public const string DB_Port = "DB_PORT";
    public const string CERTIFICATE_FILE = "ASPNETCORE_Kestrel__Certificates__Default__Path";
    public const string CERTIFICATE_PASSWORD = "ASPNETCORE_Kestrel__Certificates__Default__Password";
    public const string MySQL_Database = "MYSQL_DATABASE";
    public const string MySQL_User = "MYSQL_USER";
    public const string MySQL_Password = "MYSQL_PASSWORD";        
    public const string MySQL_Root_User = "MYSQL_ROOT_USER";
    public const string MySQL_Root_Password = "MYSQL_ROOT_PASSWORD";
    public const string SEQ_URL = "SEQ_URL";
    public const string LOG_PATH = "SEQ_URL";
    public const string OTEL_EXPORTER_OTLP_ENDPOINT = "OTEL_EXPORTER_OTLP_ENDPOINT";
}

