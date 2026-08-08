using Todo.Entities;

namespace Todo.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateJwtToken(UserEntity user);
}
