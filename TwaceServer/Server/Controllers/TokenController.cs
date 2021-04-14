using Microsoft.AspNetCore.Identity;
using TwaceServer.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using TwaceServer.Server.Data;

namespace TwaceServer.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        ApplicationContext db;

        /// <summary>
        /// Получить токен
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        /// <param name="context"></param>
        public TokenController(IConfiguration configuration,
                               SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationContext context)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            db = context;
        }

        [HttpPost]
        [Route("Token")]
        public async Task<IActionResult> Token([FromBody] LoginModel login)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(login.Login, login.Password, false, false);

                if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Неправильный логин или пароль" });

                var user = await _userManager.FindByNameAsync(login.Login);
                if (!user.EmailConfirmed) return BadRequest(new LoginResult { Successful = false, Error = "Аккаунт не активирован" });
                if (user.Banned) return BadRequest(new LoginResult { Successful = false, Error = "Пользователь заблокирован" });

                List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, login.Login)
            };

                var roles = await _userManager.GetRolesAsync(user);
                AddRolesToClaims(claims, roles);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

                var token = new JwtSecurityToken(
                    _configuration["JwtIssuer"],
                    _configuration["JwtAudience"],
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );
                string Token = new JwtSecurityTokenHandler().WriteToken(token);

                if (login.DeviceId != null)
                {
                    if (login.DeviceId != "NotRequired")
                    {
                        if (user.Device != null)
                        {
                            user.Device.DeviceId = login.DeviceId;
                        }
                        else
                        {
                            user.Device = new Device() { ApplicationUserId = user.Id, DeviceId = login.DeviceId };
                        }

                        await db.SaveChangesAsync();
                    }
                }

                Console.WriteLine(Token);

                return Ok(new LoginResult { Successful = true, Token = Token });
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                return BadRequest(er.Message);
            }

        }

        private void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }
    }
}
