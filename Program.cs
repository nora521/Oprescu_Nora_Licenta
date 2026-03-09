using Licenta.Data;
using Licenta.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community; //export pdf


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
   policy.RequireRole("Admin"));
});

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Autovehicule");
    options.Conventions.AuthorizeFolder("/Utilizatori");
    options.Conventions.AuthorizeFolder("/Marci");
    options.Conventions.AuthorizeFolder("/Combustibili");
    options.Conventions.AuthorizeFolder("/Utilizatori", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Marci", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Combustibili", "AdminPolicy");
});
builder.Services.AddDbContext<LicentaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LicentaContext") ?? throw new InvalidOperationException("Connection string 'LicentaContext' not found.")));
builder.Services.AddDbContext<LibraryIdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LicentaContext") ?? throw new InvalidOperationException("Connection string 'LicentaContext' not found.")));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<LibraryIdentityContext>();

builder.Services.AddTransient<Licenta.Services.EmailService>(); //serviciu mail
builder.Services.AddHostedService<NotificariBackgroundService>(); //background service

var app = builder.Build();


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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
