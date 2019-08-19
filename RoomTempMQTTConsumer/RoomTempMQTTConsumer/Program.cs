using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.IO;

namespace RoomTempMQTTConsumer
{
    class Program
    {
        
        private static MqttService _mqttService;

        static void ConfigureService()
        {
            // Load from configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            _mqttService = new MqttService(config);                 
        }

        static void Main(string[] args)
        {
            ConfigureService();


            _mqttService.Run();


            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
