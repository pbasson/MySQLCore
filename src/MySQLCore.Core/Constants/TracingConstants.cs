using System.Diagnostics;

namespace MySQLCore.Core.Constants;

public static class TracingConstants
{
    public const string SERVICE_NAME = "mysqlcore-api";
    public const string ACTIVITY_SOURCE = "MySQLCore.Messaging";
    public const string REPO_SOURCE = "MySQLCore.Repo";

    public static readonly ActivitySource MessagingActivitySource = new(ACTIVITY_SOURCE);
    public static readonly ActivitySource RepoActivitySource = new(REPO_SOURCE);
}