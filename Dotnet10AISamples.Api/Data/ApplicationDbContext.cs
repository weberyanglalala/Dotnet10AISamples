using Dotnet10AISamples.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet10AISamples.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.Property(u => u.UpdatedAt)
                .IsRequired();

            // Create unique indexes
            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasIndex(u => u.IsActive);
        });

        // Configure Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(r => r.Description)
                .HasMaxLength(200);

            entity.Property(r => r.CreatedAt)
                .IsRequired();

            entity.Property(r => r.UpdatedAt)
                .IsRequired();

            // Create unique index on Name
            entity.HasIndex(r => r.Name)
                .IsUnique();
        });

        // Configure UserRole entity (junction table)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.Property(ur => ur.UserId)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(ur => ur.RoleId)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(ur => ur.AssignedAt)
                .IsRequired();

            entity.Property(ur => ur.AssignedBy)
                .HasMaxLength(40);

            // Configure relationships
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.AssignedByUser)
                .WithMany()
                .HasForeignKey(ur => ur.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes
            entity.HasIndex(ur => ur.UserId);
            entity.HasIndex(ur => ur.RoleId);
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();
        });
    }
}