using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mt4LogParser.Data;
using Microsoft.Extensions.Hosting;
using Mt4LogParser.Engine;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContextFactory<Mt4Context>(options => 
            options.UseNpgsql(context.Configuration.GetConnectionString("Default")));

        services.AddSingleton<LogManager>();
        services.AddSingleton<LogProcessor>();
        services.AddSingleton<LogUploader>();
        services.AddSingleton<DataProccess>();
        services.AddHostedService<Application>();
        services.Configure<LogSettings>(context.Configuration.GetSection("LogSettings"));
    })
    .Build();

await host.RunAsync();


