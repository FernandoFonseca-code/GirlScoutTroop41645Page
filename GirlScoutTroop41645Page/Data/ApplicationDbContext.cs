using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GirlScoutTroop41645Page.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Scout> Scouts { get; set; }
    public DbSet<Member> Members { get; set; }
}
