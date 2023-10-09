using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;
using Microsoft.Extensions.Logging;
using NLog;
using System.ServiceProcess;
using SLPDBLibrary;


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

        private Logger _logger;

        public WorkWithMail()
        {
            this._logger = LogManager.GetCurrentClassLogger();

        }
        public void Dispose()
        {

        }
        public bool GetConfig()
        {
            bool bResult = false;

            try
            {
                _logger.Info("Start Mail sender configuration");

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
                _logger.Error(e, "Error creating the configuration of the mailing service");
            }
            return bResult;
        }

        public async Task SendMailAsync(int regionID, string regionName, List<MailingAddress> mailingAddresses, string[] listAtached)
        {
            try
            {
                _logger.Info("Asynchronous mailing of reports started");

                using var emailMessage = new MimeMessage();

                // Define Subject mail
                emailMessage.Subject = String.Concat(DateTime.Now.ToLongDateString(), " : ", "Звіт споживання - ", regionName);

                // Define sender address
                emailMessage.From.Add(new MailboxAddress("Система Моніторинг енергоресурсів", this.fromMail));

                // Define destination address
                foreach (var mailItem in mailingAddresses)
                {
                    emailMessage.To.Add(new MailboxAddress(mailItem.Mail, mailItem.Mail));
                }

                // Define the body of the email
                MemoryStream memoryStream = new MemoryStream();
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = @"
                        Доброго дня.

Цей лист був сгенерований автоматичною системою розсилки звітів системи моніторингу споживання енергоресурсів.

Звіти, ви можете побачити у прикріплених файлах. 

Гарного дня. 

З повагою,
Система моніторингу споживання енергоресурсів.";

                // Attaching files to be sent
                using (var wc = new WebClient())
                {
                    if (listAtached != null && listAtached.Length > 0)
                    {
                        for (int i = 0; i < listAtached.Length; i++)
                        {
                            string fileName = new FileInfo(listAtached[i]).Name;

                            bodyBuilder.Attachments.Add(fileName, wc.DownloadData(listAtached[i]));
                        }
                    }

                }

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(this.smtp, this.port, this.useSSL);
                    client.Authenticate(this.fromMail, this.pwd);
                    
                    string sServerResponse =  client.Send(emailMessage);

                    _logger.Info(sServerResponse);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetLogger(regionName).Error(ex);
            }

        }


    }
}