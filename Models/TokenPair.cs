namespace JWTAuthentication.Models
{
    public class TokenPair
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}