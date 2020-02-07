using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consumer.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MqttConsumer _mqttService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IDbInitializer _dbInitializer;

        public Worker(ILogger<Worker> logger, MqttConsumer mqttService, IHostApplicationLifetime hostApplicationLifetime, IDbInitializer dbInitializer)
        {
            _logger = logger;
            _mqttService = mqttService;
            _hostApplicationLifetime = hostApplicationLifetime;
            _dbInitializer = dbInitializer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Init Db
            _dbInitializer.Initialize();

            _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _mqttService.Run();
                }
                catch (Exception)
                {

                }
               

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        private void OnStopping()
        {
            _logger.LogInformation("Worker shutting down...");
            _mqttService.OnStopping();
        }
    }
}
