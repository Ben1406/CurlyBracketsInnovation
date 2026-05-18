using Autofac;
using FortisCommunication.Serial;
using FortisDeviceCenter;
using FortisDeviceCenter.BarcodeScanner;
using FortisDeviceCenter.LabelPrinter;
using FortisDeviceCenter.Scale;
using FortisFramework.Entity;
using FortisFramework.UserControls.External;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace FortisFramework.Utilities;

public interface IStation
{
    string Name { get; }
    string Description { get; }
    Enums.ColorTheme ColorTheme { get; }
    Enums.ScalingMode ScalingMode { get; }
    double ScalingFactor { get; }
    Enums.WindowStyle WindowStyle { get; }

    string MsSqlConnectionString { get; }
    DateTime SystemStartupTime { get; }
    string VersionNumberCore { get; }
    string VersionNumberClient { get; }
    string WindowsComputerName { get; }
    string WindowsUserName { get; }
    string WindowsApplicationFolderPath { get; }
    string WindowsLocalApplicationDataFolderPath { get; }
    string WindowsDocumentFolderPath { get; }

    void CloseStation();
}

public class Station : IStation
{
    private readonly ISystemLogger _systemLogger = null!;
    private readonly ILanguage _language = null!;
    private readonly IDeviceCenter _deviceCenter = null!;
    private readonly ISequenceHandler _sequenceHandler = null!;
    private readonly FortisFrameworkDbContext _dbContext = null!;

    public Station()
    {
        _systemLogger = CoreCompositionRoot.Container.Resolve<ISystemLogger>();
        _language = CoreCompositionRoot.Container.Resolve<ILanguage>();
        _deviceCenter = CoreCompositionRoot.Container.Resolve<IDeviceCenter>();
        _sequenceHandler = CoreCompositionRoot.Container.Resolve<ISequenceHandler>();
        _dbContext = CoreCompositionRoot.Container.Resolve<FortisFrameworkDbContext>();
    }

    #region Properties

    internal string InstanceName { get; set; } = "Default";

    internal string MsSqlServerName { get; private set; } = string.Empty;
    internal string MsSqlDatabaseName { get; private set; } = string.Empty;
    internal bool MsSqlTrustServerCertificateEnabled { get; private set; } = true;
    internal Enums.MsSqlAuthenticationType MsSqlAuthenticationType { get; private set; } = Enums.MsSqlAuthenticationType.WindowsAuthentication;
    internal Enums.MsSqlConnectionSecurityEncryption MsSqlConnectionSecurityEncryption { get; private set; } = Enums.MsSqlConnectionSecurityEncryption.Optional;
    internal int MsSqlConnectionTimeout { get; private set; } = 30;
    internal string MsSqlUserId { get; private set; } = string.Empty;
    internal string MsSqlUserPassword { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Enums.ColorTheme ColorTheme { get; private set; } = Enums.ColorTheme.Light;
    public Enums.ScalingMode ScalingMode { get; private set; } = Enums.ScalingMode.Manual;
    public double ScalingFactor { get; private set; } = 1;
    public Enums.WindowStyle WindowStyle { get; private set; } = Enums.WindowStyle.FullScreen;
    public string LanguageCultureName { get; private set; } = "en-GB";

    public string MsSqlConnectionString => GetConnectionStringSettings().ConnectionString;
    public DateTime SystemStartupTime => GetStartupTime();
    public string VersionNumberCore => GetCoreVersionNumber();
    public string VersionNumberClient => GetClientVersionNumber();
    public string WindowsComputerName => GetComputerName();
    public string WindowsUserName => System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    public string WindowsApplicationFolderPath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? string.Empty;
    public string WindowsLocalApplicationDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public string WindowsDocumentFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

    #endregion

    #region Initialize Startup

    internal void InitializeStartup()
    {
        var appDataFolderPath = WindowsLocalApplicationDataFolderPath + "\\Fortis";
        if (!Directory.Exists(appDataFolderPath))
        {
            Directory.CreateDirectory(appDataFolderPath);
        }

        var startup = new StationStartupParam();

        var instanceName = !string.IsNullOrEmpty(InstanceName) ? InstanceName : "Default";
        var appDataFile = new FileInfo(appDataFolderPath + $"\\{instanceName}.ini");
        if (File.Exists(appDataFile.FullName))
        {
            TextReader? reader = null;

            try
            {
                reader = new StreamReader(appDataFile.FullName);
                var fileContents = reader.ReadToEnd();
                startup = JsonConvert.DeserializeObject<StationStartupParam>(fileContents);
            }
            finally
            {
                reader?.Close();
            }
        }
        else
        {
            startup = new StationStartupParam
            {
                StationName = "BennyVision",
                MsSqlServerName = "BENNYVISION",
                MsSqlDatabaseName = "FortisDB",
                MsSqlTrustServerCertificateEnabled = true,
                MsSqlAuthenticationType = "Windows",
                MsSqlConnectionSecurityEncryption = "Optional",
                MsSqlUserId = "Fortis",
                MsSqlUserPassword = "Fortis"
            };

            TextWriter? writer = null;

            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(startup);
                writer = new StreamWriter(appDataFile.FullName, false);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                writer?.Close();
            }
        }

        Name = startup?.StationName ?? string.Empty;
        MsSqlServerName = startup?.MsSqlServerName ?? string.Empty;
        MsSqlDatabaseName = startup?.MsSqlDatabaseName ?? string.Empty;
        MsSqlTrustServerCertificateEnabled = startup?.MsSqlTrustServerCertificateEnabled ?? true;
        MsSqlAuthenticationType = ConvertAuthenticationType(startup?.MsSqlAuthenticationType ?? string.Empty);
        MsSqlConnectionSecurityEncryption = ConvertSecurityEncryption(startup?.MsSqlConnectionSecurityEncryption ?? string.Empty);
        if (MsSqlAuthenticationType == Enums.MsSqlAuthenticationType.SqlServerAuthentication)
        {
            MsSqlUserId = startup?.MsSqlUserId ?? string.Empty;
            MsSqlUserPassword = startup?.MsSqlUserPassword ?? string.Empty;
        }

        _dbContext.ConnectionString = MsSqlConnectionString;
        _systemLogger.Initialize(Name, MsSqlConnectionString);

        _systemLogger.Information($"Fortis started | ComputerName: {WindowsComputerName} | CoreVersion: {VersionNumberCore} | ClientVersion: {VersionNumberClient}");
    }

    private ConnectionStringSettings GetConnectionStringSettings()
    {
        var integratedSecurity = MsSqlAuthenticationType == Enums.MsSqlAuthenticationType.WindowsAuthentication;
        var connectTimeout = MsSqlConnectionTimeout > 10 ? MsSqlConnectionTimeout : 10;

        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = MsSqlServerName ?? "",
            InitialCatalog = MsSqlDatabaseName ?? "",
            UserID = integratedSecurity ? string.Empty : MsSqlUserId ?? "",
            Password = integratedSecurity ? string.Empty : MsSqlUserPassword ?? "",
            ConnectTimeout = connectTimeout,
            MultipleActiveResultSets = true,
            IntegratedSecurity = integratedSecurity,
            TrustServerCertificate = MsSqlTrustServerCertificateEnabled
        };

        var connectionStringSettings =
            new ConnectionStringSettings("FortisConnectionString", connectionStringBuilder.ConnectionString)
            {
                ProviderName = "System.Data.SqlClient"
            };

        return connectionStringSettings;
    }

    private DateTime? _startupTime;
    private DateTime GetStartupTime()
    {
        if (_startupTime is null)
        {
            _startupTime = DateTime.Now;
        }

        return (DateTime)_startupTime;
    }

    private string GetCoreVersionNumber()
    {
        return Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString(4) ?? "";
    }

    private string GetClientVersionNumber()
    {
        return Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(4) ?? "";
    }

    private string GetComputerName()
    {
        return Environment.MachineName ?? "";
    }

    private Enums.MsSqlAuthenticationType ConvertAuthenticationType(string value)
    {
        if (value.Equals("Windows", StringComparison.InvariantCultureIgnoreCase))
        {
            return Enums.MsSqlAuthenticationType.WindowsAuthentication;
        }

        if (value.Equals("Sql", StringComparison.InvariantCultureIgnoreCase))
        {
            return Enums.MsSqlAuthenticationType.SqlServerAuthentication;
        }

        return Enums.MsSqlAuthenticationType.WindowsAuthentication;
    }

    private Enums.MsSqlConnectionSecurityEncryption ConvertSecurityEncryption(string value)
    {
        if (value.Equals("Optional", StringComparison.InvariantCultureIgnoreCase))
        {
            return Enums.MsSqlConnectionSecurityEncryption.Optional;
        }

        if (value.Equals("Mandatory", StringComparison.InvariantCultureIgnoreCase))
        {
            return Enums.MsSqlConnectionSecurityEncryption.Mandatory;
        }

        if (value.Equals("Strict", StringComparison.InvariantCultureIgnoreCase))
        {
            return Enums.MsSqlConnectionSecurityEncryption.Strict;
        }

        return Enums.MsSqlConnectionSecurityEncryption.Optional;
    }

    #endregion

    #region Load Station From Server

    internal void LoadStationFromServer()
    {
        var station = _dbContext.StationDb.FirstOrDefault(x => x.Name == Name);

        if (station is null)
        {
            _systemLogger.Error("Station settings not found in database");
            return;
        }

        Description = station.Description;
        _systemLogger.Information($"Station is loaded successfully (name: '{station.Name}', description: '{station.Description}', theme: '{station.ColorTheme}')");

        var languageParsed = Enum.TryParse<Enums.SystemLanguage>(station.Language, out var languageAsEnum);
        if (!languageParsed)
        {
            _systemLogger.Information($"Language not valid (language name: '{station.Language}')");
        }
        else
        {
            _language.SetLanguage(languageAsEnum);
            _systemLogger.Information($"Language '{station.Language}' enabled");
        }

        var colorThemeParsed = Enum.TryParse<Enums.ColorTheme>(station.ColorTheme, out var colorThemeAsEnum);
        if (!colorThemeParsed)
        {
            _systemLogger.Information($"Theme not valid (theme name: '{station.ColorTheme}')");
        }
        else
        {
            ColorTheme = colorThemeAsEnum;
            _systemLogger.Information($"Theme '{station.ColorTheme}' enabled");
        }

        var scalingParsed = Enum.TryParse<Enums.ScalingMode>(station.ScalingMode, out var scalingAsEnum);
        if (!scalingParsed)
        {
            _systemLogger.Information($"ScalingMode not valid (scaling mode: '{station.ScalingMode}')");
        }
        else
        {
            ScalingMode = scalingAsEnum;
            if (scalingAsEnum == Enums.ScalingMode.Manual)
            {
                ScalingFactor = station.ScalingFactor;
                _systemLogger.Information($"ScalingMode '{station.ScalingMode}', Factor '{station.ScalingFactor:#0.0}' enabled");
            }
            else
            {
                ScalingFactor = 1;
                _systemLogger.Information($"ScalingMode '{station.ScalingMode}', Factor '{station.ScalingFactor:#0.0}' enabled");
            }
        }

        var styleParsed = Enum.TryParse<Enums.WindowStyle>(station.WindowStyle, out var styleAsEnum);
        if (!styleParsed)
        {
            _systemLogger.Information($"WindowStyle not valid (window style: '{station.WindowStyle}')");
        }
        else
        {
            WindowStyle = styleAsEnum;
            _systemLogger.Information($"Window style '{station.WindowStyle}' enabled");
        }
    }

    #endregion

    #region Load Devices From Server

    internal void LoadDevicesFromServer()
    {
        var devices = _dbContext.StationDeviceDb
            .Where(x => x.StationName == Name)
            .ToList();

        if (devices.Count == 0)
        {
            return;
        }

        foreach (var device in devices)
        {
            var deviceName = device.DeviceName;
            device.DeviceSetting = _dbContext.DeviceSettingDb
                .FirstOrDefault(x => x.DeviceName == deviceName);
        }

        foreach (var device in devices)
        {
            var deviceName = device.DeviceName;
            var deviceSetting = device.DeviceSetting;
            var deviceId = device.DeviceId;

            if (deviceSetting is not null)
            {
                deviceSetting.SocketServerSetting = _dbContext.DeviceSocketServerSettingDb
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.SocketClientSetting = _dbContext.DeviceSocketClientSettingDb
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.SerialSetting = _dbContext.DeviceSerialSettingDb
                    .FirstOrDefault(x => x.DeviceName == deviceName);

                deviceSetting.ScaleSetting = _dbContext.DeviceScaleSettingDb
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.LabelPrinterSetting = _dbContext.DeviceLabelPrinterSettingDb
                    .FirstOrDefault(x => x.DeviceName == deviceName);
                deviceSetting.BarcodeScannerSetting = _dbContext.DeviceBarcodeScannerSettingDb
                    .FirstOrDefault(x => x.DeviceName == deviceName);

                var rxLogEnable = deviceSetting.RxLogEnable;
                var txLogEnable = deviceSetting.TxLogEnable;
                var autoConnect = deviceSetting.AutoConnect;

                var r1 = Enum.TryParse<DeviceType>(deviceSetting.Type, out var deviceTypeAsEnum);
                var r2 = Enum.TryParse<DeviceCommunication>(deviceSetting.Communication, out var deviceCommunicationAsEnum);
                if (!r1)
                {
                    _systemLogger.Error($"Device type '{deviceSetting.Type}' for '{deviceName}' is not correct");
                    return;
                }
                if (!r2)
                {
                    _systemLogger.Error($"Device communication '{deviceSetting.Communication}' for '{deviceName}' is not correct");
                    return;
                }

                var deviceSettings = new Device(deviceName, deviceSetting.Description, deviceSetting.SerialNumber, deviceTypeAsEnum, deviceCommunicationAsEnum)
                {
                    ClientDeviceId = deviceId,
                    RxLogEnable = rxLogEnable,
                    TxLogEnable = txLogEnable,
                    AutoConnect = autoConnect
                };
                _deviceCenter.CreateDevice(deviceName, deviceSettings);

                _systemLogger.Information($"Device is added successfully (devicename: '{deviceName}' deviceid: '{deviceId}', type: '{deviceSetting.Type}', communication: '{deviceSetting.Communication}', rxlog: '{rxLogEnable}', txlog: '{txLogEnable}', autoconnect: '{autoConnect}')");

                if (deviceTypeAsEnum == DeviceType.Scale)
                {
                    var scaleSetting = deviceSetting.ScaleSetting;
                    if (scaleSetting is not null)
                    {
                        var r111 = Enum.TryParse<ScaleType>(scaleSetting.Type, out var typeAsEnum);
                        var r112 = Enum.TryParse<ScaleProtocol>(scaleSetting.Protocol, out var protocolAsEnum);
                        if (!r111)
                        {
                            _systemLogger.Error($"Scale type '{scaleSetting.Type}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r112)
                        {
                            _systemLogger.Error($"Scale protocol '{scaleSetting.Protocol}' for '{deviceName}' is not correct");
                            return;
                        }

                        var settings = new DeviceSettings.ScaleSettings
                        {
                            Type = typeAsEnum,
                            Protocol = protocolAsEnum,
                            AllowTareFromIndicator = scaleSetting.AllowTareFromIndicator
                        };
                        _deviceCenter.AddScaleSettingsToDevice(deviceName, settings);

                        _systemLogger.Information($"Scale is added successfully (devicename: '{deviceName}', type: '{scaleSetting.Type}', protocol: '{scaleSetting.Protocol}', allowtarefromindicator: '{scaleSetting.AllowTareFromIndicator}')");
                    }
                }

                if (deviceTypeAsEnum == DeviceType.LabelPrinter)
                {
                    var labelPrinterSetting = deviceSetting.LabelPrinterSetting;
                    if (labelPrinterSetting is not null)
                    {
                        var r121 = Enum.TryParse<LabelPrinterType>(labelPrinterSetting.Type, out var typeAsEnum);
                        var r122 = Enum.TryParse<LabelPrinterProtocol>(labelPrinterSetting.Protocol, out var protocolAsEnum);
                        var r123 = Enum.TryParse<LabelPrinterDpi>(labelPrinterSetting.Dpi, out var dpiAsEnum);
                        var r124 = Enum.TryParse<LabelPrinterRotate>(labelPrinterSetting.Rotate, out var rotateAsEnum);
                        var r125 = Enum.TryParse<LabelPrinterPrintMode>(labelPrinterSetting.PrintMode, out var printModeAsEnum);
                        var r126 = Enum.TryParse<LabelPrinterMediaType>(labelPrinterSetting.MediaType, out var mediaTypeAsEnum);
                        var r127 = Enum.TryParse<LabelPrinterPaperType>(labelPrinterSetting.PaperType, out var paperTypeAsEnum);
                        if (!r121)
                        {
                            _systemLogger.Error($"Labelprinter type '{labelPrinterSetting.Type}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r122)
                        {
                            _systemLogger.Error($"Labelprinter protocol '{labelPrinterSetting.Protocol}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r123)
                        {
                            _systemLogger.Error($"Labelprinter dpi '{labelPrinterSetting.Dpi}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r124)
                        {
                            _systemLogger.Error($"Labelprinter rotate '{labelPrinterSetting.Rotate}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r125)
                        {
                            _systemLogger.Error($"Labelprinter printmode '{labelPrinterSetting.PrintMode}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r126)
                        {
                            _systemLogger.Error($"Labelprinter mediatype '{labelPrinterSetting.MediaType}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r127)
                        {
                            _systemLogger.Error($"Labelprinter papertype '{labelPrinterSetting.PaperType}' for '{deviceName}' is not correct");
                            return;
                        }

                        var settings = new DeviceSettings.LabelPrinterSettings
                        {
                            Type = typeAsEnum,
                            Protocol = protocolAsEnum,
                            Dpi = dpiAsEnum,
                            Rotate = rotateAsEnum,
                            PrintMode = printModeAsEnum,
                            MediaType = mediaTypeAsEnum,
                            PaperType = paperTypeAsEnum,
                            PrintSpeed = labelPrinterSetting.PrintSpeed,
                            LabelLength = labelPrinterSetting.LabelLength,
                            LabelWidth = labelPrinterSetting.LabelWidth,
                            StartAdjust = labelPrinterSetting.StartAdjust,
                            StopAdjust = labelPrinterSetting.StopAdjust,
                            LeftMargin = labelPrinterSetting.LeftMargin,
                            PrintButtonCopy = labelPrinterSetting.PrintButtonCopy,
                            ProgramBeforePrint = labelPrinterSetting.ProgramBeforePrint,
                            ProgramAfterPrint = labelPrinterSetting.ProgramAfterPrint
                        };
                        _deviceCenter.AddLabelPrinterSettingsToDevice(deviceName, settings);

                        _systemLogger.Information($"Labelprinter is added successfully (devicename: '{deviceName}', type: '{labelPrinterSetting.Type}', protocol: '{labelPrinterSetting.Protocol}', dpi: '{labelPrinterSetting.Dpi}', rotate: '{labelPrinterSetting.Rotate}', printmode '{labelPrinterSetting.PrintMode}', mediatype: '{labelPrinterSetting.MediaType}', papertype: '{labelPrinterSetting.PaperType}', printspeed: '{labelPrinterSetting.PrintSpeed}', labellength: '{labelPrinterSetting.LabelLength}', labelwidth: '{labelPrinterSetting.LabelWidth}', startadjust: '{labelPrinterSetting.StartAdjust}', stopadjust: '{labelPrinterSetting.StopAdjust}', leftmargin: '{labelPrinterSetting.LeftMargin}', printbuttoncopy: '{labelPrinterSetting.PrintButtonCopy}', programbeforeprint: '{labelPrinterSetting.ProgramBeforePrint}', programafterprint: '{labelPrinterSetting.ProgramAfterPrint}')");
                    }
                }

                if (deviceTypeAsEnum == DeviceType.BarcodeScanner)
                {
                    var barcodeScannerSetting = deviceSetting.BarcodeScannerSetting;
                    if (barcodeScannerSetting is not null)
                    {
                        var r131 = Enum.TryParse<BarcodeScannerType>(barcodeScannerSetting.Type, out var typeAsEnum);
                        var r132 = Enum.TryParse<BarcodeScannerProtocol>(barcodeScannerSetting.Protocol, out var protocolAsEnum);
                        if (!r131)
                        {
                            _systemLogger.Error($"Barcodescanner type '{barcodeScannerSetting.Type}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r132)
                        {
                            _systemLogger.Error($"Barcodescanner protocol '{barcodeScannerSetting.Protocol}' for '{deviceName}' is not correct");
                            return;
                        }

                        var settings = new DeviceSettings.BarcodeScannerSettings
                        {
                            Type = typeAsEnum,
                            Protocol = protocolAsEnum,
                            SendFeedbackToHost = barcodeScannerSetting.SendFeedbackToHost
                        };
                        _deviceCenter.AddBarcodeScannerSettingsToDevice(deviceName, settings);

                        _systemLogger.Information($"Barcodescanner is added successfully (devicename: '{deviceName}', type: '{barcodeScannerSetting.Type}', protocol: '{barcodeScannerSetting.Protocol}', sendfeedbacktohost: '{barcodeScannerSetting.SendFeedbackToHost}')");
                    }
                }

                if (deviceTypeAsEnum == DeviceType.Typeless)
                {
                }

                if (deviceCommunicationAsEnum == DeviceCommunication.SocketServer)
                {
                    var socketServerSetting = deviceSetting.SocketServerSetting;
                    if (socketServerSetting is not null)
                    {
                        var deviceEncoding = Helper.ConvertToEncoding(socketServerSetting.Encoding);
                        var settings = new DeviceSettings.SocketServerSettings
                        {
                            Encoding = deviceEncoding,
                            IpPort = socketServerSetting.IpPort
                        };
                        _deviceCenter.AddSocketServerSettingsToDevice(deviceName, settings);
                    }
                }

                if (deviceCommunicationAsEnum == DeviceCommunication.SocketClient)
                {
                    var socketClientSetting = deviceSetting.SocketClientSetting;
                    if (socketClientSetting is not null)
                    {
                        var deviceEncoding = Helper.ConvertToEncoding(socketClientSetting.Encoding);
                        var r211 = IPAddress.TryParse(socketClientSetting.IpAddress, out var ipAddress);
                        if (!r211 || ipAddress is null)
                        {
                            _systemLogger.Error($"Socketclient ipaddress '{socketClientSetting.IpAddress}' for '{deviceName}' is not correct");
                            return;
                        }

                        var settings = new DeviceSettings.SocketClientSettings
                        {
                            Encoding = deviceEncoding,
                            IpAddress = ipAddress,
                            IpPort = socketClientSetting.IpPort
                        };
                        _deviceCenter.AddSocketClientSettingsToDevice(deviceName, settings);

                        _systemLogger.Information($"Socketclient is added successfully (devicename: '{deviceName}', ipaddress: '{socketClientSetting.IpAddress}', ipport: '{socketClientSetting.IpPort}', encoding: '{socketClientSetting.Encoding}')");
                    }
                }

                if (deviceCommunicationAsEnum == DeviceCommunication.SerialComport)
                {
                    var serialSetting = deviceSetting.SerialSetting;
                    if (serialSetting is not null)
                    {
                        var deviceEncoding = Helper.ConvertToEncoding(serialSetting.Encoding);
                        var r221 = Enum.TryParse<SerialPortName>(serialSetting.PortName, out var portNameAsEnum);
                        var r222 = Enum.TryParse<SerialBaudRate>(serialSetting.BaudRate, out var baudRateAsEnum);
                        var r223 = Enum.TryParse<SerialParity>(serialSetting.Parity, out var parityAsEnum);
                        var r224 = Enum.TryParse<SerialDataBit>(serialSetting.DataBit, out var dataBitAsEnum);
                        var r225 = Enum.TryParse<SerialStopBit>(serialSetting.StopBit, out var stopBitAsEnum);
                        if (!r221)
                        {
                            _systemLogger.Error($"Serialcomport portname '{serialSetting.PortName}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r222)
                        {
                            _systemLogger.Error($"Serialcomport baudrate '{serialSetting.BaudRate}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r223)
                        {
                            _systemLogger.Error($"Serialcomport parity '{serialSetting.Parity}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r224)
                        {
                            _systemLogger.Error($"Serialcomport databit '{serialSetting.DataBit}' for '{deviceName}' is not correct");
                            return;
                        }
                        if (!r225)
                        {
                            _systemLogger.Error($"Serialcomport stopbit '{serialSetting.StopBit}' for '{deviceName}' is not correct");
                            return;
                        }

                        var settings = new DeviceSettings.SerialComportSettings
                        {
                            Encoding = deviceEncoding,
                            PortName = portNameAsEnum,
                            BaudRate = baudRateAsEnum,
                            Parity = parityAsEnum,
                            DataBit = dataBitAsEnum,
                            StopBit = stopBitAsEnum,
                            ReadBufferSize = serialSetting.ReadBufferSize,
                            WriteBufferSize = serialSetting.WriteBufferSize
                        };
                        _deviceCenter.AddSerialSettingsToDevice(deviceName, settings);

                        _systemLogger.Information($"Serialcomport is added successfully (devicename: '{deviceName}', portname: '{serialSetting.PortName}', baudrate: '{serialSetting.BaudRate}', parity: '{serialSetting.Parity}', databit: '{serialSetting.DataBit}', stopbit: '{serialSetting.StopBit}', readbuffersize: '{serialSetting.ReadBufferSize}', writebuffersize: '{serialSetting.WriteBufferSize}', encoding: '{serialSetting.Encoding}')");
                    }
                }

                _deviceCenter.FinalizeConnection(deviceName);
            }
        }
    }

    #endregion

    #region Load Sequences From Server

    internal void LoadSequencesFromServer()
    {
        var sequences = _dbContext.SequenceDb.ToList();
        if (sequences.Count == 0)
        {
            return;
        }
        ((SequenceHandler)_sequenceHandler).CreateSequences(sequences);

        var sequenceGroups = _dbContext.SequenceGroupDb.ToList();
        if (sequenceGroups.Count == 0)
        {
            return;
        }
        ((SequenceHandler)_sequenceHandler).CreateSequenceGroups(sequenceGroups);

        var stationSequences = _dbContext.StationSequenceDb.Where(x => x.StationName == Name).OrderBy(x => x.GroupId).ThenBy(x => x.SortingId).ToList();
        if (stationSequences is null || stationSequences.Count == 0)
        {
            return;
        }

        ((SequenceHandler)_sequenceHandler).AddSequencesToStation(stationSequences);

        var seqLogicList = CoreCompositionRoot.Container.ResolveOptional<IEnumerable<SequenceLogicBase>>()?.ToList();
        if (seqLogicList is null)
        {
            return;
        }

        foreach (var seqLogic in seqLogicList)
        {
            if (!string.IsNullOrEmpty(seqLogic.SequenceId))
            {
                var menuButtons = seqLogic.CreateMenuButtons();
                ((SequenceHandler)_sequenceHandler).SetSequenceLogic(seqLogic.SequenceId, seqLogic);
                seqLogic.SequenceInitialized();
            }
        }
    }

    #endregion

    #region Finalize Startup

    internal void FinalizeStartup()
    {
        var welcomeWindow = Application.Current.Windows.OfType<MainWelcomeWindow>().SingleOrDefault();

        if (welcomeWindow is not null)
        {
            welcomeWindow.SequenceRibbon.InitializeRibbons();
            welcomeWindow.ToolRibbon.InitializeRibbons();
        }
    }

    #endregion

    public void CloseStation()
    {
        Application.Current.Shutdown();
    }
}
