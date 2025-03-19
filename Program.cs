using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Lab2.ShoppingWeb.CartFeature.Data;
using Lab2.ShoppingWeb.CartFeature.Data.Interfaces;
using Lab2.ShoppingWeb.CartFeature.Data.Repositories;
using Lab2.ShoppingWeb.CartFeature.Hubs;
using Lab2.ShoppingWeb.CartFeature.Middleware;
using Lab2.ShoppingWeb.CartFeature.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("wwwroot/firebase-config.json")
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"))
);
builder.Services.AddScoped<RedisService>();
// Add Firebase Auth Service
builder.Services.AddSingleton<FirebaseAuthService>();
// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
    });

builder.Services.AddAuthorization();

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

app.UseAuthorization();

//app.UseMiddleware<FirebaseAuthMiddleware>();

app.UseStatusCodePagesWithReExecute("/Home/CustomNotFound");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ProductHub>("/productHub");

app.Run();
