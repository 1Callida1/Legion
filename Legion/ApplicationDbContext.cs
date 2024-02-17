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
}
