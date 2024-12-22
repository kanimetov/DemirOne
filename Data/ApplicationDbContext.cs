using Demir.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Balance> Balances { get; set; }
    public DbSet<Transaction> Transactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Balance>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<Balance>()
        .Property(u => u.Amount)
        .HasDefaultValue(8);

        modelBuilder.Entity<Transaction>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }
}