using System.Linq;
using BCrypt.Net; 
using DrPioMuseumSurveyAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrPioMuseumSurveyAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Admins.FirstOrDefault(u => u.Username == request.Username);

            if (user == null) return Unauthorized(new { message = "User not found" });

            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (validPassword)
            {
                return Ok(new { token = "valid-token-" + user.Id });
            }
            else
            {
                return Unauthorized(new { message = "Wrong password" });
            }
        }
        
        [HttpGet("admins")]
        public IActionResult GetAllAdmins()
        {
            var list = _context.Admins
                .Select(a => new { a.Id, a.Username })
                .ToList();
            return Ok(list);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            if (_context.Admins.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists!" });
            }

            string secureHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newAdmin = new Admin
            {
                Username = request.Username,
                Password = secureHash
            };

            _context.Admins.Add(newAdmin);
            _context.SaveChanges();

            return Ok(new { message = "New admin added successfully!" });
        }

        [HttpDelete("admins/{id}")]
        public IActionResult DeleteAdmin(int id)
        {
            var admin = _context.Admins.Find(id);
            if (admin == null) return NotFound(new { message = "Admin not found" });

            if (admin.Username.ToLower() == "admin")
            {
                return BadRequest(new { message = "You cannot delete the master admin account." });
            }

            _context.Admins.Remove(admin);
            _context.SaveChanges();

            return Ok(new { message = "Admin deleted successfully." });
        }

    }
}