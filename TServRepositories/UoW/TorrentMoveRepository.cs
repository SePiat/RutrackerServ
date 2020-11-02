using TorrServData.Data;
using TorrServData.Models;

namespace TServRepositories.UoW
{
    public class TorrentMoveRepository : Repository<TorrentMovie>
    {
        public TorrentMoveRepository(ApplicationDbContext _context) : base(_context)
        {
        }
    }
}
