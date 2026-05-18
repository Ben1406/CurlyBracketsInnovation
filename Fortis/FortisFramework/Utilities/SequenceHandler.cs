using Autofac;
using FortisFramework.Entity;
using FortisFramework.UserControls.External;
using FortisFramework.UserControls.Internal.Sequence;
using System.Windows;
using System.Windows.Controls;

namespace FortisFramework.Utilities;

public interface ISequenceHandler
{
    string FocusedSequence { get; }
    void SetFocusedSequence(string sequenceId);

    List<SequenceDbEntity> Sequences { get; }
    List<SequenceGroupDbEntity> SequenceGroups { get; }
    List<StationSequence> StationSequences { get; }
}

public class SequenceHandler : ISequenceHandler
{
    internal event EventHandler<SequenceFocusedChangedEventArgs> OnSequenceFocusedChanged = delegate { };

    private SeqDefaultLogic _sequenceDefaultLogic = new SeqDefaultLogic();
    private SequenceLogicBase _currentSequenceLogic = null!;
    
    public string FocusedSequence { get; private set; } = string.Empty;

    public SequenceHandler()
    {
    }

    public void SetFocusedSequence(string sequenceId)
    {
        if (sequenceId == FocusedSequence)
        {
            return;
        }

        var sequence = StationSequences.FirstOrDefault(x => x.SequenceId == sequenceId);

        FocusedSequence = sequenceId;
        OnSequenceFocusedChanged(this, new SequenceFocusedChangedEventArgs(sequence?.SequenceId));

        var welcomeWindow = Application.Current.Windows.OfType<MainWelcomeWindow>().SingleOrDefault();
        if (welcomeWindow is not null)
        {
            if (_currentSequenceLogic is not null)
            {
                _currentSequenceLogic.SequenceUnfocused();
                welcomeWindow.MainGrid.Children.Remove(_currentSequenceLogic);
            }

            var systemDeco = CoreCompositionRoot.Container.Resolve<ISystemDeco>();
            var systemColors = systemDeco.GetSystemColors();

            _currentSequenceLogic = sequence?.SequenceLogic ?? _sequenceDefaultLogic;
            
            _currentSequenceLogic.Background = systemColors.GeneralBackgroundColor;
            _currentSequenceLogic.Foreground = systemColors.GeneralForegroundColor;
            _currentSequenceLogic.BorderBrush = systemColors.GeneralBorderColor;
            systemDeco.SetColorsOnSequenceControls(_currentSequenceLogic);
            
            welcomeWindow.MainGrid.Children.Add(_currentSequenceLogic);
            Grid.SetColumn(_currentSequenceLogic, 0);
            Grid.SetColumnSpan(_currentSequenceLogic, 1);
            Grid.SetRow(_currentSequenceLogic, 1);
            Grid.SetRowSpan(_currentSequenceLogic, 14);
            _currentSequenceLogic.SequenceFocused();
        }
    }

    public List<SequenceDbEntity> Sequences { get; private set; } = new List<SequenceDbEntity>();
    public List<SequenceGroupDbEntity> SequenceGroups { get; private set; } = new List<SequenceGroupDbEntity>();
    public List<StationSequence> StationSequences { get; private set; } = new List<StationSequence>();

    internal bool CreateSequences(List<SequenceDbEntity> sequences)
    {
        var result = true;

        for (int i = 0; i < sequences.Count; i++)
        {
            var exists = Sequences.FirstOrDefault(x => x.Id == sequences[i].Id);
            if (exists is not null) result = false;
            else Sequences.Add(sequences[i]);
        }

        return result;
    }

    internal bool CreateSequenceGroups(List<SequenceGroupDbEntity> sequenceGroups)
    {
        var result = true;

        for (int i = 0; i < sequenceGroups.Count; i++)
        {
            var exists = SequenceGroups.FirstOrDefault(x => x.Id == sequenceGroups[i].Id);
            if (exists is not null) result = false;
            else SequenceGroups.Add(sequenceGroups[i]);
        }

        return result;
    }

    internal bool AddSequencesToStation(List<StationSequenceDbEntity> stationSequences)
    {
        var result = true;

        for (int i = 0; i < stationSequences.Count; i++)
        {
            var exists = StationSequences.FirstOrDefault(x => x.SequenceId == stationSequences[i].SequenceId);
            if (exists is not null) result = false;
            else
            {
                var staSeq = new StationSequence()
                {
                    SequenceId = stationSequences[i].SequenceId,
                    GroupId = stationSequences[i].GroupId,
                    SortingId = stationSequences[i].SortingId
                };
                StationSequences.Add(staSeq);
            };
        }

        return result;
    }

    internal void SetSequenceLogic(string sequenceId, SequenceLogicBase sequenceLogic)
    {
        var sequence = StationSequences.FirstOrDefault(x => x.SequenceId == sequenceId);
        if (sequence is not null)
        {
            sequence.SequenceLogic = sequenceLogic;
        }
    }
}
