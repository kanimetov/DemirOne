using Demir.Data.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Balance> Balances { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Token> Tokens { get; set; }


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
        modelBuilder.Entity<Balance>()
        .HasOne(u => u.User)
        .WithOne(u => u.Balance);


        modelBuilder.Entity<Transaction>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Token>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<Token>()
        .HasOne(u => u.User)
        .WithMany(u => u.Tokens);

        base.OnModelCreating(modelBuilder);
    }
}