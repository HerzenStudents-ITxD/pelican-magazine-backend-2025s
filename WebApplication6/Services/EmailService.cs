
using Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using OtpNet;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


public interface IEmailService
{
    Task SendVerificationEmail(string email, string token);
    Task SendTwoFactorCode(string email, string code);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendVerificationEmail(string email, string token)
    {
        try
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var verificationUrl = $"{_config["FrontendUrl"]}/verify-email?token={token}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Pelican Magazine", emailSettings["From"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Подтвердите ваш email";

            message.Body = new TextPart("html")
            {
                Text = $"<h1>Подтверждение email</h1><p>Пожалуйста, подтвердите ваш email, перейдя по <a href='{verificationUrl}'>ссылке</a>.</p>"
            };

            using var client = new SmtpClient();

            Console.WriteLine($"Попытка подключения к SMTP: {emailSettings["SmtpServer"]}:{emailSettings["Port"]}");

            await client.ConnectAsync(
                emailSettings["SmtpServer"],
                int.Parse(emailSettings["Port"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                emailSettings["Username"],
                emailSettings["Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки email: {ex}");
            throw; // Перебросить исключение дальше или обработать
        }
    }

    public async Task SendTwoFactorCode(string email, string code)
    {
        // Аналогичная реализация для отправки кода 2FA
    }
}