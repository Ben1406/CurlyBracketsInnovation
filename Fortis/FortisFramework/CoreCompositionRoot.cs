using Autofac;
using Autofac.Features.Variance;
using FortisDeviceCenter;
using FortisFramework.Entity;
using FortisFramework.StandardSequences;
using FortisFramework.UserControls.External;
using FortisFramework.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FortisFramework;

public static class CoreCompositionRoot
{
    public static IContainer Container { get; set; } = null!;

    public static void CreateCoreContainer()
    {
        if (Container is not null)
        {
            return;
        }

        var builder = new ContainerBuilder();
        Register(builder);

        Container = builder.Build();
    }

    public static void Register(ContainerBuilder builder)
    {
        builder.RegisterSource(new ContravariantRegistrationSource());

        builder.RegisterType<Language>().As<ILanguage>().SingleInstance();
        builder.RegisterType<MainWelcome>().As<IMainWelcome>();
        builder.RegisterType<SystemDeco>().As<ISystemDeco>();
        builder.RegisterType<SystemLogger>().As<ISystemLogger>().SingleInstance();
        builder.RegisterType<Station>().As<IStation>().SingleInstance();
        builder.RegisterType<DeviceCenter>().As<IDeviceCenter>().SingleInstance();
        builder.RegisterType<SequenceHandler>().As<ISequenceHandler>().SingleInstance();
        builder.RegisterType<Datastore>().As<IDatastore>().SingleInstance();
        builder.RegisterType<ThreadSafe>().As<IThreadSafe>().SingleInstance();

        var asm = typeof(CoreCompositionRoot).Assembly;

        builder.RegisterAssemblyTypes(asm)
            .AssignableTo<SequenceLogicBase>()
            .As<SequenceLogicBase>()
            .SingleInstance();

        builder.RegisterType<FortisFrameworkDbContext>();
        builder.RegisterType<DbContext>();
    }
}
