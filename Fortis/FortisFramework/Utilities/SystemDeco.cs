using FortisFramework.UserControls.External;
using System.Windows;
using System.Windows.Media;

namespace FortisFramework.Utilities;

public interface ISystemDeco
{
    void SetWindow(Window window);
    void SetWindowDialog(Window window, double percentage);
    SystemColors SetWindowColors(Window window);
    void SetColorsOnSequenceControls(SequenceLogicBase sequenceLogicBase);
    SystemColors GetSystemColors();
}

public class SystemDeco : ISystemDeco
{
    public readonly IStation _station;

    public SystemDeco(IStation station)
    {
        _station = station;
    }

    public double ScalingFactor
    {
        get
        {
            var scalingFactor = _station.ScalingFactor;

            if (_station.ScalingMode == Enums.ScalingMode.Manual)
            {
                return scalingFactor;
            }

            var primaryScreenHeight = SystemParameters.PrimaryScreenHeight;

            switch (_station.WindowStyle)
            {
                case Enums.WindowStyle.FullScreen:
                case Enums.WindowStyle.Window:
                    primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
                    break;
                case Enums.WindowStyle.WindowXGA:
                    primaryScreenHeight = Globals.HeightXGA;
                    break;
                case Enums.WindowStyle.WindowHD:
                    primaryScreenHeight = Globals.HeightHD;
                    break;
                case Enums.WindowStyle.WindowFullHD:
                    primaryScreenHeight = Globals.HeightFullHD;
                    break;
            }

            scalingFactor = primaryScreenHeight / Globals.HeightXGA;

            return scalingFactor;
        }
    }

    public void SetWindow(Window window)
    {
        SetWindowSize(window, 100);
        SetCommonWindowStyle(window);
        SetCommonWindowScaling(window);
    }

    public void SetWindowDialog(Window window, double percentageSizeOfScreen)
    {
        if (percentageSizeOfScreen < 10) percentageSizeOfScreen = 10;
        if (percentageSizeOfScreen > 95) percentageSizeOfScreen = 95;

        SetWindowSize(window, percentageSizeOfScreen);
        SetCommonWindowStyle(window);
        SetCommonWindowScaling(window);

        window.Topmost = true;
    }

    public SystemColors SetWindowColors(Window window)
    {
        var systemsColors = GetSystemColors();

        var rootControl = window.Content as FrameworkElement;
        if (rootControl is null)
        {
            return systemsColors;
        }

        var backgroundProp = rootControl.GetType().GetProperty("Background");
        if (backgroundProp is not null) backgroundProp.SetValue(rootControl, systemsColors.GeneralBackgroundColor);

        var foregroundProp = rootControl.GetType().GetProperty("Foreground");
        if (foregroundProp is not null) foregroundProp.SetValue(rootControl, systemsColors.GeneralForegroundColor);

        var borderBrushProp = rootControl.GetType().GetProperty("BorderBrush");
        if (borderBrushProp is not null) borderBrushProp.SetValue(rootControl, systemsColors.GeneralBorderColor);

        return systemsColors;
    }

    public void SetColorsOnSequenceControls(SequenceLogicBase sequenceLogicBase)
    {
        var systemsColors = GetSystemColors();

        var rootControl = sequenceLogicBase.Content as FrameworkElement;
        if (rootControl is null)
        {
            return;
        }

        var controlCollection = new List<FrameworkElement>();
        GetControlChildCollection(rootControl, controlCollection);

        foreach (var control in controlCollection)
        {
            //var backgroundProp = control.GetType().GetProperty("Background");
            //if (backgroundProp is not null) backgroundProp.SetValue(control, systemsColors.GeneralBackgroundColor);

            var foregroundProp = control.GetType().GetProperty("Foreground");
            if (foregroundProp is not null) foregroundProp.SetValue(control, systemsColors.GeneralForegroundColor);

            var borderBrushProp = control.GetType().GetProperty("BorderBrush");
            if (borderBrushProp is not null) borderBrushProp.SetValue(control, systemsColors.GeneralBorderColor);
        }
    }

    public SystemColors GetSystemColors()
    {
        var colors = new SystemColors();

        switch (_station.ColorTheme)
        {
            case Enums.ColorTheme.Dark:
                colors.GeneralForegroundColor = Brushes.White;
                colors.GeneralBackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Dark) ?? Brushes.Black);
                colors.GeneralBorderColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Light) ?? Brushes.White);
                colors.RibbonBackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Light) ?? Brushes.White);
                colors.RibbonBorderColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Light) ?? Brushes.White);
                colors.IconColor = Brushes.Black;
                break;
            case Enums.ColorTheme.Light:
                colors.GeneralForegroundColor = Brushes.Black;
                colors.GeneralBackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Light) ?? Brushes.White);
                colors.GeneralBorderColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Dark) ?? Brushes.Black);
                colors.RibbonBackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Light) ?? Brushes.White);
                colors.RibbonBorderColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Dark) ?? Brushes.Black);
                colors.IconColor = Brushes.Black;
                break;
        }
        return colors;
    }

    private void SetWindowSize(Window window, double percentageSizeOfScreen)
    {
        var screenHeight = SystemParameters.VirtualScreenHeight;
        var screenWidth = SystemParameters.FullPrimaryScreenWidth;

        var windowState = WindowState.Maximized;
        var windowStartupLocation = WindowStartupLocation.CenterScreen;
        var windowHeight = SystemParameters.VirtualScreenHeight;
        var windowWidth = SystemParameters.FullPrimaryScreenWidth;
        var windowTop = (double)0;
        var windowLeft = (double)0;

        switch (_station.WindowStyle)
        {
            case Enums.WindowStyle.FullScreen:
                windowState = WindowState.Maximized;
                windowStartupLocation = WindowStartupLocation.CenterScreen;
                windowHeight = SystemParameters.VirtualScreenHeight;
                windowWidth = SystemParameters.FullPrimaryScreenWidth;
                windowTop = 0;
                windowLeft = 0;
                break;
            case Enums.WindowStyle.Window:
                windowState = WindowState.Maximized;
                windowStartupLocation = WindowStartupLocation.CenterScreen;
                windowHeight = SystemParameters.FullPrimaryScreenHeight;
                windowWidth = SystemParameters.FullPrimaryScreenWidth;
                windowTop = 0;
                windowLeft = 0;
                break;
            case Enums.WindowStyle.WindowXGA:
                windowState = WindowState.Normal;
                windowStartupLocation = WindowStartupLocation.Manual;
                windowHeight = Globals.HeightXGA;
                windowWidth = Globals.WidthXGA;
                windowTop = (screenHeight - windowHeight) / 2;
                windowLeft = (screenWidth - windowWidth) / 2;
                break;
            case Enums.WindowStyle.WindowHD:
                windowState = WindowState.Normal;
                windowStartupLocation = WindowStartupLocation.Manual;
                windowHeight = Globals.HeightHD;
                windowWidth = Globals.WidthHD;
                windowTop = (screenHeight - windowHeight) / 2;
                windowLeft = (screenWidth - windowWidth) / 2;
                break;
            case Enums.WindowStyle.WindowFullHD:
                windowState = WindowState.Normal;
                windowStartupLocation = WindowStartupLocation.Manual;
                windowHeight = Globals.HeightFullHD;
                windowWidth = Globals.WidthFullHD;
                windowTop = (screenHeight - windowHeight) / 2;
                windowLeft = (screenWidth - windowWidth) / 2;
                break;
        }

        window.Height = windowHeight / 100 * percentageSizeOfScreen;
        window.Width = windowWidth / 100 * percentageSizeOfScreen;
        window.Top = windowTop;
        window.Left = windowLeft;

        window.WindowState = windowState;
        window.WindowStartupLocation = windowStartupLocation;
    }

    private void SetCommonWindowStyle(Window window)
    {
        switch (_station.WindowStyle)
        {
            case Enums.WindowStyle.FullScreen:
                window.WindowStyle = WindowStyle.None;
                window.ResizeMode = ResizeMode.NoResize;
                break;
            case Enums.WindowStyle.Window:
            case Enums.WindowStyle.WindowXGA:
            case Enums.WindowStyle.WindowHD:
            case Enums.WindowStyle.WindowFullHD:
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.ResizeMode = ResizeMode.CanMinimize;
                break;
        }
    }

    private void SetCommonWindowScaling(Window window)
    {
        var rootControl = window.Content as FrameworkElement;
        if (rootControl is null)
        {
            return;
        }

        rootControl.LayoutTransform = new ScaleTransform(ScalingFactor, ScalingFactor);
    }

    private void GetControlChildCollection(DependencyObject parent, List<FrameworkElement> controlCollection)
    {
        var childrenOfParent = LogicalTreeHelper.GetChildren(parent);
        foreach (object child in childrenOfParent)
        {
            if (child is FrameworkElement)
            {
                controlCollection.Add((FrameworkElement)child);
                var dependencyChild = child as DependencyObject;
                if (dependencyChild is not null)
                {
                    GetControlChildCollection(dependencyChild, controlCollection);
                }
            }
        }
    }
}

public class SystemColors
{
    public SolidColorBrush GeneralForegroundColor { get; set; } = Brushes.White;
    public SolidColorBrush GeneralBackgroundColor { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Light) ?? Brushes.White);
    public SolidColorBrush GeneralBorderColor { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Dark) ?? Brushes.Black);

    public SolidColorBrush RibbonBackgroundColor { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Light) ?? Brushes.White);
    public SolidColorBrush RibbonBorderColor { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Dark) ?? Brushes.Black);

    public SolidColorBrush IconColor { get; set; } = Brushes.Black;

    public SolidColorBrush SelectedForegroundColor { get; set; } = Brushes.White;
    public SolidColorBrush SelectedBackgroundColor { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom(Globals.Dark) ?? Brushes.Black);
}
