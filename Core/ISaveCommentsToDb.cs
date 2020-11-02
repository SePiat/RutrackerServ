using System.Threading.Tasks;
using TorrServData.Models;

namespace Core
{
    public interface ISaveCommentsToDb
    {
        Task<bool> SaveCommens(TorrentMovie movie);
    }

}
