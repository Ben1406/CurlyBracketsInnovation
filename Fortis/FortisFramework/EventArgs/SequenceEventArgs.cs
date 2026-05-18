public class SequenceFocusedChangedEventArgs(string? sequenceId) : EventArgs
{
    public string? SequenceId { get; set; } = sequenceId;
}

public class SequenceFocusedEventArgs() : EventArgs
{

}

public class SequenceUnfocusedEventArgs() : EventArgs
{

}

public class SequenceInitializedEventArgs() : EventArgs
{

}

public class SequenceDeactivatedEventArgs() : EventArgs
{

}
