using FortisFramework.UserControls.External;
using FortisFramework.Utilities;

namespace FortisFramework.Entity;

public class StationSequence
{
    public string SequenceId { get; set; } = null!;

    public int GroupId { get; set; }
    public int SortingId { get; set; }

    //public SequenceBusinessLogic? BusinessLogic { get; set; } = null!;
    public SequenceLogicBase? SequenceLogic { get; set; } = null!;
}
