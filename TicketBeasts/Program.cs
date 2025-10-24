using Microsoft.EntityFrameworkCore;
using TicketBeasts.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// 1) MVC + "require login everywhere" (global AuthorizeFilter)
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// 2) Cookie authentication (sets redirect to Login when not signed in)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";      // redirect here if not logged in
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
    });

// 3) EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// If your project uses the new static asset helpers, keep these two lines:
app.MapStaticAssets();  // maps wwwroot (or static assets) before routing

app.UseRouting();

// 4) Auth order matters: UseAuthentication BEFORE UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// Default route — change if your home is different
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sports}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // auto-create/update tables on startup
}
app.MapGet("/health", () => "OK");


app.Run();
