using System.Net.Mail;

namespace WorkflowNotifications
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

                foreach (string item in to.Split(';'))
                {
                    try
                    {
                        mMailMessage.To.Add(new MailAddress(item));
                    }
                    catch 
                    { }
                }

                // Check if the bcc value is null or an empty string
                if ((bcc != null) && (bcc != string.Empty))
                {
                    // Set the Bcc address of the mail message
                    foreach (string item in bcc.Split(';'))
                    {
                        try
                        {
                            mMailMessage.Bcc.Add(new MailAddress(item));
                        }
                        catch 
                        {
                        }
                    }
                }      // Check if the cc value is null or an empty value
                if ((cc != null) && (cc != string.Empty))
                {
                    // Set the CC address of the mail message
                    foreach (string item in cc.Split(';'))
                    {
                        try
                        {
                            mMailMessage.CC.Add(new MailAddress(item));
                        }
                        catch 
                        {
                        }
                    }
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
            catch 
            {
                throw;
            }
        }
    }
}
