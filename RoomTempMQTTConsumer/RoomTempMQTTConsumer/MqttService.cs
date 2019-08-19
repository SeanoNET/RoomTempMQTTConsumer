using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.IO;

namespace RoomTempMQTTConsumer
{
    internal class MqttService
    {
        private readonly DataRepository _dataRepository;
        private IMqttClient _mqttClient;
        private IMqttClientOptions _mqttClientOptions;

        public MqttService(IConfiguration configuration)
        {
            _dataRepository = new DataRepository(configuration.GetSection("DataSource").Value);

            var mqqtConfig = configuration.GetSection("MqttClient");   
            
            int port = 1883;
            Int32.TryParse(mqqtConfig.GetSection("MqttServerPort").Value, out port);

            // MQTT options
            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(mqqtConfig.GetSection("ClientId").Value)
                .WithTcpServer(mqqtConfig.GetSection("MqttServerIp").Value, port)
                .WithCleanSession()
                .Build();

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }
        public void Run()
        {
            Console.WriteLine("Running");
        }
    }
}
