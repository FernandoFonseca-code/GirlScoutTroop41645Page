using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public enum FormFieldType
{
    Text,
    TextArea,
    Number,
    Email,
    Date,
    Checkbox,
    Radio,
    Dropdown
}

public class FormField
{
    [Key]
    public int FormFieldId { get; set; }
    
    [Required]
    public int FormId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Label { get; set; } = string.Empty;
    
    [Required]
    public FormFieldType FieldType { get; set; }
    
    public bool IsRequired { get; set; } = false;
    
    public int DisplayOrder { get; set; }
    
    [StringLength(500)]
    public string? HelpText { get; set; }
    
    [StringLength(1000)]
    public string? Options { get; set; } // JSON string for dropdown/radio options
    
    public Form Form { get; set; } = null!;
    
    public List<FormFieldResponse> FormFieldResponses { get; set; } = new List<FormFieldResponse>();
}