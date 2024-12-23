using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Demir.Services;
using System.IdentityModel.Tokens.Jwt;
using Demir.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=Demir.db"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
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
                context.Fail("Token is invalid.");
            }

            return Task.CompletedTask;
        }
    };
});




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
