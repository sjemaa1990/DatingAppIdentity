using Microsoft.EntityFrameworkCore;
using SGS.eCalc.Models;

namespace SGS.eCalc.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        public DbSet<CalculationVersion> CalculationVersions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // combination of primary key to avoid the many like of the same photo by the same user
            builder.Entity<Like>().HasKey(key => new {key.LikerId, key.LikeeId});

            builder.Entity<Like>().HasOne(u => u.Likee)
                                  .WithMany(u => u.Likers)
                                  .HasForeignKey( u => u.LikeeId)
                                  .OnDelete(DeleteBehavior.Restrict); // restrict to avoid the delete of user profile, delete only like ligne

             builder.Entity<Like>().HasOne(u => u.Liker)
                                  .WithMany(u => u.Likees)
                                  .HasForeignKey( u => u.LikerId)
                                  .OnDelete(DeleteBehavior.Restrict); // restrict to avoid the delete of user profile, delete only like ligne

            builder.Entity<Message>()
                                .HasOne(u => u.Sender)
                                .WithMany(u => u.MessagesSent)
                                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                    .HasOne(u => u.Recipient)
                    .WithMany(u => u.MessagesReceived)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}