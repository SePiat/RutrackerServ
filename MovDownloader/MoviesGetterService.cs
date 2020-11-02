using Core;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TorrServData.Models;

namespace MovDownloader
{
    public class MoviesGetterService : IMoviesGetterService<TorrentMovie>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaveCommentsToDb _saveCommentsToDb;
        private readonly ICommentHanlder _commentHanlder;


        private List<TorrentMovie> torrentMoves = new List<TorrentMovie>();

        public MoviesGetterService(IUnitOfWork unitOfWork, ISaveCommentsToDb saveCommentsToDb, ICommentHanlder commentHanlder)
        {
            _unitOfWork = unitOfWork;
            _saveCommentsToDb = saveCommentsToDb;
            _commentHanlder = commentHanlder;
        }




        /* //альтернативное решение проблемы с кодировкой
            WebClient wc = new WebClient();
            string str = wc.DownloadString(url);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(str);*/
        public IEnumerable<TorrentMovie> GetMovies()
        {
            Log.Information($"MoviesDownloader.MoviesGetterService.GetMovies() start  at {DateTime.Now}{Environment.NewLine}");            
            var urls = GetPaths();
            foreach (var url in urls)
            {
                var nodes = GetHtmlNodesTagTr(url);

                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {                        
                        var NodeId = node.Id.ToString();
                        if (NodeId == "tr-5836841")//ignore post"Открылся новый сайт новинок"
                        {
                            continue;
                        }
                        var SubPathDownLoad = "";
                        try
                        {
                            SubPathDownLoad = "https://rutracker.org/forum/"
                                + node.SelectSingleNode(".//a[@class='torTopic bold tt-text']").Attributes["href"].Value;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"MoviesDownloader.MoviesGetterService.GetMovies(){Environment.NewLine}" +
                                $"Get path of movies form url was fail with exception:{Environment.NewLine}{ex.Message}" +
                             $"{Environment.NewLine}url={url}{Environment.NewLine}NodeId={NodeId}");
                            continue;
                        }
                        Log.Information($"MoviesDownloader.MoviesGetterService.GetMovies(){Environment.NewLine}" +
                            $"Parent's page address{Environment.NewLine}{url}{Environment.NewLine}" +
                            $"Address is being processed{Environment.NewLine}{SubPathDownLoad}{Environment.NewLine}" +
                            $"{DateTime.Now}{Environment.NewLine}");

                        
                        var processedMovie = _unitOfWork.torrentMove.AsQueryable().FirstOrDefault(p => p.pathDownLoad.Equals(SubPathDownLoad));//Check to be movie in DB
                        if (processedMovie != null)//if movie exist in the DB
                        {
                            
                            var lastPost = node.SelectSingleNode(".//td[@class='vf-col-last-post tCenter small nowrap']").SelectSingleNode(".//p").InnerText;
                                                       
                            if (processedMovie.lastPost!= lastPost||processedMovie.amountOfComments==0||processedMovie.commentIndex>2000)
                            {
                                processedMovie.lastPost = lastPost;
                            _saveCommentsToDb.SaveCommens(processedMovie);
                            _commentHanlder.GetCommentIndex(processedMovie);
                            }
                            continue;
                        }
                        else
                        {
                            try
                            {
                                var textFromTegA = node.SelectSingleNode(".//a[@class='torTopic bold tt-text']").InnerText.ToString();
                                
                                var additInformFromTitle = Regex.Match(textFromTegA, @"(\[.+\])").ToString();

                                var _title = Regex.Replace(textFromTegA, @"(\[.+\].+)", "");//delete of addition information
                                var _genre = Regex.Replace(Regex.Match(additInformFromTitle, @"\s[а-я].+,").ToString(), @"(,$)", "");
                                var _country = Regex.Match(additInformFromTitle, @"[А-Я]{1,3}[a-я]+").ToString();
                                var _earOfIssue = Regex.Match(additInformFromTitle, @"\d{4}").ToString();
                                var _videoQuality = Regex.Replace(Regex.Match(additInformFromTitle, @"[A-Z]+.+]").ToString(), "]", "");
                                var _size = Regex.Replace(node.SelectSingleNode(".//a[@class='small f-dl dl-stub']").InnerText.ToString(), @"&nbsp;", "");
                                var _commentIndex = 0;
                                string _strDownloads = Regex.Replace(node.SelectSingleNode(".//p[@title='Торрент скачан']").InnerText, @"\D+", "").ToString();
                                int _downloads = Convert.ToInt32(_strDownloads);
                                var _pathDownLoad = "https://rutracker.org/forum/" + node.SelectSingleNode(".//a[@class='torTopic bold tt-text']").Attributes["href"].Value;
                                var _lastPost = node.SelectSingleNode(".//td[@class='vf-col-last-post tCenter small nowrap']").Element("p").InnerText;
                                if (_genre != null)
                                {
                                    torrentMoves.Add(new TorrentMovie
                                    {
                                        title = _title,
                                        genre = _genre,
                                        country = _country,
                                        earOfIssue = _earOfIssue,
                                        videoQuality = _videoQuality,
                                        size = _size,
                                        commentIndex = _commentIndex,
                                        downloads = _downloads,
                                        pathDownLoad = _pathDownLoad,
                                        movieId = NodeId,
                                        lastPost= _lastPost
                                    });
                                }

                            }
                            catch (Exception ex)
                            {
                                Log.Information($"MoviesDownloader.MoviesGetterService.GetMovies(){Environment.NewLine}Error reading html tags or/and creating new TorrentMovie object :{Environment.NewLine}{ex.Message}" +
                                    $"{Environment.NewLine}url={url}{Environment.NewLine}SubPath={SubPathDownLoad}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}");

                            }
                        }
                    }
                }
            }
            Log.Information($"MoviesDownloader.MoviesGetterService.GetMovies() finished  at {DateTime.Now}{Environment.NewLine}");            
            return torrentMoves;

        }

        private HtmlNodeCollection GetHtmlNodesTagTr(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//solves the problem with the exception of encoding Windows-1251
            HtmlNodeCollection nodesTr = null;
            var web = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("Windows-1251")//coding from head html
            };
            try
            {
                var htmlDocument = web.Load(path);
                nodesTr = htmlDocument.DocumentNode.SelectNodes("//tr[@class='hl-tr']");
            }
            catch (Exception ex)
            {
                Log.Error($"Error load html docement from {path}  :{Environment.NewLine}{ex.Message}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}");
            }

            return nodesTr;
        }

        private HtmlNodeCollection GetHtmlNodesTagA(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//solves the problem with the exception of encoding Windows-1251

            var web = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("Windows-1251")//coding from head html
            };
            var htmlDocument = web.Load(path);
            var nodesTagA = htmlDocument.DocumentNode.SelectNodes("//a[@class='pg']");
            return nodesTagA;
        }


        private List<string> GetPaths()
        {
            List<string> paths = new List<string>();
            var sourcePaths = _unitOfWork.sourceOfMovies.ToList();//Getting main paths from DB (main pages)

            foreach (var pathPage in sourcePaths)
            {
                paths.Add(pathPage.path);
                var nodesA = GetHtmlNodesTagA(pathPage.path);//Getting paths from panel of navugation
                if (nodesA != null)
                {
                    int amountPages = Convert.ToInt32(nodesA[nodesA.Count() - 2].InnerText);//defining the count pages from tag "a"(panel of navugation)
                    if (amountPages != 0)
                    {
                        for (int i = 50; i < amountPages * 50; i += 50)
                        {
                            var subPath = pathPage.path + "&start=" + i;
                            paths.Add(subPath);
                        }
                    }
                }
            }
            return paths;
        }
    }
}
