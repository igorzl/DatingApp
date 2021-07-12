using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //make sure we are authenticated
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        //method for logged in user to like another user
        //api/likes/doris
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var likingUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var likingUser = await _likesRepository.GetUserWithLikesAsync(likingUserId);

            if(likedUser == null) return NotFound();

            //likes herself
            if(likingUser.UserName == username) return BadRequest("You cannot like yourself");

            //check maybe already liked (we don't implement toggle!)
            var userLike = await _likesRepository.GetUserLikeAsync(likingUserId, likedUser.Id);
            if(userLike != null) return BadRequest("You are already like this user");

            userLike = new UserLike
             {
                LikingUserId = likingUserId,
                LikedUserId = likedUser.Id,
                //don't need those
                // LikingUser = likingUser,
                // LikedUser = likedUser
            };

            likingUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }

        //return liked users in LikeDto format
        //api/likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetLikedOrLikedByUsers([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var likedOrLikedByUsers = await _likesRepository.GetLikedOrLikedByUsersAsync(likesParams);

            Response.AddPaginationHeader(likedOrLikedByUsers.CurrentPage, 
                likedOrLikedByUsers.PageSize, likedOrLikedByUsers.TotalCount, likedOrLikedByUsers.TotalPages);

            // see "UsersController" -> GetUsers
            return Ok(likedOrLikedByUsers);
        }

    }
}