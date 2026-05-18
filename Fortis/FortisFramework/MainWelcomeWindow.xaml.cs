using Autofac;
using FortisFramework.Utilities;
using System.Windows;

namespace FortisFramework;

public partial class MainWelcomeWindow : Window
{
    public MainWelcomeWindow()
    {
        InitializeComponent();

        CoreCompositionRoot.CreateCoreContainer();

        var welcome = CoreCompositionRoot.Container.Resolve<IMainWelcome>();
        ((MainWelcome)welcome).SystemStartup();

        var windowDeco = CoreCompositionRoot.Container.Resolve<ISystemDeco>();
        windowDeco.SetWindow(this);
        windowDeco.SetWindowColors(this);
    }
}
