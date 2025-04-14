using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class Scout
{
    [Key]
    public int ScoutId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<Member> Members { get; set; } = [];
}
