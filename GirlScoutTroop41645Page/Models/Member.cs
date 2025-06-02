using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class Member : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public List<Scout> Scouts { get; set; } = [];
}
