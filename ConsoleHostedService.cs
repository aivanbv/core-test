using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace GenericHost
{
    public class ConsoleHostedService : BackgroundService
    {
        private readonly IApplicationLifetime _appLifetime;
        private readonly IMqttService _mqttService;
        private readonly ITcpService _tcpService;
        private readonly IOptions<AppConfig> _appConfig;
        private readonly ILogger<ConsoleHostedService> _logger;
        private Timer _timer;

        public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IMqttService mqttService, ITcpService tcpService, IOptions<AppConfig> appConfig, IApplicationLifetime appLifetime)
        {
            _appLifetime = appLifetime;
            _mqttService = mqttService;
            _tcpService = tcpService;
            _appConfig = appConfig;
            _mqttService.MqttServiceClient.MqttMsgPublishReceived += client_recievedMessage;
            _mqttService.MqttServiceClient.ConnectionClosed += MqttServiceClient_ConnectionClosed; 
        
            _logger = logger;
            _logger.LogInformation("ConsoleHostedService instance created...");
        }

        private void MqttServiceClient_ConnectionClosed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void client_recievedMessage(object sender, MqttMsgPublishEventArgs e)
        {
            var message = System.Text.Encoding.Default.GetString(e.Message);
            _logger.LogInformation("Message received ConsoleHostedService:" + message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
       
            _logger.LogInformation("MqttStatus: "+ _mqttService.Connected().ToString());

            Task T = Task.Run(() => SendMessage());
            _logger.LogInformation("Task is running");


            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            await Task.CompletedTask;
        }


        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
        }

        private async Task SendMessage()
        {
            var connected = _mqttService.Connected();
            while (connected)
            {
                try
                {
                    connected = _mqttService.Connected();
                    _mqttService.SendMessage(_appConfig.Value.MQTTMessage);
                }
                catch
                {
                    _logger.LogInformation("No connection to...{0}");
                }

                await Task.Delay(3000);
            }
        }

        private void OnStarted()
        {

            _logger.LogInformation("OnStarted has been called.");
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
            // Perform post-stopped activities here
        }

    }
}
