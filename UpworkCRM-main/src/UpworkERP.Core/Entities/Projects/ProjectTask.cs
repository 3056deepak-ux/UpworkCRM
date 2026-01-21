using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Enums;

namespace UpworkERP.Core.Entities.Projects;

/// <summary>
/// Task entity for project task tracking
/// </summary>
public class ProjectTask : BaseEntity
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.ToDo;
    public Priority Priority { get; set; } = Priority.Medium;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int EstimatedHours { get; set; }
    
    // Navigation property
    public Project Project { get; set; } = null!;
}
