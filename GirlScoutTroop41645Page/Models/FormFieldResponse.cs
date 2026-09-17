using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class FormFieldResponse
{
    [Key]
    public int FormFieldResponseId { get; set; }
    
    [Required]
    public int FormSubmissionId { get; set; }
    
    [Required]
    public int FormFieldId { get; set; }
    
    [StringLength(2000)]
    public string? Response { get; set; }
    
    public FormSubmission FormSubmission { get; set; } = null!;
    
    public FormField FormField { get; set; } = null!;
}