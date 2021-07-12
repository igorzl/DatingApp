using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }

        //we don't need Photos as independent entity

        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            //configure the primary key
            builder.Entity<UserLike>()
                .HasKey(key => new {key.LikingUserId, key.LikedUserId});

            //configure the one side of relashionship
            builder.Entity<UserLike>()
                .HasOne(s => s.LikingUser)
                .WithMany(m => m.LikedUsers) //AppUser
                .HasForeignKey(fk => fk.LikingUserId)
                .OnDelete(DeleteBehavior.Cascade);

            //configure the second side of relashionship
            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(m => m.LikedByUsers) //AppUser
                .HasForeignKey(fk => fk.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}