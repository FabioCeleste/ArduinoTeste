using Microsoft.EntityFrameworkCore;
using Weather.Models;

namespace Weather.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<WeatherHour> WeatherHours => Set<WeatherHour>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherHour>(entity =>
            {
                entity.HasKey(c => c.SaveHour);
                entity.Property(c => c.Humidity).IsRequired();
                entity.Property(c => c.Temperature).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
