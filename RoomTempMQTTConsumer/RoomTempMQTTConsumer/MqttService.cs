using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using RoomTempMQTTConsumer.Entities;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoomTempMQTTConsumer
{
    internal class MqttService
    {
        private readonly DataRepository _dataRepository;
        private IMqttClient _mqttClient;
        private IMqttClientOptions _mqttClientOptions;

        private string _subTopic;

        public MqttService(IConfiguration configuration)
        {
            _dataRepository = new DataRepository(configuration.GetSection("DataRepository"));

            var mqqtConfig = configuration.GetSection("MqttClient");   
            
            int port = 1883;
            Int32.TryParse(mqqtConfig.GetSection("MqttServerPort").Value, out port);

            // MQTT options
            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(mqqtConfig.GetSection("ClientId").Value)
                .WithTcpServer(mqqtConfig.GetSection("MqttServerIp").Value, port)
                .WithCleanSession()
                .Build();

            _subTopic = mqqtConfig.GetSection("MqttSubscribeTopic").Value;

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Event handlers
            _mqttClient.UseDisconnectedHandler(OnDisconnected);
            _mqttClient.UseApplicationMessageReceivedHandler(OnMessageReceived);
            _mqttClient.UseConnectedHandler(OnConnected);


        }

        private async void OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine("### DISCONNECTED FROM SERVER ###");
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch
            {
                Console.WriteLine("### RECONNECTING FAILED ###");
            }
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine();

            var payload = JsonConvert.DeserializeObject<MetricData>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

            Console.WriteLine(payload.Temperature);
            Console.WriteLine(payload.Humidity);
            payload.MeasuredAt = DateTime.Now;

            if (_dataRepository.SavePayload(payload))
            {
                Console.WriteLine("Saved successfully");
            }
        }

        private async void OnConnected(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("### CONNECTED WITH SERVER ###");

            // Subscribe to a topic
            await _mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(_subTopic).Build());

            Console.WriteLine($"Subscribed to topic: {_subTopic}");
        }

        public async void Run()
        {
            try
            {
                Console.WriteLine("Running");
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred - {ex.Message}");
            }
           
          
        }
    }
}
