using Autofac;
using FortisFramework;
using FortisFramework.StandardSequences;
using FortisFramework.UserControls.External;

namespace FortisXX;

public static class CompositionRoot
{
    public static IContainer Container { get; private set; } = null!;

    public static void CreateContainer()
    {
        var builder = new ContainerBuilder();
        Register(builder);
    }

    private static void Register(ContainerBuilder builder)
    {
        CoreCompositionRoot.Register(builder);

        var asm = typeof(CompositionRoot).Assembly;

        builder.RegisterAssemblyTypes(asm)
            .Where(type => type.GetInterfaces().Length != 0)
            .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(asm)
            .AssignableTo<SequenceLogicBase>()
            .As<SequenceLogicBase>()
            .SingleInstance();

        builder.RegisterAssemblyTypes(asm)
            .AssignableTo<SeqStdPack>()
            .As<SeqStdPack>()
            .SingleInstance();

        builder.RegisterType<Welcome>().As<MainWelcome>();

        var container = builder.Build();

        CoreCompositionRoot.Container = container;
    }
}
