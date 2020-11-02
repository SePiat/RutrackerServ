using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace TorrServ.Controllers
{


    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaveMovies _saveMovies;
        private readonly ICommentHanlder _commentHanlder;

        public AdminController(IUnitOfWork unitOfWork, ISaveMovies saveMovies, ICommentHanlder commentHanlder)
        {
            _unitOfWork = unitOfWork;
            _saveMovies = saveMovies;
            _commentHanlder = commentHanlder;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index() => View();

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateDatabase()
        {
            await Task.Run(() => _saveMovies.SaveMov());
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCommIndex()
        {
            var movies = _unitOfWork.torrentMove.Where(x => x.commentIndex == 1000).ToList();
            await Task.Run(() =>
            {
                foreach (var movie in movies)
                {
                    _commentHanlder.GetCommentIndex(movie);
                }
            });
            return RedirectToAction("Index");
        }


    }
}
