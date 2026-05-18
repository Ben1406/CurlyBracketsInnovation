using Autofac;
using FortisFramework.Utilities;

namespace FortisFramework;

public interface IMainWelcome
{
    void SetInstanceName(string instanceName);

    event EventHandler<StationStartupEventArgs> OnStationStartup;
    event EventHandler<StationLoadedEventArgs> OnStationLoaded;
    event EventHandler<DevicesLoadedEventArgs> OnDevicesLoaded;
    event EventHandler<SequencesLoadedEventArgs> OnSequencesLoaded;
}

public class MainWelcome : IMainWelcome
{
    public event EventHandler<StationStartupEventArgs> OnStationStartup = delegate { };
    public event EventHandler<StationLoadedEventArgs> OnStationLoaded = delegate { };
    public event EventHandler<DevicesLoadedEventArgs> OnDevicesLoaded = delegate { };
    public event EventHandler<SequencesLoadedEventArgs> OnSequencesLoaded = delegate { };

    private readonly IStation _station;

    public MainWelcome()
    {
        _station = CoreCompositionRoot.Container.Resolve<IStation>();
    }

    public void SetInstanceName(string instanceName)
    {
        if (!string.IsNullOrEmpty(((Station)_station).InstanceName))
        {
            return;
        }

        ((Station)_station).InstanceName = instanceName;
    }

    internal void SystemStartup()
    {
        ((Station)_station).InitializeStartup();
        OnStationStartup(this, new StationStartupEventArgs());
        ((Station)_station).LoadStationFromServer();
        OnStationLoaded(this, new StationLoadedEventArgs());
        ((Station)_station).LoadDevicesFromServer();
        OnDevicesLoaded(this, new DevicesLoadedEventArgs());
        ((Station)_station).LoadSequencesFromServer();
        OnSequencesLoaded(this, new SequencesLoadedEventArgs());
        ((Station)_station).FinalizeStartup();
    }
}
