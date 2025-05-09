using Microsoft.EntityFrameworkCore;
using MvcLab9.Models;

namespace MvcLab9.ApplicationDbContext {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Data> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasIndex(u => u.login).IsUnique();
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, login = "admin", password = "21232f297a57a5a743894a0e4a801fc3" }
            );
        }
    }
}