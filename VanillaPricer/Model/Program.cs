using Domain;
using Application;
using MarketDataServices;
using VanillaPricer;
using PositionServices;
using PricerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<PositionService>();
builder.Services.AddSingleton<MarketDataService>();
builder.Services.AddSingleton<PricerService>();
builder.Services.AddTransient<IPricer<EuropeanCallOption>, EuropeanCallPricer>();
builder.Services.AddTransient<IPricer<EuropeanPutOption>, EuropeanPutPricer>();
builder.Services.AddTransient<IPricer<AmericanCallOption>, AmericanCallPricer>();
builder.Services.AddTransient<IPricer<AmericanPutOption>, AmericanPutPricer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
