namespace FortisFramework;

public class Enums
{
    public enum MsSqlAuthenticationType
    {
        WindowsAuthentication,
        SqlServerAuthentication
    }

    public enum MsSqlConnectionSecurityEncryption
    {
        Optional,
        Mandatory,
        Strict
    }

    public enum SystemLanguage
    {
        Danish,
        English,
        German,
        Norwegian,
        Swedish
    }

    public enum ColorTheme
    {
        Dark,
        Light
    }

    public enum ScalingMode
    {
        Auto,
        Manual
    }

    public enum WindowStyle
    {
        FullScreen,
        Window,
        WindowXGA,
        WindowHD,
        WindowFullHD
    }
}
