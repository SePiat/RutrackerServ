using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TorrServ.Views.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public int Year { get; set; }
        public string Id { get; set; }
        public string Role { get; set; }

    }
}
