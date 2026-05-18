using Autofac;
using FortisFramework.Utilities;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace FortisFramework.UserControls.Internal.ToolRibbon;

[DesignTimeVisible(false)]
internal partial class ToolClockControl : UserControl
{
    private System.Timers.Timer? _timer = new System.Timers.Timer();

    public ToolClockControlViewModel ToolClockControlViewModel { get; private set; }

    public ToolClockControl()
    {
        InitializeComponent();

        ToolClockControlViewModel = new ToolClockControlViewModel();
        DataContext = ToolClockControlViewModel;

        UpdateTime();

        _timer.Interval = 1000;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        _timer.Elapsed += Timer_Elapsed;
    }

    public void Start()
    {
        _timer?.Start();
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        var currentTime = DateTime.Now;
        var dayOfWeek = (int)currentTime.DayOfWeek;

        var dateFormat = currentCulture.DateTimeFormat.ShortDatePattern;
        ToolClockControlViewModel.DateAsString = currentTime.ToString(dateFormat);
        var dayName = currentCulture.DateTimeFormat.DayNames[dayOfWeek];
        ToolClockControlViewModel.DayAsString = dayName.ToUpper();
        var timeFormat = currentCulture.DateTimeFormat.LongTimePattern;
        ToolClockControlViewModel.TimeAsString = currentTime.ToString(timeFormat);
    }

    private string _id = string.Empty;
    public string Id
    {
        get { return _id; }
        set
        {
            _id = value;
        }
    }

    private SystemColors _colors = new SystemColors();
    public SystemColors Colors
    {
        get { return _colors; }
        set
        {
            _colors = value;
            SetColors();
        }
    }

    private void SetColors()
    {
        lblDate.Foreground = Colors.IconColor;
        lblDay.Foreground = Colors.IconColor;
        lblTime.Foreground = Colors.IconColor;
    }
}
