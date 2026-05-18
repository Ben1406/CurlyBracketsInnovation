using Autofac;
using FortisFramework.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FortisFramework.Entity;

public class FortisFrameworkDbContext : DbContext
{
    public FortisFrameworkDbContext()
    {
    }

    internal string ConnectionString { get; set; } = null!;

    public DbSet<StationDbEntity> StationDb { get; set; } = null!;
    public DbSet<StationDeviceDbEntity> StationDeviceDb { get; set; } = null!;
    public DbSet<DeviceSettingDbEntity> DeviceSettingDb { get; set; } = null!;
    public DbSet<DeviceSocketServerSettingDbEntity> DeviceSocketServerSettingDb { get; set; } = null!;
    public DbSet<DeviceSocketClientSettingDbEntity> DeviceSocketClientSettingDb { get; set; } = null!;
    public DbSet<DeviceSerialSettingDbEntity> DeviceSerialSettingDb { get; set; } = null!;
    public DbSet<DeviceScaleSettingDbEntity> DeviceScaleSettingDb { get; set; } = null!;
    public DbSet<DeviceLabelPrinterSettingDbEntity> DeviceLabelPrinterSettingDb { get; set; } = null!;
    public DbSet<DeviceBarcodeScannerSettingDbEntity> DeviceBarcodeScannerSettingDb { get; set; } = null!;
    public DbSet<SequenceDbEntity> SequenceDb { get; set; } = null!;
    public DbSet<SequenceGroupDbEntity> SequenceGroupDb { get; set; } = null!;
    public DbSet<StationSequenceDbEntity> StationSequenceDb { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }
}
