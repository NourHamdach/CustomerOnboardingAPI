using CustomerOnboarding.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerOnboarding.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserSecurity> UserSecurity { get; set; }
        public DbSet<OTPAttempt> OTPAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UNIQUE constraints for fully registered users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.ICNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.EmailAddress)
                .IsUnique();

            // ONE-TO-ONE: User (1) ↔ UserSecurity (1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Security)
                .WithOne(s => s.User)
                .HasForeignKey<UserSecurity>(s => s.UserId);

            // MANY-TO-ONE: OTPAttempt (N) → User (1)
            modelBuilder.Entity<OTPAttempt>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
