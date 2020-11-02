using System;

namespace TorrServData.Models
{
    public class TorrentMovie
    {
        public Guid Id { get; set; }
        public string title { get; set; }
        public string genre { get; set; }
        public string country { get; set; }
        public string earOfIssue { get; set; }
        public string videoQuality { get; set; }
        public string size { get; set; }
        public int commentIndex { get; set; }
        public int downloads { get; set; }
        public string pathDownLoad { get; set; }
        public string movieId { get; set; }
        public int amountOfComments { get; set; }
        public string lastPost { get; set; }


    }

}
