
using TorrServData.Data;
using TorrServData.Models;

namespace TServRepositories.UoW
{
    public class CommentsRepository : Repository<Comments>
    {
        public CommentsRepository(ApplicationDbContext _context) : base(_context)
        {
        }
    }
}