using Core;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TorrServData.Models;

namespace CommService
{
    public class CommetsGetter : ICommetsGetter<Comments>
    {

        public IEnumerable<Comments> GetComments(TorrentMovie movie)
        {

            Log.Information($"GetComments to  {movie.title}{Environment.NewLine}{movie.movieId} " +
                $"{Environment.NewLine}{movie.pathDownLoad} started{DateTime.Now}{Environment.NewLine}");

            IEnumerable<string> TextCommFromHtmlNodes = new List<string>();
            List<Comments> comments = new List<Comments>();

            var urls = GetPaths(movie.pathDownLoad);//Getting paths from panel of navugation

            foreach (var url in urls)
            {
                try
                {
                    TextCommFromHtmlNodes = GetTextCommFromHtmlNodes(url);
                }
                catch (Exception ex)
                {
                    Log.Error($"Get nodes form {url} was fail with exception:{Environment.NewLine}{ex.Message}");
                    return null;

                }

                foreach (var coment in TextCommFromHtmlNodes)
                {
                    comments.Add(new Comments()
                    {
                        commentIndex = 0,
                        commentText = coment,
                        movieId = movie
                    });

                }
            }

            return comments;
        }

        private List<string> GetPaths(string path)//Getting paths from panel of navugation
        {
            List<string> paths = new List<string>();
            HtmlDocument htmlDoc;
            paths.Add(path);
            try
            {
                htmlDoc = GetHtmlDocument(path);
            }
            catch (Exception ex)
            {
                Log.Error($"A method GetHtmlDocument() with {path} in method GetPaths() was fail with exception:{Environment.NewLine}{ex.Message}");
                return null;
            }

            var nodesA = htmlDoc.DocumentNode.SelectNodes("//a[@class='pg']");

            if (nodesA != null)
            {
                int amountPages = Convert.ToInt32(nodesA[nodesA.Count() - 2].InnerText);//defining the count pages from tag "a"(panel of navugation)
                if (amountPages != 0)
                {
                    for (int i = 30; i < amountPages * 30; i += 30)
                    {
                        var subPath = path + "&start=" + i;
                        paths.Add(subPath);
                    }
                }
            }

            return paths;
        }




        IEnumerable<string> GetTextCommFromHtmlNodes(string path)
        {

            var htmlDocument = GetHtmlDocument(path);
            var nodesTagDiv = htmlDocument.DocumentNode.SelectNodes("//div[@class='post_body']");

            List<string> textFromNode = new List<string>();
            //delete of smiles
            foreach (var item in nodesTagDiv)
            {

                if (item.ChildNodes.Count > 10)
                {
                    continue;//skiping a node with the theme(without comment)
                }
                else
                {

                    var nodesTagImg = item.SelectNodes(".//img");
                    if (nodesTagImg != null)
                    {
                        foreach (var imgTag in item.SelectNodes(".//img"))//remove child nodes include smiles
                        {
                            imgTag.Remove();
                        }
                    }
                    var divClassQWrap = item.SelectNodes(".//div[@class='q-wrap']");
                    if (divClassQWrap != null)
                    {
                        foreach (var wrap in item.SelectNodes(".//div[@class='q-wrap']"))//remove child nodes include wraps
                        {
                            wrap.Remove();
                        }
                    }

                    string comment = CleanerComment(item.InnerText);
                    if (!(comment == "" || comment == " "))
                    {
                        textFromNode.Add(comment);
                    }

                }
            }
            return textFromNode;
        }



        private HtmlDocument GetHtmlDocument(string url)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//solves the problem with the exception of encoding Windows-1251

            var web = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("Windows-1251")//coding from head html
            };
            var htmlDocument = web.Load(url);
            return htmlDocument;
        }
        private string CleanerComment(string comment)
        {
            string pattern1 = "[\t]+";
            string pattern2 = "[\n]+";
            string pattern3 = @"[^А-Яа-я\s]";
            string pattern4 = @"\s{2,}";
            string cleanComment1 = Regex.Replace(comment, pattern1, " ");
            string cleanComment2 = Regex.Replace(cleanComment1, pattern2, " ");
            string cleanComment3 = Regex.Replace(cleanComment2, pattern3, " ");
            string cleanComment = Regex.Replace(cleanComment3, pattern4, " ");
            return cleanComment;
        }
    }
}
