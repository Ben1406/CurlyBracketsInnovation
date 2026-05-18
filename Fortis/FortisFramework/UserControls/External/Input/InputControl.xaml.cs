using System.Windows.Controls;

namespace FortisFramework.UserControls.External.Input;

public partial class InputControl : UserControl
{
    public InputControlViewModel InputControlViewModel { get; private set; }

    public InputControl()
    {
        InitializeComponent();

        InputControlViewModel = new InputControlViewModel();
        DataContext = InputControlViewModel;
    }

    private string _headerText = null!;
    public string HeaderText
    {
        get { return _headerText; }
        set
        {
            _headerText = value;
            InputControlViewModel.HeaderText = value;
        }   
    }

    private bool _isDropdown = false;
    public bool IsDropdown
    {
        get { return _isDropdown; }
        set
        {
            _isDropdown = value;
            InputControlViewModel.DropdownVisibility = _isDropdown ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
    }
}
