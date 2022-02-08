using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Nedrech.WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

MySqlConnectionStringBuilder conStrBuilder =
    new(builder.Configuration.GetConnectionString("IdentityConnection"))
{
    Server = builder.Configuration["DbServer"],
    UserID = builder.Configuration["DbUser"],
    Password = builder.Configuration["DbPassword"],
    Database = builder.Configuration["DbName"]
};

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseMySql(conStrBuilder.ConnectionString,
        new MariaDbServerVersion(new Version(10, 6, 5))));

builder.Services.AddAuthorization();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.Zero);

var app = builder.Build();

app.UseRequestLocalization(opts => {
    opts.AddSupportedCultures("en-US")
        .AddSupportedUICultures("en-US")
        .SetDefaultCulture("en-US");
});

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

IdentityData.EnsureMigrated(app);

app.Run();