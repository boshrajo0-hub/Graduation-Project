//using HealthCareCenter.DTOs;

//namespace HealthCareCenter.Services.Interfaces
//{
//    public interface IAuthService
//    {
//        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
//    }
//}


using HealthCareCenter.DTOs;

namespace HealthCareCenter.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<(bool Ok, string? Error, UserDto? User)> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> UpdateUserAsync(int userId, UpdateUserRequest request, CancellationToken ct = default);
    }
}