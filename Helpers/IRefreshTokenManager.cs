namespace JWTAuthentication.Helpers
{
    public interface IRefreshTokenManager<T> where T : class
    {
        string GenerateToken(T @object);

        T EncodeToken(string token);
    }
}