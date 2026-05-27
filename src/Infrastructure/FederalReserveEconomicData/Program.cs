using FederalReserveEconomicData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfiguration configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var services = new ServiceCollection()
    .Configure<FredApiKey>(configuration.GetSection(nameof(FredApiKey)).Bind)
    .AddOptions()
    .AddSingleton<ConstantMaturityTreasuriesFetcher>()
    .BuildServiceProvider();

ConstantMaturityTreasuriesFetcher? fetcher = services.GetService<ConstantMaturityTreasuriesFetcher>();

if (fetcher != null) {
    await fetcher.FetchAndDump();
}
