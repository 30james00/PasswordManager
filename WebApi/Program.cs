using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;
using Infrastructure;
using Microsoft.OpenApi.Models;
using PasswordManager.Application.Accounts;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var clientOrigin = "_clientOrigin";

var builder = WebApplication.CreateBuilder(args);

// Add CORS specification
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: clientOrigin,
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add Swagger specification
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Greengrocers",
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
        { securityScheme, System.Array.Empty<string>() }
    });
});

// Add services to the container.
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IHashService, HashService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IUserAccessor, UserAccessor>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

var app = builder.Build();

// Migrate Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
}

app.UseCors(clientOrigin);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();