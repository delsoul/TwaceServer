using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TwaceServer.Server.Helpers
{
    public class MailHelper
    {
        /// <summary>
        /// Отправить сообщение на почтовый ящик
        /// </summary>
        /// <param name="mail">адрес получателя</param>
        /// <param name="subject">заголовок письма</param>
        /// <param name="body">тело письма</param>
        /// <returns></returns>
        public async static Task<bool> SendMailAsync(string mail, string subject, string body)
        {
            try
            {
                MailAddress from = new MailAddress("dmitry.creator@gmail.com");
                MailAddress to = new MailAddress(mail);
                MailMessage m = new MailMessage(from, to);
                m.Subject = subject;
                m.Body = body;
                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("dmitry.creator@gmail.com", "dsolodov123"),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(m);

                return true;
            }
            catch (Exception er)
            {
                throw;
            }
        }
    }
}
