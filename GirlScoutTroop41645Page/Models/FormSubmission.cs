using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class FormSubmission
{
    [Key]
    public int FormSubmissionId { get; set; }
    
    [Required]
    public int FormId { get; set; }
    
    [Required]
    public string SubmittedBy { get; set; } = string.Empty;
    
    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    
    public Form Form { get; set; } = null!;
    
    public Member SubmittedByMember { get; set; } = null!;
    
    public List<FormFieldResponse> FormFieldResponses { get; set; } = new List<FormFieldResponse>();
}