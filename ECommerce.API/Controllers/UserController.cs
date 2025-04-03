using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] User user)
        {
            await _userService.AddAsync(user);
            return Ok(new { message = "Kullanıcı başarıyla eklendi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User updatedUser)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;
            user.Role = updatedUser.Role;

            await _userService.UpdateAsync(user);

            return Ok(new { message = "Kullanıcı başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok(new { message = "Kullanıcı silindi." });
        }

    }
}
