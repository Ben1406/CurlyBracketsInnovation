using FortisFramework.Utilities;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace FortisFramework.UserControls.Internal.SequenceRibbon;

[DesignTimeVisible(false)]
internal partial class SequenceRibbonGroupButton : UserControl
{
    public SequenceRibbonGroupButton()
    {
        InitializeComponent();
    }

    private int _id = 0;
    public int Id
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
        lblDescription.Foreground = Colors.IconColor;
        RootControl.BorderBrush = Colors.IconColor;
    }
}
