namespace WebApplication1.Dtos.Users;

public class UserInfoDto
{
    public string Avatar { get; set; } = string.Empty;

    public string HomePath { get; set; } = "/growth/dashboard";

    public string RealName { get; set; } = string.Empty;

    public IReadOnlyList<string> Roles { get; set; } = [];

    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
}
