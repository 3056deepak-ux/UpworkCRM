using UpworkERP.Core.Entities.Common;

namespace UpworkERP.Core.Entities.Projects;

/// <summary>
/// Time entry entity for time tracking
/// </summary>
public class TimeEntry : BaseEntity
{
    public int ProjectId { get; set; }
    public int? TaskId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Hours { get; set; }
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public Project Project { get; set; } = null!;
    public ProjectTask? Task { get; set; }
}
