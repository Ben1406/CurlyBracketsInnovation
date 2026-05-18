using System.Windows;
using System.Timers;
using System.Windows.Threading;

namespace FortisFramework.Utilities;

public interface IThreadSafe
{
    void Add(Action action);
}

public class ThreadSafe : IThreadSafe
{
    private readonly List<Action> _actions = new List<Action>();

    private System.Timers.Timer _executeTimer = null!;

    public ThreadSafe()
    {
        _executeTimer = new System.Timers.Timer { Interval = 50 };
        _executeTimer.Elapsed += OnExecute;
        _executeTimer.Start();
    }

    public void Add(Action action)
    {
        _actions.Add(action);
    }

    private void OnExecute(object? sender, ElapsedEventArgs e)
    {
        _executeTimer.Stop();

        if (_actions.Count > 0)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, _actions[0]);
            _actions.RemoveAt(0);
        }

        _executeTimer.Start();
    }
}
