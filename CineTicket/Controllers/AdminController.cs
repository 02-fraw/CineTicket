using CineTicket.Data;
using CineTicket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        //[Authorize]
        public IActionResult Index()
        {
            var movies = _context.Movies.OrderByDescending(m => m.Id).ToList();
            var comments = _context.Comments.Include(c => c.Movie).OrderByDescending(c => c.Id).ToList();
            ViewBag.Comments = comments;

            var users = _context.Users.OrderByDescending(u => u.Id).ToList();
            ViewBag.Users = users;

            return View(movies);
        }
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, string[] genres)
        {
            // 1. Türleri Birleştir (Action, Drama vs.)
            if (genres != null && genres.Length > 0)
            {
                movie.Genre = string.Join(", ", genres);
            }

            // 2. DİKEY RESİM YÜKLEME (Poster)
            if (movie.VerticalImageFile != null)
            {
                var extension = Path.GetExtension(movie.VerticalImageFile.FileName); // .jpg
                var newImageName = Guid.NewGuid() + extension; // Rastgele isim
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", newImageName);

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await movie.VerticalImageFile.CopyToAsync(stream);
                }
                movie.VerticalImageUrl = "/img/" + newImageName; // Veritabanına yolu yaz
            }

            // 3. YATAY RESİM YÜKLEME (Arka Plan)
            if (movie.HorizontalImageFile != null)
            {
                var extension = Path.GetExtension(movie.HorizontalImageFile.FileName);
                var newImageName = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", newImageName);

                using (var stream = new FileStream(location, FileMode.Create))
                {
                    await movie.HorizontalImageFile.CopyToAsync(stream);
                }
                movie.HorizontalImageUrl = "/img/" + newImageName;
            }

            // 4. Veritabanına Kaydet
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}