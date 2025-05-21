using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace GirlScoutTroop41645Page.Data;

public class ApplicationDbContext : IdentityDbContext<Member>
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Scout> Scouts { get; set; }
    public DbSet<Member> Members { get; set; }

}
