using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using ICC.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ICC.Predictor.Library.AWS
{
    public class SES : S3, Interfaces.AWS.IAWS
    {
        private static string _DisplayName
        { get { return "ICC Predictor"; } }
        private static string _AttachmentName
        { get { return "Daily-data.pdf"; } }
        private static RegionEndpoint _AWSSESRegion;
        private static SMTP _SMTP;

        public SES(IOptions<Application> appSettings) : base(appSettings)
        {
            _AWSSESRegion = RegionEndpoint.USEast1;
            _SMTP = appSettings.Value.SMTP;
        }

        private static string _Encoding { get { return "UTF-8"; } }

        public async Task<bool> SendSESMail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml, byte[] attachment = null)
        {
            if (attachment != null)
                return await SendSESAttachmentMail(from, to, cc, bcc, subject, msg, isHtml, attachment);
            else
                return await SendSESTextMail(from, to, cc, bcc, subject, msg, isHtml);
        }

        #region " Private "

        private async Task<bool> SendSESTextMail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml)
        {
            bool success = false;

            try
            {
                List<string> toRecipients = new List<string>();
                List<string> ccRecipients = new List<string>();
                List<string> bccRecipients = new List<string>();

                if (to.Length > 0)
                {
                    string[] myTo = to.Split(';');
                    foreach (string mTo in myTo)
                    {
                        if (mTo.Trim() != "")
                            toRecipients.Add(mTo);
                    }
                }

                if (cc.Length > 0)
                {
                    string[] myCc = cc.Split(';');
                    foreach (string mCc in myCc)
                    {
                        if (mCc.Trim() != "")
                            ccRecipients.Add(mCc);
                    }
                }

                if (bcc.Length > 0)
                {
                    string[] myBcc = bcc.Split(';');
                    foreach (string mBcc in myBcc)
                    {
                        if (mBcc.Trim() != "")
                            bccRecipients.Add(mBcc);
                    }
                }

                Body body = new Body();

                if (isHtml)
                    body.Html = new Content
                    {
                        Charset = _Encoding,
                        Data = msg
                    };
                else
                    body.Text = new Content
                    {
                        Charset = _Encoding,
                        Data = msg
                    };

                using (var client = SESClient())
                {
                    var sendRequest = new SendEmailRequest
                    {
                        Source = from,

                        Destination = new Destination
                        {
                            ToAddresses = toRecipients,
                            CcAddresses = ccRecipients,
                            BccAddresses = bccRecipients
                        },

                        Message = new Message
                        {
                            Subject = new Content(subject),
                            Body = body
                        }
                    };

                    var response = await client.SendEmailAsync(sendRequest);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        success = true;
                    else
                        success = false;
                }
            }
            catch (Exception ex)
            {
            }

            return success;
        }

        private async Task<bool> SendSESAttachmentMail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml, byte[] attachment = null)
        {
            bool success = false;

            try
            {
                MimeMessage message = new MimeMessage();

                if (to.Length > 0)
                {
                    string[] myTo = to.Split(';');
                    foreach (string mTo in myTo)
                    {
                        if (mTo.Trim() != "")
                            message.To.Add(new MailboxAddress(string.Empty, mTo));
                    }
                }

                if (cc.Length > 0)
                {
                    string[] myCc = cc.Split(';');
                    foreach (string mCc in myCc)
                    {
                        if (mCc.Trim() != "")
                            message.Cc.Add(new MailboxAddress(string.Empty, mCc));
                    }
                }

                if (bcc.Length > 0)
                {
                    string[] myBcc = bcc.Split(';');
                    foreach (string mBcc in myBcc)
                    {
                        if (mBcc.Trim() != "")
                            message.Bcc.Add(new MailboxAddress(string.Empty, mBcc));
                    }
                }

                BodyBuilder body = new BodyBuilder();

                if (isHtml)
                    body.HtmlBody = msg;
                else
                    body.TextBody = msg;

                if (attachment != null)
                    body.Attachments.Add(_AttachmentName, attachment);

                message.From.Add(new MailboxAddress(_DisplayName, from));
                message.Subject = subject;
                message.Body = body.ToMessageBody();

                MemoryStream stream = new MemoryStream();
                message.WriteTo(stream);

                using (var client = SESClient())
                {
                    var sendRequest = new SendRawEmailRequest { RawMessage = new RawMessage(stream) };

                    var response = await client.SendRawEmailAsync(sendRequest);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        success = true;
                    else
                        success = false;
                }
            }
            catch (Exception ex)
            {
            }

            return success;
        }

        private bool SendSMTPTextMail(string from, string to, string cc, string bcc, string subject, string msg, bool isHtml)
        {
            bool success = false;

            try
            {
                MailMessage message = new MailMessage();

                //to
                if (to.Length > 0)
                {
                    string[] myTo = to.Split(';');
                    foreach (string mTo in myTo)
                    {
                        if (mTo.Trim() != "")
                            message.To.Add(new MailAddress(mTo));
                    }
                }

                // CC.
                if (cc.Length > 0)
                {
                    string[] myCC = cc.Split(';');
                    foreach (string ccCopy in myCC)
                    {
                        if (ccCopy.Trim() != "")
                            message.CC.Add(new MailAddress(ccCopy));
                    }
                }

                // BCC.
                if (bcc.Length > 0)
                {
                    string[] myBCC = bcc.Split(';');
                    foreach (string bccHide in myBCC)
                    {
                        if (bccHide.Trim() != "")
                            message.Bcc.Add(new MailAddress(bccHide));
                    }
                }

                message.From = new MailAddress(from, _DisplayName);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = isHtml;
                message.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient(_SMTP.Host, _SMTP.Port);
                smtp.Credentials = new System.Net.NetworkCredential(_SMTP.Username, _SMTP.Password);
                smtp.EnableSsl = true;
                smtp.Timeout = 15000;

                smtp.Send(message);
                success = true;
            }
            catch (Exception ex)
            {
            }

            return success;
        }

        #endregion " Private "
    }
}