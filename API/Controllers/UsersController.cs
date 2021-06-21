using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Collections.Generic;
using API.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() {
            var users = await  _userRepository.GetMembersAsync();

            return Ok(users);
        }

        //api/users/doris
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username) {
            return await _userRepository.GetMemberAsync(username);
        }

        //api/users/ (doen't conflict with [HttpGet], because it is [HttpPut])
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //we get username from the given request authenticated user token
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);

            //automatically maps data from DTO to user entity
            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            //no need to send any conent
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
   }
}