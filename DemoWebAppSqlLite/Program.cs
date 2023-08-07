using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DemoWebAppSqlLite.Data;
using DemoWebAppSqlLite.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentitySqlLiteContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentitySqlLiteContextConnection' not found.");

builder.Services.AddDbContext<IdentitySqlLiteContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentitySqlLiteContext>();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

app.Run();
