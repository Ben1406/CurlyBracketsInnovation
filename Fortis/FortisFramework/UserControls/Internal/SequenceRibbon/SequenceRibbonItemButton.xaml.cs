using FortisFramework.Utilities;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace FortisFramework.UserControls.Internal.SequenceRibbon;

[DesignTimeVisible(false)]
internal partial class SequenceRibbonItemButton : UserControl
{
    public SequenceRibbonItemButton()
    {
        InitializeComponent();
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

    private string? _description = string.Empty;
    public string? Description
    {
        get { return _description; }
        set
        {
            _description = value;
            lblDescription.Content = _description;
        }
    }

    private string? _subDescription = string.Empty;
    public string? SubDescription
    {
        get { return _subDescription; }
        set
        {
            _subDescription = value;
            lblSubDescription.Content = _subDescription;
        }
    }

    private bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            SetColors();
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

    public int? GroupId { get; set; } = null!;

    private void SetColors()
    {
        lblDescription.Foreground = _selected ? Colors.SelectedForegroundColor : Colors.IconColor;
        lblSubDescription.Foreground = _selected ? Colors.SelectedForegroundColor : Colors.IconColor;
        RootControl.Background = _selected ? Colors.SelectedBackgroundColor : Brushes.Transparent;
        RootControl.BorderBrush = Colors.IconColor;
    }
}
