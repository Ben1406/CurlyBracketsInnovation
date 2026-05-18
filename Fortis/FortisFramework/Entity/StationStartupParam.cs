namespace FortisFramework.Entity;

public class StationStartupParam
{
    public string StationName { get; set; } = string.Empty;
    public string MsSqlServerName { get; set; } = string.Empty;
    public string MsSqlDatabaseName { get; set; } = string.Empty;
    public bool MsSqlTrustServerCertificateEnabled { get; set; } = true;
    public string MsSqlAuthenticationType { get; set; } = null!;
    public string MsSqlConnectionSecurityEncryption { get; set; } = null!;
    public string MsSqlUserId { get; set; } = null!;
    public string MsSqlUserPassword { get; set; } = null!;
}
