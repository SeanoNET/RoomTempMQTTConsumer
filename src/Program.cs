using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Consumer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {        
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Load from configuration
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .AddEnvironmentVariables()
                        .Build();

                    services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(config.GetSection("DataSource").Value));

                    services.AddTransient<IDbInitializer, DbInitializer>();
                    services.AddHostedService<Worker>();
                    services.AddSingleton<MqttConsumer>();
         
                    
                });
    }
}
