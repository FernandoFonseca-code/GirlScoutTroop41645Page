using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public enum TroopLevel
{
    [Display(Name = "Daisy")]
    Daisy,
    
    [Display(Name = "Brownie")]
    Brownie,
    
    [Display(Name = "Junior")]
    Junior,
    
    [Display(Name = "Cadette")]
    Cadette,
    
    [Display(Name = "Senior")]
    Senior,
    
    [Display(Name = "Ambassador")]
    Ambassador
}