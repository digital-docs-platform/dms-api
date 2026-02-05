using API.Auth.Cookies;

using API.Middleware;
using Application;
using Application.DocumentFields;
using Application.jwt;
using Application.Jwt;
using Application.PermissionHandling;
using Application.PermissionHandling.Resolver;
using Application.Security.Cryptography;
using Application.Seeding;
using Application.UseCaseHandling;
using Application.UseCaseHandling.CQReslover;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using Application.Validation;
using DataAccess;
using FluentValidation;
using Implementation.jwt;
using Implementation.PermissionHandling;
using Implementation.PermissionHandling.Resolver;
using Implementation.Querying.DocumentTypeFieldQuerying;
using Implementation.Querying.DocumentTypeFieldQuerying.Strategies;
using Implementation.Querying.FieldQuerying.Strategies;
using Implementation.Security.Cryptography;
using Implementation.Seeding;
using Implementation.UseCaseHandling;
using Implementation.UseCaseHandling.CQResolver;
using Implementation.UseCases.EntityFramework.Commands.Document;
using Implementation.UseCases.EntityFramework.Commands.DocumentType;
using Implementation.UseCases.EntityFramework.Commands.User;
using Implementation.UseCases.EntityFramework.Queries.Document;
using Implementation.UseCases.EntityFramework.Queries.DocumentType;
using Implementation.UseCases.EntityFramework.Queries.User;
using Implementation.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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
        p.WithOrigins("http://localhost:4200")
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

      //uzmi token iz HttpOnly cookie-ja
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

    var idValue =
        user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? user.FindFirst("sub")?.Value;

    if (!int.TryParse(idValue, out var userId))
        return new UnauthorizedActor();

    var email =
        user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("email")?.Value
        ?? "";


    return new JwtActor
    {
        Id = userId,
        Email = email
    };
});


//END OF JWT CONFIG SECTION

builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IAuthCookieService, AuthCookieService>();


builder.Services.AddScoped<IPermissionHandler, PermissionHandler>();
builder.Services.AddScoped<IPermissionProvider, PermissionProvider>();
builder.Services.AddScoped<IDocumentTypeResolver, DocumentTypeResolver>();


builder.Services.AddValidatorsFromAssemblyContaining<ApplicationMarker>();
builder.Services.AddScoped<IRequestValidation, RequestValidation>();

builder.Services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

builder.Services.AddScoped<IFieldValueMapper, FieldValueMapper>();

// COMMAND SERVICES

builder.Services.AddScoped<ICommandHandler, CommandHandler>();
builder.Services.AddScoped<ICommandResolver, CommandResolver>();



builder.Services.AddTransient<ICreateUserCommand, EFCreateUserCommand>();
builder.Services.AddTransient<ICreateDocumentTypeCommand, EFCreateDocumentTypeCommand>();
builder.Services.AddTransient<ICreateDocumentCommand, EFCreateDocumentCommand>();
builder.Services.AddTransient<IUpdateUserProfileCommand, EFUpdateUserProfileCommand>();

//End of command services



// QUERY SERVICES

builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IQueryResolver, QueryResolver>();

builder.Services.AddScoped<IFieldQueryDispatcher, FieldQueryDispatcher>();
builder.Services.AddScoped<IFieldQueryStrategy, TextFieldQueryStrategy>();
builder.Services.AddScoped<IFieldQueryStrategy, DecimalFieldQueryStrategy>();
builder.Services.AddScoped<IFieldQueryStrategy, NumberFieldQueryStrategy>();
builder.Services.AddScoped<IFieldQueryStrategy, DateFieldQueryStrategy>();
builder.Services.AddScoped<IFieldQueryStrategy, SelectFieldQueryStrategy>();



builder.Services.AddTransient<IGetMeQuery, EFGetMeQuery>();
builder.Services.AddTransient<IGetUserByIdQuery, EFGetUserByIdQuery>();
builder.Services.AddTransient<IGetDocumentTypeByIdQuery, EFGetDocumentTypeByIdQuery>();
builder.Services.AddTransient<IGetDocumentsByDocumentTypeIdQuery, EFGetDocumentsByDocumentTypeIdQuery>();
builder.Services.AddTransient<IGetDocumentByIdQuery,  EFGetDocumentByIdQuery>();

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

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedAsync();
}

app.Run();
