﻿using Blog_MVC.Data;
using Blog_MVC.Models;
using Blog_MVC.Services;
using Blog_MVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Blog_MVC.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;

        public PostsController(ApplicationDbContext context, ISlugService slugService, IImageService imageService, UserManager<BlogUser> userManager, BlogSearchService blogSearchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {


            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 5;


            var posts = _blogSearchService.Search(searchTerm);

            return View(await posts.ToPagedListAsync(pageNumber, pageSize));

        }


        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }


        public async Task<IActionResult> BlogPostIndex(int? id, int? page)
        {
            if (id is null)
            {
                return NotFound();
            }

            var pageNumber = page ?? 1;
            var pageSize = 5;

            var posts = await _context.Posts
                .Where(p => p.BlogId == id && p.ReadyStatus == Enums.ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .ToPagedListAsync(pageNumber, pageSize);

            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> TagIndex(string tagString, int? page)
        {
            Tag tag = new Tag()
            {
                Text = tagString
            };
            var pageNumber = page ?? 1;
            var pageSize = 5;



            var allPost = await _context.Posts.Include(t => t.Tags).ToListAsync();

            List<Post> filteredPost = new List<Post>();

            foreach (var post in allPost)
            {
                var allTagsOfPost = post.Tags.ToList();
                foreach (var t in allTagsOfPost)
                {
                    if (t.Equals(tag))
                    {
                        filteredPost.Add(post);
                    }
                }
            }


            return View("BlogPostIndex", filteredPost.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(string slug)
        {
            var post = await _context.Posts
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null) return NotFound();

            var dataVm = new PostDetailViewModel()
            {
                Post = post,
                Tags = _context.Tags
                    .Select(t => t.Text.ToLower())
                    .Distinct().ToList()
            };
            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["MainText"] = post.Title;
            ViewData["Subtext"] = post.Abstract;
            return View(dataVm);
        }



        // GET: Posts/Create
        public IActionResult Create(int? id)
        {

            ViewData["BlogId"] = id;
            //ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            //ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;

            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;
                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                // Use the _imageService to store the incoming user specified image
                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);


                var slug = _slugService.UrlFriendly(post.Title);

                //Create a variable to store whether an error has occurred
                var validationError = false;


                //Detect empty slug
                if (string.IsNullOrEmpty(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("", "The title you provided cannot be used as it results in an empty slug.");

                }

                //Detect incoming duplicate slugs
                else if (!_slugService.IsUnique(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("Title", "The title you provided cannot be used as it results in duplicate slug.");

                }


                if (validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();


                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });
            }


            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);

        }



        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post post, IFormFile newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var originalPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);
                try
                {
                    //The original Post


                    originalPost.Updated = DateTime.Now;
                    originalPost.Title = post.Title;
                    originalPost.Abstract = post.Abstract;
                    originalPost.Content = post.Content;
                    originalPost.ReadyStatus = post.ReadyStatus;

                    var newSlug = _slugService.UrlFriendly(post.Title);

                    if (newSlug != originalPost.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            originalPost.Title = post.Title;
                            originalPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This title cannot be used as it results in a duplicate slug");
                            ViewData["TagValues"] = string.Join(",", tagValues);
                            return View(post);
                        }
                    }

                    if (newImage is not null)
                    {
                        originalPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        originalPost.ContentType = _imageService.ContentType(newImage);
                    }

                    // Remove all Tags previously associated with this Post
                    _context.Tags.RemoveRange(originalPost.Tags);

                    // Add in the new Tags from the edit form

                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = originalPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


                return RedirectToAction("BlogPostIndex", new { id = originalPost.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);



            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
