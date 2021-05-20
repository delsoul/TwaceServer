using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwaceServer.Server.Data;
using TwaceServer.Server.Helpers;
using TwaceServer.Shared.Models;
using TwaceServer.Shared.Models.Enums;

namespace TwaceServer.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public RequestsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("NewRequest")]
        public async Task<IActionResult> NewRequest(Request request)
        {
            try
            {
                request.User = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                request.Status = RequestStatus.Sended;

                await _context.Requests.AddAsync(request);

                await _context.SaveChangesAsync();

                await PushNotificationToMail(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet]
        [Route("MyRequests")]
        public async Task<IActionResult> MyRequests()
        {
            try
            {
                ApplicationUser user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
                List<Request> result = await _context.Requests.Where(r => r.UserId == user.Id).ToListAsync();
                result.Reverse();
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task PushNotificationToMail(Request request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);

                try
                {
                    if (user.EmailConfirmed == true)
                    {
                        var comment = (request.Comment != null && request.Comment.Length > 0) ? request.Comment : "отсутствует";
                        await MailHelper.SendMailAsync(user.Email, "Ваш заказ принят!",
                            $"<b>Заявка успешно отправлена</b></br></br></br>" +
                            $"<b>Заявка №{request.Id} от:</b> {request.UserName}</br></br>" +
                            $"<b>Номер телефона:</b> {request.PhoneNumber}</br></br>" +
                            $"<b>Электронная почта:</b> {request.User.Email}</br></br>" +
                            $"<b>Дата события:</b> {request.RequestDateTime}</br></br>" +
                            $"<b>Вам позвонят для подтверждения заказа</b>");
                    }
                }
                catch (Exception ex)
                {
                    //_logger.LogError("PushNotificationToMail |" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError("PushNotificationToMail |" + ex.Message);
            }
        }
    }
}
