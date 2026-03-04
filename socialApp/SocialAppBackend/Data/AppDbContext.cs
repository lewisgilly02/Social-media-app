using Microsoft.EntityFrameworkCore;
using SocialAppBackend.Models;


namespace SocialAppBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Like>()
            .HasKey(l =>  new {l.UserId, l.PostId});
    }

    public DbSet<Post> Posts => Set<Post>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Like> Likes => Set<Like>();
}