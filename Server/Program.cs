using Application;
using Domain;
using MarketDataServices;
using PositionServices;
using PricerServices;
using Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<PositionService>();
builder.Services.AddSingleton<MarketDataService>();
builder.Services.AddSingleton<PricerService>();
builder.Services.AddTransient<IPricer<CashFlow>, CashFlowPricer>();
builder.Services.AddTransient<IPricer<EuropeanCall>, EuropeanCallPricer>();
builder.Services.AddTransient<IPricer<EuropeanPut>, EuropeanPutPricer>();

builder.Services.AddControllers().AddJsonOptions(o => {
    o.JsonSerializerOptions.Converters.Add(new ContractConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
