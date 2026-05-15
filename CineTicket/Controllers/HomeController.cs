using CineTicket.Data;
using CineTicket.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CineTicket.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var movies = _context.Movies.OrderByDescending(m => m.Id).ToList();
            return View(movies);
        }

        public IActionResult Movies()
        {
            var movies = _context.Movies.OrderByDescending(m => m.Id).ToList();
            return View(movies);
        }

        public IActionResult Details(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return RedirectToAction("Index");
            }

            return View(movie);
        }
        [HttpPost]
        public IActionResult AddComment(int MovieId, string Content)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.IsNullOrEmpty(Content))
            {
                var userIdString = User.FindFirst("UserId")?.Value;
                int userId = int.Parse(userIdString);

                var newComment = new Comment
                {
                    MovieId = MovieId,
                    Content = Content,
                    UserId = userId
                };

                _context.Comments.Add(newComment);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Home", new { id = MovieId });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BuyTicket([FromBody] TicketRequest data)
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("The user could not be found, please log in again.");
            }

            int userId = int.Parse(userIdString);

            string rawPnr = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
            string generatedPnr = $"{rawPnr.Substring(0, 3)}-{rawPnr.Substring(3, 3)}-{rawPnr.Substring(6, 3)}-{rawPnr.Substring(9, 3)}";

            var newTicket = new Ticket
            {
                UserId = userId,
                MovieId = data.MovieId,
                City = data.City,
                Hall = data.Hall,
                ShowTime = data.ShowTime,
                SeatNumbers = data.SeatNumbers,
                PnrCode = generatedPnr,
                Price = data.Price,
            };

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "The ticket was successfully purchased!" });
        }

        public class TicketRequest
        {
            public int MovieId { get; set; }
            public string City { get; set; }
            public string Hall { get; set; }
            public string ShowTime { get; set; }
            public string SeatNumbers { get; set; }
            public double Price { get; set; }
        }
    }
}
