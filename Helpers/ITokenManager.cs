using JWTAuthentication.Models;

namespace JWTAuthentication.Helpers
{
    public interface ITokenManager
    {
        string GenerateToken(User user);

        string GenerateRefreshToken(User user);
    }
}