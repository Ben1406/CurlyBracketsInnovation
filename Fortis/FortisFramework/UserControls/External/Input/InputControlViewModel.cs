using System.ComponentModel;
using System.Windows;

namespace FortisFramework.UserControls.External.Input;

public class InputControlViewModel : INotifyPropertyChanged
{
    private string _headerText = null!;
    public string HeaderText
    {
        get { return _headerText; }
        set
        {
            _headerText = value;
            NotifyPropertyChanged("HeaderText");
        }
    }

    private Visibility _dropdownVisibility = Visibility.Hidden;
    public Visibility DropdownVisibility
    {
        get { return _dropdownVisibility; }
        set
        {
            _dropdownVisibility = value;
            NotifyPropertyChanged("DropdownVisibility");
        }
    }

    private object _typedValue = null!;
    public object TypedValue
    {
        get { return _typedValue; }
        set
        {
            _typedValue = value;
            NotifyPropertyChanged("TypedValue");
        }
    }

    private string _valueAsFormatString = "N3";
    public string ValueAsFormatString
    {
        get { return _valueAsFormatString; }
        set
        {
            _valueAsFormatString = value;
            NotifyPropertyChanged("ValueAsFormatString");
        }
    }

    public string DropDownImagePath => "pack://application:,,,/FortisFramework;Component/Images/Dropdown_Dark.png";

    public event PropertyChangedEventHandler? PropertyChanged;
    public void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged is not null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
