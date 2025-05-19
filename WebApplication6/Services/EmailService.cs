using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

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

    private IConfigurationSection GetActiveEmailConfig()
    {
        var defaultProvider = _config["EmailSettings:DefaultProvider"];
        return _config.GetSection($"EmailSettings:{defaultProvider}");
    }

    private async Task SendEmail(string email, string subject, string htmlBody)
    {
        var settings = GetActiveEmailConfig();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Pelican Magazine", settings["From"]));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(settings["SmtpServer"], int.Parse(settings["Port"]), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(settings["Username"], settings["Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendVerificationEmail(string email, string token)
    {
        var frontendUrl = _config["FrontendUrl"];
        var link = $"{frontendUrl}/verify-email?token={token}";
        var body = $"<h1>Подтверждение email</h1><p>Перейдите по <a href='{link}'>ссылке</a>.</p>";
        await SendEmail(email, "Подтверждение email", body);
    }

    public async Task SendTwoFactorCode(string email, string code)
    {
        var body = $"<h1>Код подтверждения</h1><p>Ваш код: <strong>{code}</strong></p>";
        await SendEmail(email, "Код 2FA", body);
    }
}
