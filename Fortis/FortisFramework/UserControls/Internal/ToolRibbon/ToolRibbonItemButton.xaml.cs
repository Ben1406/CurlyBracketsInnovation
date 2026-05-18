using FortisFramework.Utilities;
using System.ComponentModel;
using System.Windows.Controls;

namespace FortisFramework.UserControls.Internal.ToolRibbon;

[DesignTimeVisible(false)]
internal partial class ToolRibbonItemButton : UserControl
{
    public ToolRibbonItemButton()
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

    private Image _iconImage = null!;
    public Image IconImage
    {
        get { return _iconImage; }
        set
        {
            _iconImage = value;

            if (_iconImage is null || _iconImage.Source is null)
            {
                imgIcon.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            imgIcon.Source = _iconImage.Source;
            imgIcon.Visibility = System.Windows.Visibility.Visible;
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

    public string? GroupId { get; set; } = null!;

    private void SetColors()
    {
        lblDescription.Foreground = Colors.IconColor;
    }
}
