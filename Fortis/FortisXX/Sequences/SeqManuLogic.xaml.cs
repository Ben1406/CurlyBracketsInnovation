using FortisFramework.UserControls.External;

namespace FortisXX.Sequences;

public partial class SeqManuLogic : SequenceLogicBase
{
    public override string? SequenceId => "Manu";

    public SeqManuLogic()
    {
        InitializeComponent();

        this.OnSequenceInitialized += OnSequenceInitializedEvent;
        this.OnSequenceFocused += OnSequenceFocusedEvent;
        this.OnSequenceUnfocused += OnSequenceUnfocusedEvent;
    }

    private void OnSequenceInitializedEvent(object? sender, SequenceInitializedEventArgs e)
    {

    }

    private void OnSequenceFocusedEvent(object? sender, SequenceFocusedEventArgs e)
    {
        Scale.Start();
    }

    private void OnSequenceUnfocusedEvent(object? sender, SequenceUnfocusedEventArgs e)
    {
        Scale.Stop();
    }
}
