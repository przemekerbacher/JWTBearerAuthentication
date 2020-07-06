namespace JWTAutentication.Models
{
    public class TokenResponseModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}