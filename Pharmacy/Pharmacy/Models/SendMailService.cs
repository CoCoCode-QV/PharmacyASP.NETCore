using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit;
using Pharmacy.ViewsModels;

public class SendMailService {
    
    // Gửi email, theo nội dung trong mailContent
    public bool SendMail(string to, string subject, string body, string attachFile)
    {
        try
        {
           
            MailMessage message = new MailMessage(ViewMailSettingcs.EmailSender, to, subject, body);
            message.IsBodyHtml = true;
            using (var client = new SmtpClient(ViewMailSettingcs.Host, ViewMailSettingcs.Port))
            {
                client.EnableSsl = true;
                if(!string.IsNullOrEmpty(attachFile)) {
                    Attachment attachment = new Attachment(attachFile);
                    message.Attachments.Add(attachment);
                }
               
                NetworkCredential credential = new NetworkCredential(ViewMailSettingcs.EmailSender, ViewMailSettingcs.PasswordEmail);
                client.UseDefaultCredentials = false;
                client.Credentials = credential;
                client.Send(message);
             
            }
        }
        catch(Exception) 
        {
            return false;
        }
        return true;
    }

}