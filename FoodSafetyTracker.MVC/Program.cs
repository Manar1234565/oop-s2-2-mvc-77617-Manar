using FoodSafetyTracker.MVC.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// 🔥 SERILOG CONFIG
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "FoodSafetyTracker")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// MVC + Razor Pages (Identity UI)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=foodsafety.db"));

// 🔥 IDENTITY + ROLES
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultUI();

var app = builder.Build();

// 🔥 LOG START
Log.Information("Application started");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 AUTH
app.UseAuthentication();
app.UseAuthorization();

// Identity pages
app.MapRazorPages();

// ROUTES
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// 🔥 SEED DATA (ROLES + ADMIN + DATA)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();