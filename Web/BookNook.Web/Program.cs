using BookNook.Data;
using BookNook.Data.Models;
using BookNook.Data.Repository.Interface;
using BookNook.Services.Data.Mapper;
using BookNook.Services.Data.Repository;
using BookNook.Services.Data.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDataLayer(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.AddScoped<Mapper>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderLineRepository, OrderLineRepository>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderLineService>();

builder.Services.AddIdentity<Client, IdentityRole<int>>(o =>
    {
        o.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<BookNookContext>()
    .AddDefaultTokenProviders();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookNookContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Client>>();
    context.Database.Migrate();
    await DbSeeder.SeedAsync(context, roleManager, userManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
