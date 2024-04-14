using Legion.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Navigation
        modelBuilder.Entity<Contract>().Navigation(c => c.Investor).AutoInclude();
        modelBuilder.Entity<Contract>().Navigation(c => c.Manager).AutoInclude();
        modelBuilder.Entity<Contract>().Navigation(c => c.ContractType).AutoInclude();
        modelBuilder.Entity<Contract>().Navigation(c => c.Status).AutoInclude();
        modelBuilder.Entity<Contract>().Navigation(c => c.Referral).AutoInclude();

        modelBuilder.Entity<Referral>().Navigation(c => c.InvestorCalled).AutoInclude();
        modelBuilder.Entity<Referral>().Navigation(c => c.InvestorInvited).AutoInclude();
        ;
        modelBuilder.Entity<AdditionalPayment>().Navigation(c => c.Contract).AutoInclude();
    }



    public DbSet<User> Users { get; set; }
    public DbSet<Investor> Investors { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractStatus> ContractStatuses { get; set; }
    public DbSet<ContractType> ContractTypes { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<AdditionalPayment> AdditionalPayments { get; set; }
    public DbSet<Repeat> Repeats { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
}
