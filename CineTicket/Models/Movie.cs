using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace CineTicket.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        public string? MovieName { get; set; }

        public string? VerticalImageUrl { get; set; } 
        [NotMapped]
        public IFormFile? VerticalImageFile { get; set; }

        public string? HorizontalImageUrl { get; set; } 
        [NotMapped]
        public IFormFile? HorizontalImageFile { get; set; }

        public string? TrailerLink { get; set; }
        public string? ImdbLink { get; set; }

        //public string? ImdbScore { get; set; }

        public string? Description { get; set; }
        public string? Genre { get; set; }
        public int Duration { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}