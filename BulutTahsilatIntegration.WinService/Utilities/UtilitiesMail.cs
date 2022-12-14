using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using BulutTahsilatIntegration.WinService.Core;
using BulutTahsilatIntegration.WinService.Model.ResultTypes;

namespace BulutTahsilatIntegration.WinService.Utilities
{
    public class UtilitiesMail
    {
        public static bool SendMailTest(string to)
        {
            bool flag;
            var setting = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath);
            _ = setting.MailPassword.DecryptIt();
            using (var messages = new MailMessage()
            {
                To = { to },
                From = new MailAddress(setting.MailFrom),
                Subject = "Test",
                Body = "Test",
                IsBodyHtml = true,
            })
                try
                {
                    var smtp = new SmtpClient(setting.MailHost, setting.MailPort)
                    {
                        Credentials = new NetworkCredential(setting.MailUserName, setting.MailPassword.DecryptIt()),
                        EnableSsl = setting.MailSsl,
                    };
                    smtp.Send(messages);
                    flag = true;
                }
                catch (Exception)
                {
                    return flag = false;
                }

            return flag;
        }
        public static ServiceResult SendMail(string toEmail, string bcc = "", string cc = "", string subject = "", string body = "")
        {
            var result = new ServiceResult();
            var setting = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath);

            try
            {
                var mailMessages = new MailMessage();
                char[] Splitter = { ';' };
                string[] AddressCollection = toEmail.Split(Splitter);
                for (int x = 0; x < AddressCollection.Length; ++x)
                {
                    mailMessages.To.Add(AddressCollection[x]);
                }
                
                mailMessages.From = new MailAddress(setting.MailFrom);
                mailMessages.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(bcc))
                {
                    mailMessages.Bcc.Add(new MailAddress(bcc));
                }
                if (!string.IsNullOrEmpty(cc))
                {
                    mailMessages.CC.Add(new MailAddress(cc));
                }
                if (!string.IsNullOrEmpty(subject))
                {
                    mailMessages.Subject = subject;
                }
                else
                {
                    mailMessages.Subject = "Bulut Tahsilat ödeme hareketleri bilgilendirme servisi"; ;
                }
                mailMessages.Body = body;
                mailMessages.Priority = MailPriority.Normal;

                var smtp = new SmtpClient(setting.MailHost, setting.MailPort)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(setting.MailUserName, setting.MailPassword.DecryptIt()),
                    EnableSsl = setting.MailSsl,
                };
                smtp.Send(mailMessages);
                result.Success = true;
            }
            catch (Exception exception)
            {
                result.Success = false;
                result.ErrorDesc = exception.Message;
            }

            return result;
        }
        public static ServiceResult SendMail(string toEmail, string bcc, string cc, string subject, string body, List<string> attachmentFullPath)
        {
            var result = new ServiceResult();
            var setting = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath);

            try
            {
                var mailMessages = new MailMessage();

                mailMessages.To.Add(new MailAddress(toEmail));
                mailMessages.From = new MailAddress(setting.MailFrom);
                mailMessages.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(bcc))
                {
                    mailMessages.Bcc.Add(new MailAddress(bcc));
                }
                if (!string.IsNullOrEmpty(cc))
                {
                    mailMessages.CC.Add(new MailAddress(cc));
                }
                if (!string.IsNullOrEmpty(subject))
                {
                    mailMessages.Subject = subject;
                }
                else
                {
                    mailMessages.Subject = "Bulut Tahsilat ödeme hareketleri bilgilendirme servisi"; ;
                }
                foreach (var attachmentPath in attachmentFullPath)
                {
                    Attachment mailAttachment = new Attachment(attachmentPath);
                    mailMessages.Attachments.Add(mailAttachment);
                }

                mailMessages.Body = body;
                mailMessages.Priority = MailPriority.Normal;


                var smtp = new SmtpClient(setting.MailHost, setting.MailPort)
                {
                    Credentials = new NetworkCredential(setting.MailUserName, setting.MailPassword.DecryptIt()),
                    EnableSsl = setting.MailSsl,
                };
                smtp.Send(mailMessages);
                result.Success = true;
            }
            catch (Exception exception)
            {
                result.Success = false;
                result.ErrorDesc = exception.Message;
            }

            return result;
        }
        public static ServiceResult SendMail(string toEmail, string bcc, string cc, string subject, string body, MemoryStream attachmentFile, string fileName = null)
        {
            var result = new ServiceResult();
            var setting = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath);

            try
            {
                var mailMessages = new MailMessage();

                mailMessages.To.Add(new MailAddress(toEmail));
                mailMessages.From = new MailAddress(setting.MailFrom);
                mailMessages.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(bcc))
                {
                    mailMessages.Bcc.Add(new MailAddress(bcc));
                }
                if (!string.IsNullOrEmpty(cc))
                {
                    mailMessages.CC.Add(new MailAddress(cc));
                }
                mailMessages.Subject = "Bulut Tahsilat ödeme hareketleri bilgilendirme servisi";

                attachmentFile.Seek((long)0, SeekOrigin.Begin);
                var attachment = new Attachment(attachmentFile, string.Concat(fileName ?? Guid.NewGuid().ToString(), ".pdf"), "application/pdf");
                mailMessages.Attachments.Add(attachment);
                mailMessages.Body = body;
                mailMessages.Priority = MailPriority.Normal;


                var smtp = new SmtpClient(setting.MailHost, setting.MailPort)
                {
                    Credentials = new NetworkCredential(setting.MailUserName, setting.MailPassword.DecryptIt()),
                    EnableSsl = setting.MailSsl,
                };
                smtp.Send(mailMessages);
                result.Success = true;
                result.Description = @"Mail gönderimi başarılı";
            }
            catch (Exception exception)
            {
                result.Success = false;
                result.ErrorDesc = exception.Message;
            }

            return result;
        }
        /// <summary>
        /// Determines whether an email address is valid.
        /// </summary>
        /// <param name="emailAddress">The email address to validate.</param>
        /// <returns>
        /// 	<c>true</c> if the email address is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmailAddress(string emailAddress)
        {
            // An empty or null string is not valid
            if (String.IsNullOrEmpty(emailAddress))
            {
                return (false);
            }

            // Regular expression to match valid email address
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            // Match the email address using a regular expression
            Regex re = new Regex(emailRegex);
            if (re.IsMatch(emailAddress))
                return (true);
            else
                return (false);
        }
    }
}
