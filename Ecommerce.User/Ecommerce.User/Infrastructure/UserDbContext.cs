using Microsoft.EntityFrameworkCore;

namespace ecommerce_user.Infrastructure;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) {}

    public DbSet<Entities.User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<Entities.User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Entities.User>()
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
    }
}