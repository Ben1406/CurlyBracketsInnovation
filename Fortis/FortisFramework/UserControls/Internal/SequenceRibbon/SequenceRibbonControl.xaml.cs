using Autofac;
using FortisFramework.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FortisFramework.UserControls.Internal.SequenceRibbon;

[DesignTimeVisible(false)]
internal partial class SequenceRibbonControl : UserControl
{
    public ISequenceHandler _sequenceHandler = null!;
    public ISystemDeco _systemDeco = null!;

    private int? _focusedGroupId;

    public SequenceRibbonControl()
    {
        InitializeComponent();

        _sequenceHandler = CoreCompositionRoot.Container.Resolve<ISequenceHandler>();
        _systemDeco = CoreCompositionRoot.Container.Resolve<ISystemDeco>();
    }

    public void InitializeRibbons()
    {
        ((SequenceHandler)_sequenceHandler).OnSequenceFocusedChanged += OnSequenceFocusedChanged;

        var systemColors = _systemDeco.GetSystemColors();
        RootControl.Background = systemColors.RibbonBackgroundColor;
        RootControl.BorderBrush = systemColors.RibbonBorderColor;

        int? firstGroupId = null;
        string? firstSequenceId = null;
        var rowId = 0;
        var columnId = 1;

        var groups = _sequenceHandler.StationSequences.DistinctBy(x => x.GroupId).OrderBy(x => x.GroupId).ToList();
        for (var g = 0; g < groups.Count; g++)
        {
            var staGrp = groups[g];
            var grp = _sequenceHandler.SequenceGroups.FirstOrDefault(x => x.Id == staGrp.GroupId);

            if (grp is not null)
            {
                firstGroupId = firstGroupId ?? grp.Id;
                columnId = 1;

                RibbonGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(0)
                });

                var groupButton = new SequenceRibbonGroupButton()
                {
                    Name = $"seqGrpBtn_{grp.Id}",
                    Id = grp.Id,
                    Description = grp.Description
                };
                groupButton.Colors = systemColors;
                groupButton.MouseLeftButtonUp += GroupButton_MouseLeftButtonUp;
                RibbonGrid.Children.Add(groupButton);
                Grid.SetColumn(groupButton, 0);
                Grid.SetRow(groupButton, rowId);

                var sequences = _sequenceHandler.StationSequences.Where(x => x.GroupId == grp.Id).OrderBy(x => x.SortingId).ToList();
                for (var s = 0; s < sequences.Count; s++)
                {
                    var staSeq = sequences[s];
                    var seq = _sequenceHandler.Sequences.FirstOrDefault(x => x.Id == staSeq.SequenceId);

                    if (seq is not null)
                    {
                        firstSequenceId = firstSequenceId ?? seq.Id;
                        columnId += 1;

                        var sequenceButton = new SequenceRibbonItemButton()
                        {
                            Name = $"seqItmBtn_{seq.Id}",
                            Id = seq.Id,
                            GroupId = grp.Id,
                            Description = seq.Description,
                            SubDescription = seq.SubDescription
                        };
                        sequenceButton.Colors = systemColors;
                        sequenceButton.MouseLeftButtonUp += SequenceButton_MouseLeftButtonUp;
                        RibbonGrid.Children.Add(sequenceButton);
                        Grid.SetColumn(sequenceButton, columnId);
                        Grid.SetRow(sequenceButton, rowId);
                    }
                }

                rowId += 1;
            }
        }

        if (firstSequenceId is not null)
        {
            _sequenceHandler.SetFocusedSequence(firstSequenceId);
        }
        else if (firstGroupId is not null)
        {
            FocusRibbon(firstGroupId);
        }
    }

    private void OnSequenceFocusedChanged(object? sender, SequenceFocusedChangedEventArgs e)
    {
        var sequenceId = e.SequenceId;

        var staSeq = _sequenceHandler.StationSequences.FirstOrDefault(x => x.SequenceId == sequenceId);
        if (staSeq is null)
        {
            return;
        }

        if (staSeq.GroupId != _focusedGroupId)
        {
            FocusRibbon(staSeq.GroupId);
        }

        FocusSequence();
    }

    private void GroupButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_focusedGroupId is not null)
        {
            ShowAllRibbons();
        }
    }

    private void SequenceButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var sequenceId = ((SequenceRibbonItemButton)sender).Id;
        var groupId = ((SequenceRibbonItemButton)sender).GroupId;

        if (sequenceId != _sequenceHandler.FocusedSequence)
        {
            _sequenceHandler.SetFocusedSequence(sequenceId);
            return;
        }

        FocusRibbon(groupId);
    }

    private void FocusSequence()
    {
        var visibleSequenceId = _sequenceHandler.FocusedSequence;

        foreach (var gridButton in RibbonGrid.Children)
        {
            if (gridButton.GetType() == typeof(SequenceRibbonItemButton))
            {
                var seqButton = (SequenceRibbonItemButton)gridButton;
                seqButton.Selected = seqButton.Id == visibleSequenceId;
            }
        }
    }

    private void FocusRibbon(int? groupId)
    {
        _focusedGroupId = groupId;

        int? rowId = null!;

        foreach (var gridButton in RibbonGrid.Children)
        {
            if (gridButton.GetType() == typeof(SequenceRibbonGroupButton))
            {
                var grpButton = (SequenceRibbonGroupButton)gridButton;
                if (grpButton.Id == groupId)
                {
                    rowId = Grid.GetRow(grpButton);
                    break;
                }
            }
        }

        Grid.SetRowSpan(this, 1);

        for (var r = 0; r < RibbonGrid.RowDefinitions.Count; r++)
        {
            if (r == rowId)
            {
                RibbonGrid.RowDefinitions[r].Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                RibbonGrid.RowDefinitions[r].Height = new GridLength(0);
            }
        }
    }

    private void ShowAllRibbons()
    {
        _focusedGroupId = null;

        Grid.SetRowSpan(this, RibbonGrid.RowDefinitions.Count);

        for (var r = 0; r < RibbonGrid.RowDefinitions.Count; r++)
        {
            RibbonGrid.RowDefinitions[r].Height = new GridLength(1, GridUnitType.Star);
        }
    }
}
