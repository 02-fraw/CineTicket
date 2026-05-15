using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineTicket.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
        public string City { get; set; }     
        public string Hall { get; set; }      
        public string ShowTime { get; set; }  
        public string SeatNumbers { get; set; } 
        public string PnrCode { get; set; }
        //public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public double Price { get; set; }
    }
}