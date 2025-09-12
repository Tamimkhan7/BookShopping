using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
//email send korar jonno .net er built in library use korbo
using System.Net;
using System.Net.Mail;

namespace BookShopping.Utility
{
    public class EmailSender : IEmailSender
    {
        //appsettings.json, environment variables, command-line args just fonfiguration system er part, ja amra easily access korte pari
        //Purpose: configuration system access kora, jemon SMTP username/password।
        private readonly IConfiguration _configuration; //ja asp.net code configuration  system main, aita directly run hoy na framework a configuration acccess korar jonno use hoy

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //this method Identity interface ar requirement।
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //appsettings.json a "Smtp" object access kora
            var smtpSection = _configuration.GetSection("Smtp"); //this is key value pair, jekhane smtp holo key, ar tar value gula holo object
            var username = smtpSection.GetValue<string>("Username");
            var password = smtpSection.GetValue<string>("Password");

            //built -in class email pathanor jonno "SmtpClient"
            //smtp.gmail.com → Gmail server. Port = 587 → TLS/STARTTLS port. TLS → Transport Layer Security,,,STARTTLS → “Start Transport Layer Security”
            //TLS - Eta ekta protocol ja data encryption kore, mane communication secure hoy. Email, web, VPN, etc. e use hoy.
            //Mane port 587 normally plain SMTP e start hoy, tarpor STARTTLS diye encryption on kore.
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(username, password), //login credentials for Gmail SMTP.
                EnableSsl = true, //secure connection
            };

            //email content prepare kore
            var mailMessage = new MailMessage
            {
                From = new MailAddress(username, "BookShopping"), //sender email address
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true, //HTML content allowed।
            };
            //sender ar receiver email address add kora
            mailMessage.To.Add(email);

            //email pathano hoy Gmail SMTP server diye
            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}


//Identity integration:

//ForgotPassword / ResetPassword call → uses this EmailSender.SendEmailAsync() to send reset token.