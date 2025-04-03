using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Models.Requests;
using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(user.FullName))
                errors.Add("Ad-Soyad alanı boş olamaz.");

            if (string.IsNullOrWhiteSpace(user.Email))
                errors.Add("Email boş olamaz.");

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                errors.Add("Şifre boş olamaz.");

            if (errors.Any())
            {
                return BadRequest(new
                {
                    message = "Eksik veya hatalı alanlar var.",
                    errors
                });
            }

            // 🔍 Email veritabanında var mı kontrolü
            var existingUser = await _userService.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Bu e-posta adresi zaten kayıtlı." });
            }

            // Rolü sistem belirler
            user.Role = "User";

            await _userService.AddAsync(user);
            return Ok(new { message = "Kullanıcı başarıyla kaydedildi." });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request.Email, request.PasswordHash);

            if (user == null)
                return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre." });

            Console.WriteLine("Jwt Key = " + _configuration["Jwt:Key"]);

            // JWT token oluştur
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return Ok(new { token = jwt });
        }

        [HttpGet("me")]
        [Authorize] // Bu endpoint'e sadece token'ı olanlar erişebilir
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Token geçersiz veya eksik." });

            int userId = int.Parse(userIdClaim.Value);
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(user);
        }
    }
}
