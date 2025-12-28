using API.jwt;
using API.Middleware;
using Application;
using Application.jwt;
using Application.Security.Cryptography;
using Application.UseCaseHandling;
using Application.UseCases.Commands;
using DataAccess;
using Implementation.jwt;
using Implementation.Security.Cryptography;
using Implementation.UseCases.EntityFramework.Commands.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultLocalConnection")));

builder.Services.AddAutoMapper(typeof(Program));



// My services

// JWT CONFIG SECTION

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));


var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
          ?? throw new InvalidOperationException("Jwt settings are missing.");

if (string.IsNullOrWhiteSpace(jwt.Key))
    throw new InvalidOperationException("Jwt:Key is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });






builder.Services.AddTransient<IJwtManager, JwtManager>();
builder.Services.AddScoped<IApplicationActor>(sp =>
{
    var http = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var user = http?.User;

    if (user?.Identity?.IsAuthenticated != true)
        return new UnauthorizedActor();

    var sub = user.FindFirst("sub")?.Value;
    if (!int.TryParse(sub, out var userId))
        return new UnauthorizedActor();

    return new JwtActor();
});


//END OF JWT CONFIG SECTION

builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();


// Command services

builder.Services.AddScoped<ICommandHandler, CommandHandler>();

builder.Services.AddTransient<ICreateUserCommand, CreateUserCommand>();

//End ov command services

//-------------------





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
