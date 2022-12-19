using Blog_MVC.Data;
using Blog_MVC.Enums;
using Blog_MVC.Models;
using System.Linq;

namespace Blog_MVC.Services
{
    public class BlogSearchService
    {
        private readonly ApplicationDbContext _dbContext;

        public BlogSearchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Post> Search(string searchTerm)
        {
            var posts = _dbContext.Posts.Where(p => p.ReadyStatus == ReadyStatus.ProductionReady).AsQueryable();
            if (searchTerm != null)
            {
                searchTerm = searchTerm.ToLower();

                posts = posts.Where(
                    p => p.Title.ToLower().Contains(searchTerm) ||
                         p.Abstract.ToLower().Contains(searchTerm) ||
                         p.Content.ToLower().Contains(searchTerm) ||
                         p.Comments.Any(c => c.Body.ToLower().Contains(searchTerm) ||
                                             c.ModeratedBody.ToLower().Contains(searchTerm) ||
                                             c.BlogUser.FirstName.ToLower().Contains(searchTerm) ||
                                             c.BlogUser.LastName.ToLower().Contains(searchTerm) ||
                                             c.BlogUser.Email.ToLower().Contains(searchTerm)));
            }

            return posts.OrderByDescending(p => p.Created);
        }

    }
}
