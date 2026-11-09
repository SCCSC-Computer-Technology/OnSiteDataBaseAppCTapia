using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnSiteApp.Data;
using OnSiteApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Users sign in with a cookie. Pages that need a login send people to the login page.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/Index";
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //connection to DB
builder.Services.AddDbContext<OnSiteDatabase>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<OnSiteDatabase>();
    InitialDataSeeder.AddInitialData(database); //feeds data If empty
}

// Configure the HTTP 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
