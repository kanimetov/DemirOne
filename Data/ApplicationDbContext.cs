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
        modelBuilder.Entity<User>()
        .HasOne(u => u.Balance)
        .WithOne(u => u.User)
        .HasForeignKey<Balance>(u => u.UserId);

        modelBuilder.Entity<Balance>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<Balance>()
        .Property(u => u.Amount)
        .HasDefaultValue(8);


        modelBuilder.Entity<Transaction>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<Transaction>()
        .HasOne(u => u.Balance)
        .WithMany(u => u.Transactions)
        .HasForeignKey(u => u.BalanceId);


        modelBuilder.Entity<Token>()
        .Property(u => u.Id)
        .ValueGeneratedOnAdd();
        modelBuilder.Entity<Token>()
        .HasOne(u => u.User)
        .WithMany(u => u.Tokens);

        base.OnModelCreating(modelBuilder);
    }
}