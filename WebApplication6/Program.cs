using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Backend.DataAccess;
using Backend.Repositories;
using Backend.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:3172")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



// Регистрация репозиториев
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ArticleRepository>();
builder.Services.AddScoped<AgeCategoryRepository>();
builder.Services.AddScoped<ArticleAgeCategoryRepository>();
builder.Services.AddScoped<ArticleReviewRepository>();
builder.Services.AddScoped<ArticleThemeRepository>();
builder.Services.AddScoped<ThemeRepository>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<LikeRepository>();
builder.Services.AddScoped<ArticleModerationRepository>();

// Настройка аутентификации
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

// Rate limiting
builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("auth", config => {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
    });
});

// Регистрация контекста БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactDevClient");
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

// Автоматическое создание БД (без миграций)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.Run();