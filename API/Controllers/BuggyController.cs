using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")] //401
        public ActionResult<string> GetUnauthError()
        {
            return "secret text";
        }

        [HttpGet("not-found")] //404
        public ActionResult<AppUser> GetNotFound()
        {
            var noChanceUser = _context.Users.Find(-1);
            if (noChanceUser == null)
                return NotFound();
            else
                return Ok(noChanceUser);
        }

        [HttpGet("server-error")] //500
        public ActionResult<string> GetServerError()
        {
                var noChanceUser = _context.Users.Find(-1);

                var produceError = noChanceUser.ToString(); //exception guaranteed

                return produceError;
        }

        [HttpGet("bad-request")] //400
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was a bad request");
        }
    }
}