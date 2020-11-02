using Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TorrServData.Models;

namespace CommService
{
    public class CommentHanlder : ICommentHanlder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILemmatization _lemmatization;
        private readonly IAFINNService _afinn;

        public CommentHanlder(IUnitOfWork unitOfWork, ILemmatization lemmatization, IAFINNService afinn)
        {
            _unitOfWork = unitOfWork;
            _lemmatization = lemmatization;
            _afinn = afinn;
        }

        public void GetCommentIndex(TorrentMovie movie)
        {
            Log.Information($"CommentService.CommentHanlder.GetCommentIndex() started at {DateTime.Now}{ Environment.NewLine}");
            System.Diagnostics.Debug.WriteLine($"CommentService.CommentHanlder.GetCommentIndex() started at {DateTime.Now}{ Environment.NewLine}");

            try
            {
                Dictionary<string, int> dictFromLemms = new Dictionary<string, int>();
                Dictionary<string, string> DictionaryAffin = _afinn.GetDictionary();
                int countAffinWordInText = 0;
                int IndexWordsFromText = 0;
                int CommentIndex = 0;
                var comments = _unitOfWork.comments.Where(x => x.movieId.Id.ToString().Equals(movie.Id.ToString()));
                if (comments != null)
                {                   
                    string commentsString = Regex.Replace(BuildSringFromComments(comments), @"\s{2}", " ");
                    System.Diagnostics.Debug.WriteLine($"CommentService.CommentHanlder.GetCommentIndex() work with movie  {movie.title}{ Environment.NewLine}{movie.Id}");
                    Log.Information($"CommentService.CommentHanlder.GetCommentIndex() work with movie  {movie.title}{ Environment.NewLine}{movie.Id}");

                    dictFromLemms = _lemmatization.GetDictFromLemms(commentsString);
                    if (dictFromLemms != null)
                    {
                        foreach (var wordFromText in dictFromLemms)
                        {
                            if (DictionaryAffin.ContainsKey(wordFromText.Key))
                            {
                                int AffinWordValue = Convert.ToInt32(DictionaryAffin[wordFromText.Key]) + 5;//remove negative values
                                IndexWordsFromText += AffinWordValue * wordFromText.Value;
                                countAffinWordInText += wordFromText.Value;
                            }
                        }
                        if (countAffinWordInText != 0)
                        {
                                                       
                            CommentIndex = Convert.ToInt32(Math.Round((double)IndexWordsFromText / countAffinWordInText, 3) * 10)+movie.amountOfComments;
                            movie.commentIndex = CommentIndex;

                        }
                        else
                        {                            
                            movie.commentIndex = movie.amountOfComments;
                        }
                    }
                    else
                    {                      
                        movie.commentIndex = movie.amountOfComments;
                    }
                }
                else
                {
                    Log.Information($"CommentService.CommentHanlder.GetCommentIndex() finish at {DateTime.Now}{ Environment.NewLine}" +
                        $"DB not contain comments by movie {movie.title}{ Environment.NewLine} ");
                    System.Diagnostics.Debug.WriteLine($"CommentService.CommentHanlder.GetCommentIndex() finish at {DateTime.Now}{ Environment.NewLine}" +
                        $"DB not contain comments by movie {movie.title}{ Environment.NewLine} ");                    
                    movie.commentIndex = 0;
                }

                _unitOfWork.Save();
                Log.Information($"CommentService.CommentHanlder.GetCommentIndex() finish at {DateTime.Now}{ Environment.NewLine}");
                System.Diagnostics.Debug.WriteLine($"CommentService.CommentHanlder.GetCommentIndex() finish at {DateTime.Now}{ Environment.NewLine}");

            }
            catch (Exception ex)
            {
                Log.Error($"CommentService.CommentHanlder.GetCommentIndex() worked with error {DateTime.Now}" +
                    $"{Environment.NewLine}{ex}{Environment.NewLine}");
            }
        }

        private string BuildSringFromComments(IEnumerable<Comments> comments)
        {
            string commentsString = "";
            foreach (var comment in comments)
            {
                commentsString += comment.commentText;
            }
            return commentsString;
        }
    }
}
