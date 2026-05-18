using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortisFramework.Entity;

[Table("StationDevice")]
[PrimaryKey("StationName", "DeviceName")]
public class StationDeviceDbEntity
{
    [Key] [Required] [StringLength(100)]
    public string StationName { get; set; } = null!;

    [Key] [Required] [StringLength(100)]
    public string DeviceName { get; set; } = null!;

    [Required]
    public short DeviceId { get; set; }

    [NotMapped]
    public DeviceSettingDbEntity? DeviceSetting { get; set; }
}
