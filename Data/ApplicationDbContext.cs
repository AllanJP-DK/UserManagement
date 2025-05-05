using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccessRight> AccessRights { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleAccessRight> RoleAccessRights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite keys for many-to-many relationships
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RoleAccessRight>()
                .HasKey(ra => new { ra.RoleId, ra.AccessRightId });

            // Configure relationships for UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Configure relationships for RoleAccessRight
            modelBuilder.Entity<RoleAccessRight>()
                .HasOne(ra => ra.Role)
                .WithMany(r => r.RoleAccessRights)
                .HasForeignKey(ra => ra.RoleId);

            modelBuilder.Entity<RoleAccessRight>()
                .HasOne(ra => ra.AccessRight)
                .WithMany(a => a.RoleAccessRights)
                .HasForeignKey(ra => ra.AccessRightId);

            // Configure Address to User relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Address)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}