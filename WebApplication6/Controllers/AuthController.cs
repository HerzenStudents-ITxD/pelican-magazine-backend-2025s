using Backend.Contracts.Requests.Auth;
using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OtpNet;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Backend.Contracts.Responses;


namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly JwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    public AuthController(
        UserRepository userRepository,
        JwtService jwtService,
        IEmailService emailService,
        IConfiguration config,
        IWebHostEnvironment env)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
        _config = config;
        _env = env;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Проверка существования пользователя
        var exists = await _userRepository.GetByEmailAsync(request.Email);
        if (exists != null)
            return Conflict("Email уже зарегистрирован");

        // Хеширование пароля
        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password);

        // Создание пользователя
        var user = new DbUser
        {
            UserId = Guid.NewGuid(),
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Birth = request.Birth,
            Sex = request.Sex,
            Sec = Guid.NewGuid().ToString(),
            EmailVerificationToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1),
            EmailVerified = false
        };

        await _userRepository.AddAsync(user);

        // Отправка email для подтверждения
        await _emailService.SendVerificationEmail(user.Email, user.EmailVerificationToken);

        return Ok(new { Message = "Регистрация успешна. Проверьте email для подтверждения." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized("Неверный email или пароль");

        // Проверка пароля
        var valid = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash);
        if (!valid)
            return Unauthorized("Неверный email или пароль");

        // Проверка подтверждения email
        if (!user.EmailVerified)
            return BadRequest("Email не подтвержден");

        // Проверка 2FA
        if (user.TwoFactorEnabled)
        {
            if (string.IsNullOrEmpty(request.TwoFactorCode))
            {
                // Генерируем 6-значный код
                var code = new Random().Next(100000, 999999).ToString();

                // Сохраняем временно в поле TwoFactorSecret
                user.TwoFactorSecret = code;
                user.TwoFactorSecretExpiry = DateTime.UtcNow.AddMinutes(10); // добавь это поле в модель
                await _userRepository.UpdateAsync(user);

                // Отправляем код
                await _emailService.SendTwoFactorCode(user.Email, code);

                return BadRequest(new { Requires2fa = true });
            }

            // Проверка кода
            if (user.TwoFactorSecret != request.TwoFactorCode || user.TwoFactorSecretExpiry < DateTime.UtcNow)
            {
                return Unauthorized("Неверный или просроченный код 2FA");
            }
        }



        // Генерация токена
        var token = _jwtService.GenerateToken(user);

        return Ok(new { Token = token });

    }

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        await _emailService.SendVerificationEmail("твой_email@mail.ru", "FAKE_TOKEN");
        return Ok("Письмо отправлено");
    }


    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
    {
        var user = await _userRepository.GetByVerificationTokenAsync(request.Token);
        if (user == null || user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            return BadRequest("Неверный или просроченный токен");

        user.EmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;

        await _userRepository.UpdateAsync(user);

        return Ok(new { Message = "Email успешно подтвержден" });
    }

    [HttpPost("enable-2fa")]
    [Authorize]
    public async Task<IActionResult> Enable2fa(Enable2faRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        if (user == null)
            return NotFound("Пользователь не найден");

        // Проверка кода
        var isValid = VerifyTwoFactorCode(user.TwoFactorSecret, request.Code);
        if (!isValid)
            return BadRequest("Неверный код");

        user.TwoFactorEnabled = true;
        await _userRepository.UpdateAsync(user);

        return Ok(new { Message = "2FA успешно включена" });
    }

    [HttpGet("setup-2fa")]
    [Authorize]
    public async Task<IActionResult> Setup2fa()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        if (user == null)
            return NotFound("Пользователь не найден");

        // Генерация секрета для 2FA
        var secret = Guid.NewGuid().ToString("N").Substring(0, 16);
        user.TwoFactorSecret = secret;
        await _userRepository.UpdateAsync(user);

        // Генерация QR-кода (можно использовать библиотеку QRCoder)
        var issuer = _config["Jwt:Issuer"];
        var qrCodeSetup = $"otpauth://totp/{issuer}:{user.Email}?secret={secret}&issuer={issuer}";

        return Ok(new
        {
            Secret = secret,
            QrCodeSetup = qrCodeSetup
        });
    }

    [HttpPost("verify-2fa")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify2Fa([FromBody] Verify2FaRequest request)
    {
        // 1. Находим пользователя по email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { Message = "User not found" });

        // 2. Проверяем, что 2FA включена для пользователя
        if (!user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            return BadRequest(new { Message = "2FA is not enabled for this user" });

        // 3. Проверяем код
        var isValid = VerifyTwoFactorCode(user.TwoFactorSecret, request.Code);
        if (!isValid)
            return Unauthorized(new { Message = "Invalid 2FA code" });

        // 4. Генерируем токен
        var token = _jwtService.GenerateToken(user);

        // 5. Возвращаем токен
        return Ok(new
        {
            Token = token,
            User = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email
            }
        });
    }

    private bool VerifyTwoFactorCode(string secret, string code)
    {
        try
        {
            var keyBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(keyBytes);
            return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
        }
        catch
        {
            return false;
        }
    }

    [HttpPost("init-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> InitAdmin([FromQuery] string secretKey)
    {
        if (secretKey != _config["AdminInitSecret"])
            return Forbid();

        var admin = await _userRepository.GetByEmailAsync("admin@example.com");
        if (admin != null)
        {
            admin.IsAdmin = true;
            await _userRepository.UpdateAsync(admin);
        }

        return Ok();
    }


}
