using System.Globalization;

namespace FortisFramework.Utilities;

public interface ILanguage
{
    event EventHandler<LanguageChangedEventArgs> OnLanguageChanged;
    CultureInfo CurrentCultureInfo { get; }

    void SetLanguage(Enums.SystemLanguage language);
    Enums.SystemLanguage CurrentLanguage { get; }
}

public class Language : ILanguage
{
    public event EventHandler<LanguageChangedEventArgs> OnLanguageChanged = delegate { };

    public Enums.SystemLanguage CurrentLanguage { get; private set; } = Enums.SystemLanguage.English;

    public void SetLanguage(Enums.SystemLanguage language)
    {
        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;

        Thread.CurrentThread.CurrentCulture = CurrentCultureInfo;
        Thread.CurrentThread.CurrentUICulture = CurrentCultureInfo;

        OnLanguageChanged(this, new LanguageChangedEventArgs(language));
    }

    public CultureInfo CurrentCultureInfo
    {
        get
        {
            switch (CurrentLanguage)
            {
                case Enums.SystemLanguage.Danish:
                    return new CultureInfo("da-DK");
                case Enums.SystemLanguage.Swedish:
                    return new CultureInfo("sv-SE");
                case Enums.SystemLanguage.Norwegian:
                    return new CultureInfo("nb-NO");
                case Enums.SystemLanguage.German:
                    return new CultureInfo("de-DE");
                case Enums.SystemLanguage.English:
                    return new CultureInfo("en-US");
                default:
                    return new CultureInfo("en-US");
            }
        }
    }
}
