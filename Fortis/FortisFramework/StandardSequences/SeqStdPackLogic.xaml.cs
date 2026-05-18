using Autofac;
using FortisFramework.UserControls.External;

namespace FortisFramework.StandardSequences;

public partial class SeqStdPackLogic : SequenceLogicBase
{
    public override string? SequenceId => "StdPack";

    private readonly SeqStdPack _seqStdPack;

    public SeqStdPackLogic()
    {
        InitializeComponent();

        _seqStdPack = CoreCompositionRoot.Container.Resolve<SeqStdPack>();

        OnSequenceInitialized += OnSequenceInitializedEvent;
        OnSequenceFocused += OnSequenceFocusedEvent;
        OnSequenceUnfocused += OnSequenceUnfocusedEvent;
    }

    private void OnSequenceInitializedEvent(object? sender, SequenceInitializedEventArgs e)
    {
        _seqStdPack.SequenceInitialized();
    }

    private void OnSequenceFocusedEvent(object? sender, SequenceFocusedEventArgs e)
    {
        _seqStdPack.SequenceFocus();
    }

    private void OnSequenceUnfocusedEvent(object? sender, SequenceUnfocusedEventArgs e)
    {
        _seqStdPack.SequenceUnfocus();
    }
}
