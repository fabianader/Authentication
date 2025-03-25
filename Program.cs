using IdentityProject.Context;
using IdentityProject.Models.AppCustomEntities;
using IdentityProject.Services;
using IdentityProject.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;
using DbContext = IdentityProject.Context.AppDbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = "8789649397-ptqtlbu11j9ejgpukffvjttn777fk0re.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        #region Options

        //User Options
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;

        //SignIn Options
        options.SignIn.RequireConfirmedEmail = true;
        options.SignIn.RequireConfirmedPhoneNumber = false;

        //Password Options
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 5;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;

        //LockOut Options
        options.Lockout.AllowedForNewUsers = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        //Sores Options
        options.Stores.ProtectPersonalData = false;

        #endregion
    })
    .AddEntityFrameworkStores<DbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<PersianIdentityErrors>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/LogOut";
});


builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
