﻿using System;
using System.Linq;
using System.Net.Mail;
namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class MailHelper
    {
        /// <summary>
        /// Sends an mail message
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recepient address</param>
        /// <param name="bcc">Bcc recepient</param>
        /// <param name="cc">Cc recepient</param>
        /// <param name="subject">Subject of mail message</param>
        /// <param name="body">Body of mail message</param>
        public static void SendMailMessage(string from, string to, string bcc, string cc, string subject, string body)
        {
            try
            {
                // Instantiate a new instance of MailMessage
                MailMessage mMailMessage = new MailMessage();

                // Set the sender address of the mail message
                mMailMessage.From = new MailAddress(from);
                // Set the recepient address of the mail message
                mMailMessage.To.Add(new MailAddress(to));

                // Check if the bcc value is null or an empty string
                if ((bcc != null) && (bcc != string.Empty))
                {
                    // Set the Bcc address of the mail message
                    mMailMessage.Bcc.Add(new MailAddress(bcc));
                }      // Check if the cc value is null or an empty value
                if ((cc != null) && (cc != string.Empty))
                {
                    // Set the CC address of the mail message
                    mMailMessage.CC.Add(new MailAddress(cc));
                }       // Set the subject of the mail message
                mMailMessage.Subject = subject;
                // Set the body of the mail message
                mMailMessage.Body = body;

                // Set the format of the mail message body as HTML
                mMailMessage.IsBodyHtml = true;
                // Set the priority of the mail message to normal
                mMailMessage.Priority = MailPriority.Normal;

                // Instantiate a new instance of SmtpClient
                SmtpClient mSmtpClient = new SmtpClient();
                // Send the mail message
                mSmtpClient.Send(mMailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SendMail(string from, string to, string bcc, string cc, string subject, string body, string file)
        {
            bool IsSuccess = true;
            try
            {
              

                // Instantiate a new instance of MailMessage
                MailMessage mMailMessage = new MailMessage();

                // Set the sender address of the mail message
                mMailMessage.From = new MailAddress(from);
                // Set the recepient address of the mail message
                //mMailMessage.To.Add(new MailAddress(to));

                if ((to != null) && (to != string.Empty))
                {
                    if (to.Contains(','))
                    {
                        mMailMessage.To.Add(to);
                    }
                    else
                    {
                        // Set the CC address of the mail message
                        mMailMessage.To.Add(new MailAddress(to));
                    }
                }       

                // Check if the bcc value is null or an empty string
                if ((bcc != null) && (bcc != string.Empty))
                {
                    // Set the Bcc address of the mail message
                    mMailMessage.Bcc.Add(new MailAddress(bcc));
                }      // Check if the cc value is null or an empty value
                if ((cc != null) && (cc != string.Empty))
                {
                    if (cc.Contains(','))
                    {
                        mMailMessage.CC.Add(cc);
                    }
                    else
                    {
                        // Set the CC address of the mail message
                        mMailMessage.CC.Add(new MailAddress(cc));
                    }
                }       // Set the subject of the mail message
                mMailMessage.Subject = subject;
                // Set the body of the mail message
                mMailMessage.Body = body;

                // Set the format of the mail message body as HTML
                mMailMessage.IsBodyHtml = true;
                // Set the priority of the mail message to normal
                mMailMessage.Priority = MailPriority.Normal;


                if (file.Length > 0)
                {
                    //Attaching Document
                    mMailMessage.Attachments.Add(new Attachment(file));
                }


                // Instantiate a new instance of SmtpClient
                SmtpClient mSmtpClient = new SmtpClient();

                // Send the mail message
                mSmtpClient.Send(mMailMessage);

                
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            return IsSuccess;
        }
    }
}
