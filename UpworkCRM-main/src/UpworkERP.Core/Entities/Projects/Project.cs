using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Projects;

/// <summary>
/// Project entity for project management
/// </summary>
public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public decimal Budget { get; set; }
    public string ProjectManager { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
