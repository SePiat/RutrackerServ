using Core;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using TorrServ.Models;
using TorrServData.Models;

namespace TorrServ.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaveCommentsToDb _saveCommentsToDb;
        private readonly ISaveMovies _saveMovies;
        private readonly ILemmatization _lemmatization;
        private readonly ICommentHanlder _commentHanlder;


        public HomeController(IUnitOfWork unitOfWork, ISaveCommentsToDb saveCommentsToDb, ISaveMovies saveMovies, ILemmatization lemmatization, ICommentHanlder commentHanlder)
        {
            _unitOfWork = unitOfWork;
            _saveCommentsToDb = saveCommentsToDb;
            _saveMovies = saveMovies;
            _lemmatization = lemmatization;
            _commentHanlder = commentHanlder;

        }

        public ActionResult Index()
        {
            Log.Information($"Load controller Home/index{ Environment.NewLine}");
            var movies = _unitOfWork.torrentMove.Where(x => x.commentIndex > 20).OrderByDescending(x => x.commentIndex).AsQueryable();
            return View(movies);
        }



        public IActionResult SearchFromTitle(string SerchText)
        {
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.AsQueryable();
            movies = movies.Where(s => s.title.Contains(SerchText)).OrderByDescending(x => x.commentIndex).AsQueryable();

            return PartialView("MoviesContainer", movies);
        }

        public IActionResult SearhFromGenre(string GenreText)
        {
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.AsQueryable();
            movies = movies.Where(s => s.genre.Contains(GenreText)).OrderByDescending(x => x.commentIndex).AsQueryable();
            return PartialView("MoviesContainer", movies);
        }

        public IActionResult SearhFromCountry(string CountryText)
        {
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.AsQueryable();
            movies = movies.Where(s => s.country.Contains(CountryText)).OrderByDescending(x => x.commentIndex).AsQueryable();
            return PartialView("MoviesContainer", movies);
        }

        public IActionResult SearhFromEarOfIssue(string EarOfIssueText)
        {
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.AsQueryable();
            movies = movies.Where(s => s.earOfIssue.Contains(EarOfIssueText)).OrderByDescending(x => x.commentIndex).AsQueryable();
            return PartialView("MoviesContainer", movies);
        }

        public IActionResult SearhFromVideoQuality(string VideoQualityText)
        {
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.AsQueryable();
            movies = movies.Where(s => s.videoQuality.Contains(VideoQualityText)).OrderByDescending(x => x.commentIndex).AsQueryable();
            return PartialView("MoviesContainer", movies);
        }

        public IActionResult SearhFromSize(string SizeText)
        {
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.AsQueryable();
            movies = movies.Where(s => s.size.Contains(SizeText)).OrderByDescending(x => x.commentIndex).AsQueryable();
            return PartialView("MoviesContainer", movies);
        }

        public IActionResult SearhFromIndex(string indexText)
        {
            int index = 0;
            if (indexText != null)
            {
                try
                {
                    index = Convert.ToInt32(indexText);
                }
                catch (Exception)
                {
                }
            }
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.Where(x => x.commentIndex > index).OrderByDescending(x => x.commentIndex).AsQueryable();
            return PartialView("MoviesContainer", movies);
        }

        public IActionResult SearhFromDownloads(string DownloadsText)
        {
            int DownloadsInput = Convert.ToInt32(DownloadsText);
            var movies = _unitOfWork.torrentMove.Where(x => x.downloads > DownloadsInput).OrderByDescending(x => x.downloads).AsQueryable();
            return PartialView("MoviesContainer", movies);

        }

        public IActionResult SearhFromPathDowLoad(string PathDowLoadText)
        {
            IQueryable<TorrentMovie> movies = null;
            movies = _unitOfWork.torrentMove.AsQueryable();
            movies = movies.Where(s => s.pathDownLoad.Contains(PathDowLoadText)).OrderByDescending(x => x.commentIndex).AsQueryable();
            return PartialView("MoviesContainer", movies);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
