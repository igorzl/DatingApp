using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        //Likes table object (or null) for given pair of users
        Task<UserLike> GetUserLikeAsync(int likingUserId, int likedUserId);

        //AppUser object with collection of her liked users
        Task<AppUser> GetUserWithLikesAsync(int userId);

        //LikeDto lists of both types (or/or) of "Likes" relationships for the given user (see predicate)
        //Task<IEnumerable<LikeDto>> GetLikedOrLikedByUsersAsync(string predicate, int userId);
        Task<PagedList<LikeDto>> GetLikedOrLikedByUsersAsync(LikesParams likesParams);
    }
}