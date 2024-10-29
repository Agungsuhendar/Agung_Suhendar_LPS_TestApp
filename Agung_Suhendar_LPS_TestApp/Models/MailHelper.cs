using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;


namespace Agung_Suhendar_LPS_TestApp.Models
{
        //Created By to Sending Email
        public class SMTPProperties
        {
            public int SMTP_Port { get; set; }
            public string SMTP_Host { get; set; }
            public object? SMTP_SSL { get; set; }
            public bool? IsUseSSL { get; set; }
            public string AccountMailAddress { get; set; }
            public string AccountName { get; set; }
            public string Password { get; set; }

        }

        public class MailProperties
        {
            public string From { get; set; }
            public string To { get; set; }
            public string? CC { get; set; }
            public string? BCC { get; set; }
            public string SubjectEmail { get; set; }
            public string? BodyEmail { get; set; }
            public bool? IsHTML { get; set; }
            public SMTPProperties? SMTPProperties { get; set; }
            public string? Messages { get; set; }
            public bool? IsError { get; set; }
            public bool? IsSent { get; set; }
        }

        public class MailSending
        {
            public MailProperties Send(MailProperties mailProperties)
            {
                MailProperties xResult = new MailProperties();
                if (mailProperties == null || mailProperties.SMTPProperties == null)
                {
                    xResult.IsError = true;
                    xResult.Messages = "Data SMTP Not Found!";
                    xResult.IsSent = false;
                    xResult.SMTPProperties = null;
                    return xResult;
                }
                else
                {
                    try
                    {
                        // create message
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(mailProperties.From));
                        string[] ListTo = mailProperties.To.Split(";");
                        foreach (string itemTo in ListTo)
                        {
                            System.Net.Mail.MailAddress xmailAddress;
                            if (System.Net.Mail.MailAddress.TryCreate(itemTo, out xmailAddress!))
                            {
                                email.To.Add(MailboxAddress.Parse(itemTo));
                            }
                        }
                        if (mailProperties.CC != null && mailProperties.CC != "")
                        {
                            // email.Cc.Add(MailboxAddress.Parse(mailProperties.CC));
                            string[] ListCC = mailProperties.CC.Split(";");
                            foreach (string itemCC in ListCC)
                            {
                                // email.Cc.Add(MailboxAddress.Parse(itemCC));
                                System.Net.Mail.MailAddress xmailAddress;
                                if (System.Net.Mail.MailAddress.TryCreate(itemCC, out xmailAddress!))
                                {
                                    email.To.Add(MailboxAddress.Parse(itemCC));
                                }
                            }
                        }
                        if (mailProperties.BCC != null && mailProperties.BCC != "")
                        {
                            // email.Bcc.Add(MailboxAddress.Parse(mailProperties.BCC));
                            string[] ListBCC = mailProperties.BCC.Split(";");
                            foreach (string itemBCC in ListBCC)
                            {
                                // email.Bcc.Add(MailboxAddress.Parse(itemBCC));
                                System.Net.Mail.MailAddress xmailAddress;
                                if (System.Net.Mail.MailAddress.TryCreate(itemBCC, out xmailAddress!))
                                {
                                    email.To.Add(MailboxAddress.Parse(itemBCC));
                                }
                            }
                        }
                        email.Subject = mailProperties.SubjectEmail;
                        if (mailProperties.IsHTML.HasValue && mailProperties.IsHTML.Value == true)
                        {
                            email.Body = new TextPart(TextFormat.Html) { Text = mailProperties.BodyEmail != null ? mailProperties.BodyEmail : "" };
                        }
                        if (!mailProperties.IsHTML.HasValue || (mailProperties.IsHTML.HasValue && mailProperties.IsHTML.Value == false))
                        {
                            email.Body = new TextPart(TextFormat.Plain) { Text = mailProperties.BodyEmail != null ? mailProperties.BodyEmail : "" };
                        }
                        // send email
                        using var smtp = new SmtpClient();
                        SMTPProperties xSMTP = mailProperties.SMTPProperties;
                        smtp.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                        smtp.Connect(xSMTP.SMTP_Host, xSMTP.SMTP_Port, SecureSocketOptions.StartTls);
                        // smtp.Connect(xSMTP.SMTP_Host, xSMTP.SMTP_Port, xSMTP.IsUseSSL.HasValue?xSMTP.IsUseSSL.Value:false);
                        // smtpClient.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                        smtp.Authenticate(xSMTP.AccountName, xSMTP.Password);
                        smtp.Send(email);
                        smtp.Disconnect(true);
                        xResult.IsError = false;
                        xResult.IsSent = true;
                    }
                    catch (Exception ex)
                    {
                        xResult.IsError = true;
                        xResult.Messages = ex.Message;
                        xResult.IsSent = false;
                        xResult.SMTPProperties = null;
                        return xResult;
                    }
                }
                xResult.SMTPProperties = null;
                return xResult;
            }
        }
}
