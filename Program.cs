using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Demir.Services;
using Demir.Middlewares;
using Demir.Services.Contracts;
using Demir.Constants;


var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<UserAgentMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Configure database context
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite("Data Source=Demir.db"));

    // Configure application services
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IBalanceService, BalanceService>();
    services.AddScoped<ITokenService, TokenService>();

    // Configure JWT authentication
    var jwtSettings = configuration.GetSection("JwtSettings");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context => 
            {
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();

                var token = context.HttpContext.Request.Headers["Authorization"].ToString();
                var isTokenBlocked = tokenService.IsTokenExist(token);

                if (isTokenBlocked)
                {
                    context.Fail(Messages.InvalidToken);
                }

                return Task.CompletedTask;
            }
        };
    });
}