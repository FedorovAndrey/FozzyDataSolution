using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

//rcvLbyuc5Yvd08Mk

//"MailSettings": {
//    "smtp": "smtp.ukr.net",
//    "port": "2525",
//    "SSL": "true",
//    "From": "an.fedorov@ukr.net",
//    "pwd": "rcvLbyuc5Yvd08Mk"
//  }

namespace SLPMailSender
{
    public class WorkWithMail : IDisposable
    {
        private bool useSSL;
        private int port;
        private string? smtp;
        private string? fromMail;
        private string? pwd;
        public WorkWithMail()
        {
        }
        public void Dispose()
        {

        }
        public bool GetConfig()
        {
            bool bResult = false;

            try
            {
                var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var configSection = configBuilder.GetSection("MailSettings");
                
                useSSL = Convert.ToBoolean(configSection["SSL"]);
                port = Convert.ToInt32(configSection["port"]);
                smtp = configSection["smtp"] ?? null;
                fromMail = configSection["From"] ?? null;
                pwd = configSection["pwd"] ?? null;
            }
            catch (Exception e)
            {

            }
            return bResult;
        }

        public async Task SendMailAsync(string mail, string subject, string body)
        {
            try
            {
                using var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Система Моніторинг енергоресурсів", this.fromMail));
                emailMessage.To.Add(new MailboxAddress("", mail));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(this.smtp, this.port, this.useSSL);
                    client.Authenticate(this.fromMail, this.pwd);
                    client.Send(emailMessage);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {

            }

        }

    }
}