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
    public DbSet<Form> Forms { get; set; }
    public DbSet<FormField> FormFields { get; set; }
    public DbSet<FormSubmission> FormSubmissions { get; set; }
    public DbSet<FormFieldResponse> FormFieldResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Form relationships
        modelBuilder.Entity<FormField>()
            .HasOne(ff => ff.Form)
            .WithMany(f => f.FormFields)
            .HasForeignKey(ff => ff.FormId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormSubmission>()
            .HasOne(fs => fs.Form)
            .WithMany(f => f.FormSubmissions)
            .HasForeignKey(fs => fs.FormId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormSubmission>()
            .HasOne(fs => fs.SubmittedByMember)
            .WithMany()
            .HasForeignKey(fs => fs.SubmittedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormFieldResponse>()
            .HasOne(ffr => ffr.FormSubmission)
            .WithMany(fs => fs.FormFieldResponses)
            .HasForeignKey(ffr => ffr.FormSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormFieldResponse>()
            .HasOne(ffr => ffr.FormField)
            .WithMany(ff => ff.FormFieldResponses)
            .HasForeignKey(ffr => ffr.FormFieldId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
