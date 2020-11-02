using System;
using System.Collections.Generic;
using System.Text;

namespace TorrServData.Models
{
    public class Comments
    {
        public Guid Id { get; set; }
        public TorrentMovie movieId { get; set; } 
        public int commentIndex { get; set; }
        public string commentText { get; set; }


    }
}
