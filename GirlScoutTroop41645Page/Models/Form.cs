using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class Form
{
    [Key]
    public int FormId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? DueDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public List<FormField> FormFields { get; set; } = new List<FormField>();
    
    public List<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>();
}