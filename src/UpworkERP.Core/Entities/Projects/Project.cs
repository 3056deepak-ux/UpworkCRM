using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UpworkERP.Core.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Projects;

/// <summary>
/// Project entity
/// </summary>
public class Project : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ProjectManager { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Budget { get; set; }

    [Required]
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
}
