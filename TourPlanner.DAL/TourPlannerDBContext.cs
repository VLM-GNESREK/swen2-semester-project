using Microsoft.EntityFrameworkCore;

namespace TourPlanner.DAL
{
    public class TourPlannerDBContext : DbContext
    {
        public TourPlannerDBContext(DbContextOptions<TourPlannerDBContext> options) : base(options) {}

        public DbSet<Entities.Tour> Tours { get; set; }
        public DbSet<Entities.TourLog> TourLogs { get; set; }
        public DbSet<Entities.User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Tour>()
                .HasMany(t => t.TourLogs)
                .WithOne(tl => tl.Tour)
                .HasForeignKey(tl => tl.tour_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Entities.User>()
                .HasMany(u => u.Tours)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.user_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}