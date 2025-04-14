using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GirlScoutTroop41645Page.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GirlScoutTroop41645Page.Models.Scout> Scouts { get; set; } = default!;
    public DbSet<GirlScoutTroop41645Page.Models.Member> Members { get; set; } = default!;
}
