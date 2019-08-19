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
        private static DataRepository _dataRepository;
        private static IMqttClient _mqttClient;
        private static IMqttClientOptions _mqttClientOptions;

        static void ConfigureService()
        {
            // Load from configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var mqqtConfig = config.GetSection("MqttClient");

            int port = 1883;
            Int32.TryParse(config.GetSection("MqttServerPort").Value, out port);

            // MQTT options
            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(config.GetSection("ClientId").Value)
                .WithTcpServer(config.GetSection("MqttServerIp").Value, port)
                .WithCleanSession()
                .Build();

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();


            _dataRepository = new DataRepository(config.GetSection("DataSource").Value);
        }
        static void Main(string[] args)
        {
            ConfigureService();

            Console.WriteLine("Running");
        }
    }
}
