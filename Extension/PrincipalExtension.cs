using System.Security.Claims;
using Todo.Model.UserDto;

namespace Todo.Extension;

public static class PrincipalExtension
{
    public static AuthData GetAuthData(this ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        var token = principal.FindFirstValue(ClaimTypes.Thumbprint) ?? string.Empty;

        return new AuthData()
        {
            Id = id,
            Token = token
        };
    }
}