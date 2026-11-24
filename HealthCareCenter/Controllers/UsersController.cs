//using Microsoft.AspNetCore.Mvc;

//namespace HealthCareCenter.Controllers
//{
//    public class UsersController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}

using HealthCareCenter.Data;
using HealthCareCenter.DTOs;
using HealthCareCenter.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthCareCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly HealthCareCenterDBContext _db;
        private readonly IAuthService _authService;

        public UsersController(HealthCareCenterDBContext db, IAuthService authService)
        {
            _db = db;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(CancellationToken ct)
        {
            var users = await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.UserId)
                .Select(u => new UserDto(u.UserId, u.Email, u.Role, u.IsActive))
                .ToListAsync(ct);

            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(
            [FromBody] CreateUserRequest request, CancellationToken ct)
        {
            var (ok, error, user) = await _authService.CreateUserAsync(request, ct);
            if (!ok || user is null)
                return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetUsers), new { id = user.UserId }, user);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(
            int id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        {
            var (ok, error) = await _authService.UpdateUserAsync(id, request, ct);
            if (!ok)
                return BadRequest(new { message = error });

            return NoContent();
        }
    }
}
