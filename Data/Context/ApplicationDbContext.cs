using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TorrServData.Models;


namespace TorrServData.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TorrentMovie> TorrentMoves { get; set; }
        public DbSet<SourceOfMovies> SourceOfMovies { get; set; }
        public DbSet<Comments> Comments { get; set; }


    }
}
