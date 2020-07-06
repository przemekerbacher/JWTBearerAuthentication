namespace JWTAuthentication.Models
{
    public class RefreshTokenClaims
    {
        public string UserId { get; set; }
        public string RandomText { get; set; }
    }
}