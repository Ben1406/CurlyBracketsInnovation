using System.Windows.Controls;

namespace FortisFramework.UserControls.External;

public class SequenceMenuButton
{
    public SequenceMenuButton()
    {
        
    }

    public SequenceMenuButton(string id, string title, string? groupId, Image? iconImage)
    {
        Id = id;
        Title = title;
        GroupId = groupId;
        IconImage = iconImage;
    }

    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string InputGesture { get; set; } = null!;
    public string? GroupId { get; set; } = null!;
    public Image? IconImage { get; set; } = null!;

    public bool IsEnabled { get; set; } = true;
    public bool IsVisible { get; set; } = true;

    public event EventHandler<SequenceMenuButtonClickEventArgs> OnClick = delegate { };
    public event EventHandler<SequenceMenuButtonLeftClickEventArgs> OnLeftClick = delegate { };
    public event EventHandler<SequenceMenuButtonRightClickEventArgs> OnRightClick = delegate { };

    public void RaiseClickEvent()
    {
        OnClick?.Invoke(this, new SequenceMenuButtonClickEventArgs(this));
    }

    public void RaiseLeftClickEvent()
    {
        OnLeftClick?.Invoke(this, new SequenceMenuButtonLeftClickEventArgs(this));
    }

    public void RaiseRightClickEvent()
    {
        OnRightClick?.Invoke(this, new SequenceMenuButtonRightClickEventArgs(this));
    }

    public class SequenceMenuButtonClickEventArgs(SequenceMenuButton menuButton) : EventArgs
    {
        public SequenceMenuButton MenuButton { get; set; } = menuButton;
    }

    public class SequenceMenuButtonLeftClickEventArgs(SequenceMenuButton menuButton) : EventArgs
    {
        public SequenceMenuButton MenuButton { get; set; } = menuButton;
    }

    public class SequenceMenuButtonRightClickEventArgs(SequenceMenuButton menuButton) : EventArgs
    {
        public SequenceMenuButton MenuButton { get; set; } = menuButton;
    }
}
