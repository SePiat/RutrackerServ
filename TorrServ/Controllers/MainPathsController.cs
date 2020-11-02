using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TorrServData.Models;

namespace TorrServ.Controllers
{
    public class MainPathsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainPathsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            var sourcePaths = _unitOfWork.sourceOfMovies.ToList();

            return View(sourcePaths);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create() => View();
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(SourceOfMovies model)
        {
            if (model != null)
            {
                SourceOfMovies newPath = new SourceOfMovies { path = model.path, Id = model.Id };
                _unitOfWork.sourceOfMovies.Add(newPath);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }
            else
                return View(model);
        }

        public IActionResult Edit(Guid Id)
        {
            SourceOfMovies path = _unitOfWork.sourceOfMovies.GetById(Id);
            if (path == null)
            {
                return NotFound();
            }

            SourceOfMovies model = new SourceOfMovies { Id = path.Id, path = path.path };
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(SourceOfMovies model)
        {
            if (ModelState.IsValid)
            {
                SourceOfMovies editPath = _unitOfWork.sourceOfMovies.GetById(model.Id);
                if (editPath != null)
                {
                    editPath.path = model.path;
                    editPath.Id = model.Id;
                    await _unitOfWork.SaveAsync();
                    return RedirectToAction("Index");
                }
                else return NotFound();
            }
            else return View(model);



        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            SourceOfMovies path = _unitOfWork.sourceOfMovies.GetById(Id);
            if (path != null)
            {
                _unitOfWork.sourceOfMovies.Delete(Id);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

    }
}