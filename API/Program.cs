using API.Auth.Cookies;
using API.Auth.jwt;
using API.Middleware;
using Application;
using Application.jwt;
using Application.PermissionHandling;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("client", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});



// My services

// JWT CONFIG SECTION

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));


var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
          ?? throw new InvalidOperationException("Jwt settings are missing.");

if (string.IsNullOrWhiteSpace(jwt.Key))
    throw new InvalidOperationException("Jwt:Key is missing.");

var cookieName = builder.Configuration["AuthCookie:Name"] ?? "dms_at";

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

      // ključni deo: uzmi token iz HttpOnly cookie-ja
      options.Events = new JwtBearerEvents
      {
          OnMessageReceived = context =>
          {
              if (context.Request.Cookies.TryGetValue(cookieName, out var token))
                  context.Token = token;

              return Task.CompletedTask;
          }
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
builder.Services.AddScoped<IAuthCookieService, AuthCookieService>();
builder.Services.AddScoped<IPermissionHandler, PermissionHandler>();


// COMMAND SERVICES

builder.Services.AddScoped<ICommandHandler, CommandHandler>();

builder.Services.AddTransient<ICreateUserCommand, CreateUserCommand>();

//End of command services



// QUERY SERVICES

builder.Services.AddScoped<IQueryHandler, QueryHandler>();

//END OF QUERY SERVICES

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

app.UseCors("client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
