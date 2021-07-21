using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
             ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await UserExists(registerDto.Username)) return BadRequest("Username is already taken");

            var user = _mapper.Map<AppUser>(registerDto);

            // HMAC uses two passes of hash computation.
            // The secret key is first used to derive two keys – inner and outer.
            // The first pass of the algorithm produces an internal hash derived from the message and the inner key.
            // The second pass produces the final HMAC code derived from the inner hash result and the outer key.
            // Thus the algorithm provides better immunity against length extension attacks.

            //var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            // .Net Identity instead
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            // user.PasswordSalt = hmac.Key;

            var identitiyResult = await _userManager.CreateAsync(user, registerDto.Password);

            if(!identitiyResult.Succeeded)
                return BadRequest(identitiyResult.Errors);

            //every new registered user add to role "Member"
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!addToRoleResult.Succeeded)
                return BadRequest(addToRoleResult.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user),
                Gender = user.Gender,
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //we don't use FindAsync, because it for primary key fields
            //we don't use FirstOrDefault
            var user = await _userManager.Users
                .Include(user => user.Photos)
                .SingleOrDefaultAsync(user => user.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid user name");

            //salt(key) is known
            // using var hmac = new HMACSHA512(user.PasswordSalt);

            // var memStream = new MemoryStream(Encoding.UTF8.GetBytes(loginDto.Password));
            // var computedHash = await hmac.ComputeHashAsync(memStream);

            // string compareError;
            // if (!ComparableByteArray.CompareData(
            //     computedHash,
            //     user.PasswordHash,
            //     "=",
            //     out compareError))
            // {
            //     return Unauthorized("Invalid user password");
            // }

            // .Net Identity instead

            var signInResult = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!signInResult.Succeeded)
                return Unauthorized();

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user),
                MainPhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender,
                KnownAs = user.KnownAs
            };
        }

        public async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

    }
}