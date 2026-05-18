using FortisFramework;

public class LanguageChangedEventArgs(Enums.SystemLanguage language) : EventArgs
{
    public Enums.SystemLanguage Language { get; set; } = language;
}
