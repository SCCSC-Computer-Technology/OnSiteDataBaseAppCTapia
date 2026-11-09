using Microsoft.EntityFrameworkCore;
using OnSiteApp.Models;

namespace OnSiteApp.Data;

public class OnSiteDatabase : DbContext
{
    
    public OnSiteDatabase(DbContextOptions<OnSiteDatabase> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }
    public DbSet<Technician> Technicians { get; set; }
    public DbSet<AdminUser> Admins { get; set; }

    // configures relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Job>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(job => job.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Job>()
            .Ignore(job => job.DoneCount)
            .Ignore(job => job.Total)
            .Ignore(job => job.IsLate)
            .Ignore(job => job.StatusText);

        modelBuilder.Entity<ChecklistItem>()
            .HasOne<Job>()
            .WithMany(job => job.Checklist)
            .HasForeignKey(item => item.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}