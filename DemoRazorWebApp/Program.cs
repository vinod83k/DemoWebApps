using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DemoRazorWebApp.Data;
using DemoRazorWebApp.Areas.Identity.Models;
using DemoRazorWebApp.Areas.Identity.Data;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var connectionString = 
    builder.Configuration.GetConnectionString("IdentityContextConnection") 
        ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

//builder.Services.AddDbContext<IdentityContext>(options =>
//    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<IdentityContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityContextConnection")));

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // add 10 seconds delay to ensure the db server is up to accept connections
        // this won't be needed in real world application
        System.Threading.Thread.Sleep(10000);
        var context = services.GetRequiredService<IdentityContext>();
        var created = context.Database.EnsureCreated();

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        Console.WriteLine(ex.Message);
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<IdentityContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ContextSeed.SeedRolesAsync(userManager, roleManager);
        await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}


app.Run();

