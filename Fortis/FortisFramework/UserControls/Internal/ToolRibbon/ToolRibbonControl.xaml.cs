using Autofac;
using FortisFramework.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FortisFramework.UserControls.Internal.ToolRibbon;

[DesignTimeVisible(false)]
internal partial class ToolRibbonControl : UserControl
{
    public ISystemDeco _systemDeco = null!;
    public IStation _station = null!;

    private string? _focusedGroupId;

    public ToolRibbonControl()
    {
        InitializeComponent();

        _systemDeco = CoreCompositionRoot.Container.Resolve<ISystemDeco>();
        _station = CoreCompositionRoot.Container.Resolve<IStation>();
    }

    public void InitializeRibbons()
    {
        var systemColors = _systemDeco.GetSystemColors();
        RootControl.Background = systemColors.RibbonBackgroundColor;
        RootControl.BorderBrush = systemColors.RibbonBorderColor;

        var toolMenuIcon = "ToolMenu_Dark.png";
        var toolDevicesIcon = "Devices_Dark.png";
        var toolCloseStationIcon = "Exit_Dark.png";

        var rowId = 0;

        #region Home Menu

        RibbonGrid.RowDefinitions.Add(new RowDefinition()
        {
            Height = new GridLength(0)
        });

        var mainMenuButton = new ToolRibbonGroupButton()
        {
            Name = "toolGrpBtn_Menu",
            Id = "Menu",
            Description = "Menu",
            IconImage = new Image { Source = new BitmapImage(new Uri(@$"pack://application:,,,/FortisFramework;Component/Images/{toolMenuIcon}", UriKind.Absolute)) }
        };
        mainMenuButton.Colors = systemColors;
        mainMenuButton.MouseLeftButtonUp += GroupButton_MouseLeftButtonUp;
        RibbonGrid.Children.Add(mainMenuButton);
        Grid.SetColumn(mainMenuButton, 0);
        Grid.SetRow(mainMenuButton, rowId);

        var devicesButton = new ToolRibbonItemButton()
        {
            Name = "toolGrpBtn_Devices",
            Id = "Devices",
            GroupId = "Menu",
            Description = "Devices",
            IconImage = new Image { Source = new BitmapImage(new Uri(@$"pack://application:,,,/FortisFramework;Component/Images/{toolDevicesIcon}", UriKind.Absolute)) }
        };
        devicesButton.Colors = systemColors;
        devicesButton.MouseLeftButtonUp += DeviceCenter_MouseLeftButtonUp;
        RibbonGrid.Children.Add(devicesButton);
        Grid.SetColumn(devicesButton, 2);
        Grid.SetRow(devicesButton, rowId);

        var closeStationButton = new ToolRibbonItemButton()
        {
            Name = "toolGrpBtn_CloseStation",
            Id = "CloseStation",
            GroupId = "Menu",
            Description = "Close Station",
            IconImage = new Image { Source = new BitmapImage(new Uri(@$"pack://application:,,,/FortisFramework;Component/Images/{toolCloseStationIcon}", UriKind.Absolute)) }
        };
        closeStationButton.Colors = systemColors;
        closeStationButton.MouseLeftButtonUp += CloseStation_MouseLeftButtonUp;
        RibbonGrid.Children.Add(closeStationButton);
        Grid.SetColumn(closeStationButton, 15);
        Grid.SetRow(closeStationButton, rowId);

        var toolClock = new ToolClockControl()
        {
        };
        toolClock.Colors = systemColors;
        RibbonGrid.Children.Add(toolClock);
        Grid.SetColumn(toolClock, 17);
        Grid.SetRow(toolClock, rowId);
        toolClock.Start();

        rowId -= 1;

        #endregion

        FocusRibbon("Menu");
    }

    private void GroupButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_focusedGroupId is not null)
        {
            ShowAllRibbons();
            return;
        }

        if (_focusedGroupId is null)
        {
            FocusRibbon("Menu");
        }
    }

    private void CloseStation_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _station.CloseStation();
    }

    private void DeviceCenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private void FocusRibbon(string? groupId)
    {
        _focusedGroupId = groupId;

        int? rowId = null!;

        foreach (var gridButton in RibbonGrid.Children)
        {
            if (gridButton.GetType() == typeof(ToolRibbonGroupButton))
            {
                var grpButton = (ToolRibbonGroupButton)gridButton;
                if (grpButton.Id == groupId)
                {
                    rowId = Grid.GetRow(grpButton);
                    break;
                }
            }
        }

        Grid.SetRow(this, 15);
        Grid.SetRowSpan(this, 1);

        for (var r = 0; r < RibbonGrid.RowDefinitions.Count; r++)
        {
            if (r == rowId)
            {
                RibbonGrid.RowDefinitions[r].Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                RibbonGrid.RowDefinitions[r].Height = new GridLength(0);
            }
        }
    }

    private void ShowAllRibbons()
    {
        if (RibbonGrid.RowDefinitions.Count == 1)
        {
            return;
        }

        _focusedGroupId = null;

        Grid.SetRow(this, 16 - RibbonGrid.RowDefinitions.Count);
        Grid.SetRowSpan(this, RibbonGrid.RowDefinitions.Count);

        for (var r = 0; r < RibbonGrid.RowDefinitions.Count; r++)
        {
            RibbonGrid.RowDefinitions[r].Height = new GridLength(1, GridUnitType.Star);
        }
    }
}
