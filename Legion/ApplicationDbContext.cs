using Legion.Model;
using Legion.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Investor> Investors { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractStatus> ContractStatuses { get; set; }
    public DbSet<ContractType> ContractTypes { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<RenewalContract> RenewalContracts { get; set; }
    public DbSet<Repeated> Repeateds { get; set; }
}
