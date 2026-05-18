using FortisFramework.UserControls.External;
using System.ComponentModel;

namespace FortisFramework.UserControls.Internal.Sequence;

[DesignTimeVisible(false)]
internal partial class SeqDefaultLogic : SequenceLogicBase
{
    public override string? SequenceId => null!;

    public SeqDefaultLogic()
    {
        InitializeComponent();
    }
}
