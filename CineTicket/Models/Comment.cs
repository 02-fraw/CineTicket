using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineTicket.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }


        //public string UserName { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}