using Consumer.Data;
using Consumer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    internal class MqttConsumer
    {
        private readonly ILogger<MqttConsumer> _logger;
        private IMqttClient _mqttClient;
        private IMqttClientOptions _mqttClientOptions;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private string _subTopic;

        public MqttConsumer(ILogger<MqttConsumer> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
          
            int port = 1883;
            Int32.TryParse(configuration.GetSection("MqttServerPort").Value, out port);

            // MQTT options
            _mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(configuration.GetSection("ClientId").Value)
                .WithTcpServer(configuration.GetSection("MqttServerIp").Value, port)
                .WithCleanSession()
                .Build();

            _subTopic = configuration.GetSection("MqttSubscribeTopic").Value;


            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Event handlers
            _mqttClient.UseDisconnectedHandler(OnDisconnected);
            _mqttClient.UseApplicationMessageReceivedHandler(OnMessageReceived);
            _mqttClient.UseConnectedHandler(OnConnected);


        }

        private async void OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            _logger.LogWarning($"Disconnected from MQTT server: {e.Exception.Message}");

            if(e.Exception != null)
                _logger.LogError($"Error: {e.Exception.Message}");

            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch
            {
                _logger.LogError($"Reconnection failed");
            }
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            _logger.LogInformation($"Received message");
            _logger.LogInformation($"Topic = {e.ApplicationMessage.Topic}");
            _logger.LogInformation($"Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            _logger.LogInformation($"QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            _logger.LogInformation($"Retain = {e.ApplicationMessage.Retain}");

            var payload = JsonConvert.DeserializeObject<SensorData>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

            _logger.LogInformation($"Temperature {payload.Temperature}");
            _logger.LogInformation($"Humidity {payload.Humidity}");
            payload.MeasuredAt = DateTime.Now;

            SavePayload(payload);
        }

        private async void OnConnected(MqttClientConnectedEventArgs e)
        {
            _logger.LogInformation($"Connected to MQTT server");


            // Subscribe to a topic
            await _mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(_subTopic).Build());
            _logger.LogInformation($"Subscribed to topic: {_subTopic}");
        }

        private async void SavePayload(SensorData data)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                try
                {
                    context.SensorData.Add(data);
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Saved successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to save payload data", ex);
                    return;
                }
            }
        }
        public async void Run()
        {
            try
            {
                _logger.LogInformation("Running");
                await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }


        }

        /// <summary>
        /// Executes on worker shutdown
        /// </summary>
        public async void OnStopping()
        {
            await _mqttClient.DisconnectAsync();
            _mqttClient.Dispose();
        }
    }
}
