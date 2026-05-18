using System.ComponentModel;
using System.Windows.Controls;

namespace FortisFramework.UserControls.External;

[DesignTimeVisible(false)]
public class SequenceLogicBase : UserControl
{
    public virtual string? SequenceId { get; } = null!;
    public virtual List<SequenceMenuButton>? CreateMenuButtons()
    {
        return MenuButtons;
    }

    public List<SequenceMenuButton>? MenuButtons { get; set; } = null!;

    public event EventHandler<SequenceInitializedEventArgs> OnSequenceInitialized = delegate { };
    public event EventHandler<SequenceFocusedEventArgs> OnSequenceFocused = delegate { };
    public event EventHandler<SequenceUnfocusedEventArgs> OnSequenceUnfocused = delegate { };

    public SequenceLogicBase()
    {
    }

    internal void SequenceInitialized()
    {
        OnSequenceInitialized(this, new SequenceInitializedEventArgs());
    }

    internal void SequenceFocused()
    {
        OnSequenceFocused(this, new SequenceFocusedEventArgs());
    }

    internal void SequenceUnfocused()
    {
        OnSequenceUnfocused(this, new SequenceUnfocusedEventArgs());
    }
}
