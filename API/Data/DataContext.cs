using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<
        AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //DB tables

        // public DbSet<AppUser> Users { get; set; }

        //we don't need Photos as independent entity

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            //configure relationship "AppUser" and its 'has-a' collection of "Roles"
            //1-st side of relationship
            builder.Entity<AppUser>()
                .HasMany(ur => ur.Roles)
                .WithOne(u => u.User)
                //"AppUserRole" table field to be linked to "AppUser" object
                .HasForeignKey(fk => fk.UserId)
                .IsRequired();

            //configure relationship "AppRole" and its 'has-a' collection of "Users"
            //2-nd side of relationship
            builder.Entity<AppRole>()
                .HasMany(ar => ar.Users)
                .WithOne(u => u.Role)
                //"AppUserRole" table field to be linked to "AppRole" object
                .HasForeignKey(fk => fk.RoleId)
                .IsRequired();

            //configure the primary key in Likes table
            builder.Entity<UserLike>()
                .HasKey(key => new {key.LikingUserId, key.LikedUserId});

            //configure "UserLike" to its 'own-a' "LikingUser" and "LikedUser" AppUser objects
            // first side of relashionship
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

            //configure messages recipient
            builder.Entity<Message>()
                .HasOne(s => s.Recipient)
                .WithMany(m => m.MessagesReceived)
                .HasForeignKey(fk => fk.RecipientId)
                //we don't want delete the message until sender will delete them
                .OnDelete(DeleteBehavior.Restrict);

            //configure messages sender
            builder.Entity<Message>()
                .HasOne(s => s.Sender)
                .WithMany(m => m.MessagesSent)
                .HasForeignKey(fk => fk.SenderId)
                //we don't want delete the message until recipient will delete them
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}