using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("Device")]
[PrimaryKey("DeviceName")]
public class DeviceSettingDbEntity
{
    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required] [StringLength(int.MaxValue)]
    public string Description { get; set; } = null!;

    [Required]
    [StringLength(int.MaxValue)]
    public string SerialNumber { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Type { get; set; } = null!;

    [Required] [StringLength(100)]
    public string Communication { get; set; } = null!;

    [Required]
    public bool RxLogEnable { get; set; } = false;

    [Required]
    public bool TxLogEnable { get; set; } = false;

    [Required]
    public bool AutoConnect { get; set; } = false;

    [NotMapped]
    internal DeviceSocketServerSettingDbEntity? SocketServerSetting { get; set; }

    [NotMapped]
    internal DeviceSocketClientSettingDbEntity? SocketClientSetting { get; set; }

    [NotMapped]
    internal DeviceSerialSettingDbEntity? SerialSetting { get; set; }

    [NotMapped]
    internal DeviceScaleSettingDbEntity? ScaleSetting { get; set; }

    [NotMapped]
    internal DeviceLabelPrinterSettingDbEntity? LabelPrinterSetting { get; set; }

    [NotMapped]
    internal DeviceBarcodeScannerSettingDbEntity? BarcodeScannerSetting { get; set; }
}
