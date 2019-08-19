using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace RoomTempMQTTConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load from configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var mqqtConfig = config.GetSection("MqttClient");

            Console.WriteLine(mqqtConfig.GetSection("ClientId").Value);
        }
    }
}
