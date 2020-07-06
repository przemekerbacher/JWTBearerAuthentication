using Microsoft.IdentityModel.Tokens;
using JWTAuthentication.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthentication.Helpers
{
    public class TokenManager : ITokenManager
    {
        private AppSettings _appSettings;
        private IRefreshTokenManager<RefreshTokenClaims> _refreshTokenManager;

        public TokenManager(AppSettings appSettings, IRefreshTokenManager<RefreshTokenClaims> refreshTokenManager)
        {
            _appSettings = appSettings;
            _refreshTokenManager = refreshTokenManager;
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>{
                new Claim("userId", user.Id)
            };
            var token = new JwtSecurityToken(null, null, claims, expires: DateTime.Now.AddMinutes(_appSettings.JWT_Expire_Minutes), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var token = _refreshTokenManager.GenerateToken(new RefreshTokenClaims
            {
                UserId = user.Id,
                RandomText = GenerateRandemText()
            });

            return token;
        }

        private string GenerateRandemText()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}