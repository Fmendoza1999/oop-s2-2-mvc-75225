using FoodSafetyTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// Configure Serilog first
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "FoodSafetyTracker")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
        "{Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] " +
            "App={Application} Env={EnvironmentName} User={UserName} " +
            "{Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting FoodSafetyTracker application");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for all logging
    builder.Host.UseSerilog();

    // Add services
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // MySQL Database - fixed version instead of AutoDetect to avoid hanging
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

    // Identity
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseMiddleware<FoodSafetyTracker.Middleware.ExceptionMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    // Seed database
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            await DbSeeder.SeedAsync(scope.ServiceProvider);
            Log.Information("Database seeded successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error seeding database");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}