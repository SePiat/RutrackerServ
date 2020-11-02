using Core;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using TorrServData.Models;

namespace CommService
{
    public class SaveCommentsToDb : ISaveCommentsToDb
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommetsGetter<Comments> _commetsGetter;
        public SaveCommentsToDb(IUnitOfWork unitOfWork, ICommetsGetter<Comments> commetsGetter)
        {
            _unitOfWork = unitOfWork;
            _commetsGetter = commetsGetter;
        }
        public async Task<bool> SaveCommens(TorrentMovie movie)

        {
            Log.Information($"CommentService.SaveCommtnts.SaveComments() start at {DateTime.Now}{Environment.NewLine}");
            //var movies = _unitOfWork.torrentMove.AsQueryable();
            //int counter = 0;
            //int numberCommentsToRenewal = 11;
            try
            {
                /* foreach (var movie in movies)
                 {*/


                var commentsMovie = _commetsGetter.GetComments(movie);
                if (commentsMovie == null || commentsMovie.Count() == movie.amountOfComments)//if anount of comments not change, continue, else remove old comments and add new comments
                {
                    Log.Information($"CommentService.SaveCommtnts.SaveComments().GetComments(movie){Environment.NewLine}" +
                        $"Amount of comments not change or GetComments(movie) equal null{Environment.NewLine}{movie.title}{Environment.NewLine}{movie.movieId} " +
                            $"{Environment.NewLine}{movie.pathDownLoad}  {DateTime.Now}{Environment.NewLine}");

                }
                else
                {
                    /*counter++;
                    if (counter == numberCommentsToRenewal) { break; }*/

                    if (movie.amountOfComments != 0)
                    {
                        try
                        {
                            var oldComments = _unitOfWork.comments.Where(x => x.movieId.Id.ToString().Equals(movie.Id.ToString())).ToList();
                            _unitOfWork.comments.Remove(oldComments);
                        }
                        catch (Exception)
                        {
                            Log.Information($"CommentService.SaveCommtnts.SaveComments()..Remove(oldComments){Environment.NewLine}Comments missing in DB to  {movie.title}{Environment.NewLine}{movie.movieId} " +
                            $"{Environment.NewLine}{movie.pathDownLoad}{DateTime.Now}{Environment.NewLine}");
                        }
                    }

                    _unitOfWork.comments.AddRange(commentsMovie);
                    movie.amountOfComments = commentsMovie.Count();
                    /*}*/
                }
                _unitOfWork.Save();
                Log.Information($"Finish SaveComments() at {DateTime.Now}{Environment.NewLine}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"(CommentService.SaveCommentsToDb.SaveCommens()){Environment.NewLine}Get and Save comment was fail with exception:at {DateTime.Now}{Environment.NewLine}{ex.Message}");
                return  false;
            }

        }
    }
}