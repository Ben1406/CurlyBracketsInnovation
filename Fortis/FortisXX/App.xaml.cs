using System.Windows;

namespace FortisXX;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        CompositionRoot.CreateContainer();
    }
}
