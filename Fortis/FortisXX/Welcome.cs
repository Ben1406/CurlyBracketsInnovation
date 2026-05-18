using FortisFramework;

namespace FortisXX;

public class Welcome : MainWelcome
{
    public Welcome()
    {
        SetInstanceName("MyFirstFortiApp");

        OnStationStartup += Welcome_OnStationStartup;
        OnStationLoaded += Welcome_OnStationLoaded;
        OnDevicesLoaded += Welcome_OnDevicesLoaded;
        OnSequencesLoaded += Welcome_OnSequencesLoaded;
    }

    private void Welcome_OnStationStartup(object? sender, StationStartupEventArgs e)
    {
    }

    private void Welcome_OnStationLoaded(object? sender, StationLoadedEventArgs e)
    {
    }

    private void Welcome_OnDevicesLoaded(object? sender, DevicesLoadedEventArgs e)
    {
    }

    private void Welcome_OnSequencesLoaded(object? sender, SequencesLoadedEventArgs e)
    {
    }
}
