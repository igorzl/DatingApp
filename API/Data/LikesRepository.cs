using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<UserLike> GetUserLikeAsync(int likingUserId, int likedUserId)
        {
            //find by primary key
            return await _context.Likes.FindAsync(likingUserId, likedUserId);
        }

        public async Task<AppUser> GetUserWithLikesAsync(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<PagedList<LikeDto>> GetLikedOrLikedByUsersAsync(LikesParams likesParams)
        {
            var likes = _context.Likes.AsQueryable();
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.LikingUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.LikingUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                MainPhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, 
                likesParams.PageNumber, likesParams.PageSize);
                
            // return await users.Select(user => new LikeDto {
            //     Username = user.UserName,
            //     KnownAs = user.KnownAs,
            //     Age = user.DateOfBirth.CalculateAge(),
            //     MainPhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain == true).Url,
            //     City = user.City,
            //     Id = user.Id
            // }).ToListAsync();
        }
    }
}