using Microsoft.EntityFrameworkCore;

namespace Cloudweather.Temperature.DataAccess
{
    public class TempDbContext:DbContext
    {
        public TempDbContext()
        {

        }
        public TempDbContext(DbContextOptions opts) : base(opts)
        {

        }
        public DbSet<Temperature> Temps { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }
        private void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>(b => { b.ToTable("temperature"); });
        }
    }
}
