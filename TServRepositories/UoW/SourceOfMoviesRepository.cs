
using TorrServData.Data;
using TorrServData.Models;

namespace TServRepositories.UoW
{
    public class SourceOfMoviesRepository : Repository<SourceOfMovies>
    {
        public SourceOfMoviesRepository(ApplicationDbContext _context) : base(_context)
        {
        }
    }
}


