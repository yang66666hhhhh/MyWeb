using WebApplication1.Features.Auth.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Auth;

public interface IUserService
{
    Task<PageResult<UserDto>> GetPageAsync(UserQueryDto query, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserDto input, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto input, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(Guid id, ResetPasswordDto input, CancellationToken cancellationToken = default);
    Task<bool> ValidatePasswordAsync(string username, string password);
}