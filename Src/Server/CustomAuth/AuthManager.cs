using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;

namespace Puniemu.Src.Server.CustomAuth
{
    public static class AuthManager
    {
        // code -> (email, udkey, expiration)
        public static ConcurrentDictionary<int, (string email, bool isLink, string udkey, DateTime expires)> CodeCache = new();

        public static void CleanupExpiredCodes()
        {
            foreach (var entry in CodeCache)
            {
                if (DateTime.UtcNow > entry.Value.expires)
                {
                    CodeCache.TryRemove(entry.Key, out _);
                }
            }
        }

        public static async Task SendCodeEmail(string email, int code)
        {
            var myEmail = DataManager.Logic.DataManager.EmailForAuthMessages;
            var pass = DataManager.Logic.DataManager.AppPasswordForAuthMessages;
            using var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    myEmail,
                    pass
                )
            };

            var mail = new MailMessage
            {
                From = new MailAddress(myEmail),
                Subject = "Your account managment code",
                Body = $"Your account managment code is: {code}\n\nTo continue this process, enter the code in the \"Confirm action\" menu in the game settings.\nThis code expires in 15 minutes.",
                IsBodyHtml = false
            };

            mail.To.Add(email);

            await client.SendMailAsync(mail);
        }

    }
}
