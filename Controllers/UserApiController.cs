using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JWTAuthentication.Data;
using JWTAuthentication.Helpers;
using JWTAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAuthentication.Helpers;

namespace JWTAuthentication.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private RelacjeBazyDanychContext _context;
        private AppSettings _appSettings;
        private ITokenManager _tokenManager;
        private IRefreshTokenManager<RefreshTokenClaims> _refreshTokenManager;
        private IMailSender _mailSender;

        public UserApiController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RelacjeBazyDanychContext context,
            AppSettings appSettings,
            ITokenManager tokenManager,
            IRefreshTokenManager<RefreshTokenClaims> refreshTokenManager,
            IMailSender mailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenManager = tokenManager;
            _context = context;
            _appSettings = appSettings;
            _refreshTokenManager = refreshTokenManager;
            _mailSender = mailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Login
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return StatusCode(200);
                }
                else
                {
                    return StatusCode(406);
                }
            }
            return StatusCode(406);
        }

        [HttpGet("login")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Login);
                    var tokens = CreateTokens(user);

                    AddRefreshToken(user, tokens.RefreshToken);

                    return Ok(tokens);
                }
                else
                {
                    return StatusCode(401);
                }
            }

            return StatusCode(401);
        }

        [HttpGet("Info")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Info()
        {
            var currentUser = HttpContext.User;
            var id = currentUser.Claims.First(x => x.Type == "userId").Value;
            var user = await _userManager.FindByIdAsync(id);

            return Ok(user.UserName);
        }

        [HttpPut("refreshtoken")]
        public IActionResult RefreshToken(RefreshTokenRequestModel model)
        {
            var token = model.RefreshToken;

            if (!IsValid(token))
            {
                return StatusCode(401);
            }

            var refreshTokenClaims = _refreshTokenManager.EncodeToken(token);
            var userId = refreshTokenClaims.UserId;
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);

            var newRefreshToken = new RefreshToken
            {
                Token = _tokenManager.GenerateRefreshToken(user),
                Expires = DateTime.Now.AddMinutes(_appSettings.JWT_Refresh_Token_Expire_Minutes)
            };

            var userRefreshTokens = user.RefreshTokens;
            var oldRefreshToken = userRefreshTokens.Where(t => t.Token == token).FirstOrDefault();
            userRefreshTokens.Remove(oldRefreshToken);
            userRefreshTokens.Add(newRefreshToken);

            var newToken = _tokenManager.GenerateToken(user);

            _context.Update(user);
            _context.SaveChanges();

            var tokenPair = new TokenPair
            {
                Token = newToken,
                RefreshToken = newRefreshToken.Token
            };

            return Ok(tokenPair);
        }

        [HttpPost("sendmail")]
        public IActionResult SendMail()
        {
            _mailSender.Send(new MailModel()
            {
                From = "przemyslaw.erbacher@gmail.com",
                PlainTextContent = "test maila",
                Subject = "test",
                To = "pe@s4h.pl"
            });

            return Ok();
        }

        private TokenResponseModel CreateTokens(User user)
        {
            var tokenResponseModel = new TokenResponseModel
            {
                RefreshToken = _tokenManager.GenerateRefreshToken(user),
                Token = _tokenManager.GenerateToken(user)
            };

            return tokenResponseModel;
        }

        private void AddRefreshToken(User user, string refreshToken)
        {
            if (user.RefreshTokens == null)
            {
                user.RefreshTokens = new List<RefreshToken>();
            }

            var newToken = CreateRefreshToken(refreshToken, user);
            user.RefreshTokens.Add(newToken);

            _context.Update(user);
            _context.SaveChanges();
        }

        private RefreshToken CreateRefreshToken(string token, User user)
        {
            var tokenObject = new RefreshToken
            {
                Token = token,
                Expires = DateTime.Now.AddMinutes(_appSettings.JWT_Refresh_Token_Expire_Minutes),
            };

            return tokenObject;
        }

        private bool IsValid(string token)
        {
            if (token == null)
            {
                return false;
            }

            var refreshTokenClaims = _refreshTokenManager.EncodeToken(token);
            var userId = refreshTokenClaims.UserId;
            var user = _context.Users.Include(u => u.RefreshTokens).SingleOrDefault(u => u.Id == userId);

            if (user == null || user.RefreshTokens == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Where(x => x.Token == token).FirstOrDefault();

            if (refreshToken == null)
            {
                return false;
            }

            if (refreshToken.Expires < DateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}