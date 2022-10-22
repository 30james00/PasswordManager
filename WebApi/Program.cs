using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Security.Hash;
using PasswordManager.Application.Security.Token;
using Infrastructure;
using PasswordManager.Application.Accounts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IHashService, HashService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IUserAccessor, UserAccessor>();

var app = builder.Build();

// Migrate Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
}

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