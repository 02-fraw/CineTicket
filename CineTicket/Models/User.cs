using System.ComponentModel.DataAnnotations;

namespace CineTicket.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = "Member";
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

}