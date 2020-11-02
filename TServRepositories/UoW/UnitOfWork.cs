using Core;
using System;
using System.Threading.Tasks;
using TorrServData.Data;
using TorrServData.Models;

namespace TServRepositories.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IGenericRepository<TorrentMovie> _torrentMoveRepository;
        private readonly IGenericRepository<SourceOfMovies> _sourceOfMoviesRepository;
        private readonly IGenericRepository<Comments> _commentsRopository;

        public IGenericRepository<TorrentMovie> torrentMove => _torrentMoveRepository;
        public IGenericRepository<SourceOfMovies> sourceOfMovies => _sourceOfMoviesRepository;
        public IGenericRepository<Comments> comments => _commentsRopository;

        private bool disposed = false;

        public UnitOfWork(ApplicationDbContext context,
            IGenericRepository<TorrentMovie> torrentMoveRepository,
            IGenericRepository<SourceOfMovies> sourceOfMoviesRepository,
            IGenericRepository<Comments> commentsRopository
            )
        {
            _context = context;
            _torrentMoveRepository = torrentMoveRepository;
            _sourceOfMoviesRepository = sourceOfMoviesRepository;
            _commentsRopository = commentsRopository;
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();

        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
