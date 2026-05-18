using System.ComponentModel;

namespace FortisFramework.UserControls.Internal.ToolRibbon;

public class ToolClockControlViewModel : INotifyPropertyChanged
{
    private string? _date = string.Empty;
    public string? DateAsString
    {
        get { return _date; }
        set
        {
            _date = value;
            NotifyPropertyChanged("DateAsString");
        }
    }

    private string? _day = string.Empty;
    public string? DayAsString
    {
        get { return _day; }
        set
        {
            _day = value;
            NotifyPropertyChanged("DayAsString");
        }
    }

    private string? _time = string.Empty;
    public string? TimeAsString
    {
        get { return _time; }
        set
        {
            _time = value;
            NotifyPropertyChanged("TimeAsString");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged is not null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
