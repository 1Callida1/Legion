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

    public DbSet<User> Users { get; set; }
    public DbSet<Investor> Investors { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractStatus> ContractStatuses { get; set; }
    public DbSet<ContractType> ContractTypes { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<RenewalContract> RenewalContracts { get; set; }
    public DbSet<Repeat> Repeats { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
}
