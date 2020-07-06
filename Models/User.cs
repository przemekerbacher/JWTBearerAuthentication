using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace JWTAutentication.Models
{
    public class User : IdentityUser
    {
        public IList<RefreshToken> RefreshTokens { get; set; }
    }
}