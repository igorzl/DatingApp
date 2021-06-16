using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) {

            if(await UserExists(registerDto.Username)) return BadRequest("Username is already taken");

            //MAC + SHA512 - salt (key) generated
            var hmac = new HMACSHA512();

            var user = new AppUser {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user); // not adds to DB, only tracks

            await _context.SaveChangesAsync();

            return new UserDto {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) {
            //we don't use FindAsync, because it for primary key fields
            //we don't use FirstOrDefault
            var user = await _context.Users
                .SingleOrDefaultAsync(user => user.UserName == loginDto.Username.ToLower());

            if(user == null) return Unauthorized("Invalid user name");

            //salt(key) is known
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(loginDto.Password));
            var computedHash = await hmac.ComputeHashAsync(memStream);

            string compareError;
            if (!ComparableByteArray.CompareData(
                computedHash,
                user.PasswordHash,
                "=",
                out compareError))
                {
                    return Unauthorized("Invalid user password");
                }

            return new UserDto {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<bool> UserExists(string username) {
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

    }
}