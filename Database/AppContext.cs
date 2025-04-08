using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PostsWebApi.Models;

namespace PostsWebApi.Database
{
    public class AppContext:DbContext
    {


        public AppContext(DbContextOptions<AppContext> options) : base(options) { }


        public DbSet<Users> Users { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Follows> Followers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Follows>()
                .HasKey(f => new { f.SeguidorId, f.SeguidoId });

            
            modelBuilder.Entity<Follows>()
                .HasOne(f => f.Seguidor)
                .WithMany()
                .HasForeignKey(f => f.SeguidorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follows>()
                .HasOne(f => f.Seguido)
                .WithMany()
                .HasForeignKey(f => f.SeguidoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
