using System.Text;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;
using Infrastructure;
using MediatR;
using Microsoft.OpenApi.Models;
using PasswordManager.Application.Accounts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using PasswordManager.Application;
using PasswordManager.Application.Core;
using PasswordManager.Application.LoginAttempts;
using PasswordManager.Application.Security.Crypto;
using PasswordManager.Middleware;

var clientOrigin = "_clientOrigin";

var builder = WebApplication.CreateBuilder(args);

// Add CORS specification
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: clientOrigin,
        policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddTransient<IAccountService, AccountService>();

builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IHashService, HashService>();
builder.Services.AddTransient<ICryptoService, CryptoService>();

builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddScoped<ILoginAttemptsService, LoginAttemptsService>();

builder.Services.AddControllers(opt =>
{
    // Add authorization to controllers
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

// Add Swagger specification
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PasswordManager",
        Version = "v1"
    });

    // Add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddMediatR(typeof(MediatREntrypoint).Assembly);

builder.Services.AddDbContext<DataContext>(options =>
    // Configure SQLite database
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]));

//Configure Authentication and add Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Migrate Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
}

// Activate CORS
app.UseCors(clientOrigin);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();