using System;
using System.Threading.Tasks;
using TorrServData.Models;

namespace Core
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TorrentMovie> torrentMove { get; }
        IGenericRepository<SourceOfMovies> sourceOfMovies { get; }
        IGenericRepository<Comments> comments { get; }
        void Save();
        Task SaveAsync();
    }

}
