using System.Collections.Generic;
using System.Net;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NLog;
using SLPDBLibrary;
using SLPHelper;


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

        private static Logger logger = LogManager.GetLogger("logger");

        public WorkWithMail()
        {
            //this.logger = LogManager.GetCurrentClassLogger();

        }
        public void Dispose()
        {

        }
        public bool GetConfig()
        {
            bool bResult = false;

            try
            {
                logger.Info("Start Mail sender configuration");

                var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var configSection = configBuilder.GetSection("MailSettings");

                useSSL = Convert.ToBoolean(configSection["SSL"]);
                port = Convert.ToInt32(configSection["port"]);
                smtp = configSection["smtp"] ?? null;
                fromMail = configSection["From"] ?? null;
                pwd = configSection["pwd"] ?? null;

                bResult = true;
            }
            catch (Exception e)
            {
                logger.Error(e, "Error creating the configuration of the mailing service");
            }
            return bResult;
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task SendMailAsync(int regionID, string regionName, List<MailingAddress> mailingAddresses, string filename)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                logger.Info("Asynchronous mailing of reports started");

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
                    if (filename != String.Empty && File.Exists(filename))
                    {
                        bodyBuilder.Attachments.Add(filename, wc.DownloadData(filename));
                    }

                }

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(this.smtp, this.port, this.useSSL);
                    client.Authenticate(this.fromMail, this.pwd);

                    string sServerResponse = client.Send(emailMessage);

                    logger.Info(sServerResponse);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.Source); logger.Error(ex.StackTrace);
            }

        }

        public bool SendReport(string regionName, string filename, int regionID, ReportType reportType)
        {
            bool bResult = false;

            try
            {
                if (!GetConfig())
                {
                    logger.Warn("Failed to get the mail server configuration to send the report");
                    return false;
                }

                List<MailingAddress> addresses = Controler.GetListMailing(reportType);
                if(addresses == null) {
                    logger.Warn("Failed to retrieve the list of report recipients");
                    return false;
                }

                _ = this.SendMailAsync(regionID, regionName, addresses, filename);






                bResult = true;
            }
            catch (Exception ex)
            { 

            }
            return bResult;
        }


    }
}