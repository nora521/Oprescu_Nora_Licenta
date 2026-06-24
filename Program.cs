using Licenta.Data;
using Licenta.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using QuestPDF.Infrastructure;
using Licenta.Services;


var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.EnableDebugging = false;
QuestPDF.Settings.License = LicenseType.Community; 


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
   policy.RequireRole("Admin"));
});

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Autovehicule/Index");
    options.Conventions.AuthorizeFolder("/Utilizatori");
    options.Conventions.AuthorizePage("/Chat");
    options.Conventions.AuthorizeFolder("/Marci", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Combustibili", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Utilizatori", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Marci", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Combustibili", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Rezervari");
    options.Conventions.AuthorizePage("/Permise/Permis");
});
builder.Services.AddDbContext<LicentaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LicentaContext") ?? throw new InvalidOperationException("Connection string 'LicentaContext' not found.")));
builder.Services.AddDbContext<LibraryIdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LicentaContext") ?? throw new InvalidOperationException("Connection string 'LicentaContext' not found.")));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<LibraryIdentityContext>();

builder.Services.AddScoped<PermisParser>();
builder.Services.AddTransient<Licenta.Services.EmailService>(); builder.Services.AddHostedService<NotificariBackgroundService>();
builder.Services.AddSingleton<ChatbotService>();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
Environment.SetEnvironmentVariable(
    "GOOGLE_APPLICATION_CREDENTIALS",
    Path.Combine(Directory.GetCurrentDirectory(), "google-credentials.json"));

builder.Services.AddScoped<OCRService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var user = await userManager.FindByEmailAsync("nora_oprescu@yahoo.com");

    if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
    {
        await userManager.AddToRoleAsync(user, "Admin");
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
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/Autovehicule"));
app.MapRazorPages();



app.Run();
