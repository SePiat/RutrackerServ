using System.Collections.Generic;
using TorrServData.Models;

namespace Core
{
    public interface ICommetsGetter<T> where T : class
    {
        IEnumerable<T> GetComments(TorrentMovie movie);
    }

}
