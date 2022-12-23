using Blog_MVC.Models;
using System.Collections.Generic;

namespace Blog_MVC.ViewModels
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
