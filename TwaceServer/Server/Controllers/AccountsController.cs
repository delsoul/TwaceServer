using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwaceServer.Server.Data;
using TwaceServer.Server.Helpers;
using TwaceServer.Shared.Models;

namespace TwaceServer.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        ApplicationContext db;

        public AccountsController(ApplicationContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            db = context;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Login);

                if (user != null)
                {
                    if (user.EmailConfirmed == false) // если пользователь регистрировался, но не активировал свою почту / другой пользователь может использовать эту почту
                    {
                        string verifyCode = await GetVerifyCode();

                        if (user.ConfirmCode != null)
                        {
                            user.ConfirmCode.VerifyCode = verifyCode;
                        }
                        else
                        {
                            user.ConfirmCode = new ConfirmCode() { VerifyCode = verifyCode };
                        }

                        await db.SaveChangesAsync();

                        await MailHelper.SendMailAsync(model.Login, $"Код подтверждения", verifyCode); // TODO change email and text

                        return Ok(new RegisterResult() { Successful = true, Error = null, VerifyCode = verifyCode });
                    }
                    else
                    {
                        return BadRequest(new RegisterResult() { Successful = false, Error = "Пользователь с таким почтовым ящиком уже зарегистрирован", VerifyCode = null });
                    }
                }
                else
                {
                    string verifyCode = await GetVerifyCode();
                    var new_user = new ApplicationUser() { UserName = model.Login, Email = model.Login, ConfirmCode = new ConfirmCode() { VerifyCode = verifyCode } };

                    IdentityResult result = await _userManager.CreateAsync(new_user, model.Password);

                    if (!result.Succeeded)
                    {
                        if (result.Errors.First().Description == $"User name '{model.Login}' is invalid, can only contain letters or digits.")
                        {
                            return BadRequest(new RegisterResult() { Successful = true, Error = "Неккоректный почтовый ящик", VerifyCode = null });
                        }
                        else
                        {
                            return BadRequest(new RegisterResult() { Successful = true, Error = result.Errors.First().Description, VerifyCode = null });
                        }
                    }
                    else
                    {
                        if (await _roleManager.FindByNameAsync("User") == null)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }

                        await _userManager.AddToRoleAsync(new_user, "user");

                        await MailHelper.SendMailAsync(model.Login, $"Код подтверждения", verifyCode); // TODO: change email adress and text

                        return Ok(new RegisterResult() { Successful = true, Error = null, VerifyCode = verifyCode });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResult() { Successful = false, Error = "Ошибка сервера", VerifyCode = null });
            }
        }

        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(RegisterModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Login);

                if (user != null)
                {
                    string verify_code = user.ConfirmCode?.VerifyCode;

                    if (verify_code != null)
                    {
                        if (verify_code == model.VerifyCode)
                        {
                            user.EmailConfirmed = true;
                            IdentityResult result = await _userManager.UpdateAsync(user);

                            if (!result.Succeeded)
                            {
                                return BadRequest(result.Errors.First().Description);
                            }

                            return Ok(new RegisterResult() { Successful = true });
                        }
                        else
                        {
                            return BadRequest(new RegisterResult() { Successful = false, Error = "Небезопасный вход", VerifyCode = null });
                        }
                    }
                    else
                    {
                        return BadRequest(new RegisterResult() { Successful = false, Error = "Небезопасный вход", VerifyCode = null });
                    }
                }
                else
                {
                    return BadRequest(new RegisterResult() { Successful = false, Error = "Небезопасный вход", VerifyCode = null });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResult() { Successful = false, Error = "Ошибка сервера", VerifyCode = null });
            }
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
            {
                return BadRequest(new RegisterResult() { Successful = false, Error = "Пользователя c данной почтой не существует", VerifyCode = null });
            }

            if (user.Banned == true)
            {
                return BadRequest(new RegisterResult() { Successful = false, Error = "Пользователь заблокирован", VerifyCode = null });
            }

            try
            {
                string verifyCode = await GetVerifyCode();

                if (user.ConfirmCode != null)
                {
                    user.ConfirmCode.VerifyCode = verifyCode;
                }
                else
                {
                    user.ConfirmCode = new ConfirmCode() { VerifyCode = verifyCode };
                }

                await _userManager.UpdateAsync(user);

                await MailHelper.SendMailAsync(email, $"Код подтверждения", verifyCode);

                return Ok(new RegisterResult() { Successful = true, Error = null, VerifyCode = verifyCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResult() { Successful = false, Error = "Ошибка сервера", VerifyCode = null });
            }
        }

        /// <summary>
        /// тест
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(RegisterModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Login);

                if (user != null)
                {
                    if (user.ConfirmCode.VerifyCode == model.VerifyCode)
                    {
                        if (user.EmailConfirmed == false) // Для зарегистрированных админинистраторами ( они просто восстанавливают пароль, тем самым подтверждают свою почту )
                        {
                            user.EmailConfirmed = true;
                            IdentityResult r = await _userManager.UpdateAsync(user);

                            if (!r.Succeeded)
                            {
                                return BadRequest(new RegisterResult() { Successful = false, Error = "Ошибка сервера (3)" });
                            }
                        }

                        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
                        if (result.Succeeded)
                        {
                            return Ok(new RegisterResult() { Successful = true });
                        }
                        else
                        {
                            return BadRequest(new RegisterResult() { Successful = false, Error = "Ошибка сервера (2)" });
                        }
                    }
                    else
                    {
                        return BadRequest(new RegisterResult() { Successful = false, Error = "Код подверждения не совпадает" });
                    }
                }
                else
                {
                    return BadRequest(new RegisterResult() { Successful = false, Error = "Ошибка сервера (1)" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new RegisterResult() { Successful = false, Error = "Ошибка сервера (0)" });
            }
        }

        /// <summary>
        /// Смена пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<RegisterResult> ChangePassword(ChangePassModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user != null)
                {

                    var result = await _userManager.ChangePasswordAsync(user, model.OldPass, model.NewPass);

                    if (result.Succeeded)
                    {
                        return new RegisterResult() { Successful = true };
                    }
                    else
                    {
                        return new RegisterResult() { Successful = false, Error = result.Errors.First().Description };
                    }

                }
                else
                {
                    return new RegisterResult() { Successful = false, Error = "Пользователь не найден" };
                }
            }
            catch (Exception ex)
            {
                return new RegisterResult() { Successful = false, Error = ex.Message };
            }
        }



        private static async Task<string> GetVerifyCode()
        {
            Random rand = new Random();
            string VERIFY_CODE = rand.Next(99999, 1000000).ToString();

            //await SendEmailAsync(/*email*/ "danismar@mail.ru", VERIFY_CODE); // TODO: change email adress

            return VERIFY_CODE;
        }
    }
}
