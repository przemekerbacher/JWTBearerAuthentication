using JWTAutentication.Models;

namespace JWTAutentication.Helpers
{
    public interface ITokenManager
    {
        string GenerateToken(User user);

        string GenerateRefreshToken(User user);
    }
}