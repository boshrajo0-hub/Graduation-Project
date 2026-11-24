//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using HealthCareCenter.Data;
//using HealthCareCenter.DTOs;
//using HealthCareCenter.Models;
//using HealthCareCenter.Services.Interfaces;




//namespace HealthCareCenter.Services.Implementations
//{
//    public class AuthService : IAuthService
//    {
//        private readonly HealthCareCenterDBContext _db;
//        private readonly IConfiguration _config;
//        private readonly PasswordHasher<User> _passwordHasher = new();

//        public AuthService(HealthCareCenterDBContext db, IConfiguration config)
//        {
//            _db = db;
//            _config = config;
//        }

//        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
//        {
//            var email = request.Email.Trim().ToLowerInvariant();
//            var user = await _db.Users
//                .FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.IsActive, ct);

//            if (user is null)
//                return null;

//            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
//            if (verification == PasswordVerificationResult.Failed)
//                return null;

//            var jwtSection = _config.GetSection("Jwt");
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
//            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
//            new Claim(ClaimTypes.Email, user.Email),
//            new Claim(ClaimTypes.Role, user.Role)
//        };

//            var token = new JwtSecurityToken(
//                issuer: jwtSection["Issuer"],
//                audience: jwtSection["Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(8),
//                signingCredentials: creds);

//            return new LoginResponse(
//                new JwtSecurityTokenHandler().WriteToken(token),
//                user.UserId,
//                user.Email,
//                user.Role);
//        }
//    }
//}


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HealthCareCenter.Data;
using HealthCareCenter.DTOs;
using HealthCareCenter.Models;
using HealthCareCenter.Services.Interfaces;

namespace HealthCareCenter.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin", "Doctor", "Receptionist", "Pharmacist", "Patient"
        };

        private readonly HealthCareCenterDBContext _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(HealthCareCenterDBContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.IsActive, ct);

            if (user is null)
                return null;

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verification == PasswordVerificationResult.Failed)
                return null;

            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new LoginResponse(
                new JwtSecurityTokenHandler().WriteToken(token),
                user.UserId,
                user.Email,
                user.Role);
        }

        public async Task<(bool Ok, string? Error, UserDto? User)> CreateUserAsync(
            CreateUserRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return (false, "Email and password are required.", null);

            if (!AllowedRoles.Contains(request.Role))
                return (false, "Invalid role. Allowed roles: Admin, Doctor, Receptionist, Pharmacist, Patient.", null);

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail, ct);
            if (exists)
                return (false, "A user with this email already exists.", null);

            var canonicalRole = AllowedRoles.First(r => r.Equals(request.Role, StringComparison.OrdinalIgnoreCase));
            var user = new User
            {
                Email = normalizedEmail,
                Role = canonicalRole,
                IsActive = request.IsActive
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            return (true, null, new UserDto(user.UserId, user.Email, user.Role, user.IsActive));
        }

        public async Task<(bool Ok, string? Error)> UpdateUserAsync(
            int userId, UpdateUserRequest request, CancellationToken ct = default)
        {
            if (!AllowedRoles.Contains(request.Role))
                return (false, "Invalid role.");

            var user = await _db.Users.FindAsync([userId], ct);
            if (user is null)
                return (false, "User not found.");

            user.Role = AllowedRoles.First(r => r.Equals(request.Role, StringComparison.OrdinalIgnoreCase));
            user.IsActive = request.IsActive;

            await _db.SaveChangesAsync(ct);
            return (true, null);
        }
    }
}
