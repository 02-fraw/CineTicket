using CineTicket.Models;
using Microsoft.EntityFrameworkCore;


namespace CineTicket.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

    }
}