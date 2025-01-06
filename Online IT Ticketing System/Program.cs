using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
        });


// Add distributed memory cache for session storage
builder.Services.AddDistributedMemoryCache();

// Configure session options
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout to 30 minutes
    options.Cookie.HttpOnly = true;                // Protect cookies from client-side access
    options.Cookie.IsEssential = true;             // Ensure cookies are always created
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use cookies only over HTTPS
});

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error pages in development
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Redirect to error page in production
    app.UseHsts();                          // Enable strict transport security
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles();      // Serve static files

app.UseRouting();          // Enable routing
app.UseSession();          // Enable session middleware
app.UseAuthentication();   // Enable authentication
app.UseAuthorization();    // Enable authorization

// Configure route mappings
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
