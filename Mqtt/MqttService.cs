using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace GenericHost
{
    public class MqttService : IMqttService
    {
        private ILogger<MqttService> _logger;
        private MqttClient _mqttClient;
        string clientId;

        public MqttService(ILogger<MqttService> logger)
        {
            _logger = logger;
            clientId = Guid.NewGuid().ToString();
            Connect();
        }

        public bool Connect()
        {
            if (_mqttClient?.IsConnected == true)
            {
                return true;
            }
            else
            {
                // Create Client instance
                _mqttClient = new MqttClient("localhost");
       
                // Register to message received
                _mqttClient.MqttMsgPublishReceived += client_recievedMessage;
                _mqttClient.ConnectionClosed += _mqttClient_ConnectionClosed;
                CancellationTokenSource source = new CancellationTokenSource(); Task T = Task.Run(() => TryReconnectAsync(source.Token)); T.Wait();
                return true;
            }
        }

        private void _mqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            _logger.LogInformation("ConnectionClosed");
            CancellationTokenSource source = new CancellationTokenSource();
            Task T = Task.Run(() => TryReconnectAsync(source.Token)); T.Wait();
          
        }
        private async Task TryReconnectAsync(CancellationToken cancellationToken)
        {
            var connected = _mqttClient.IsConnected;
            while (!connected && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _mqttClient.Connect(clientId);

                    _mqttClient.Publish("/testing", Encoding.ASCII.GetBytes("TestMsg"));
                    // Subscribe to topic, Subscribed topics disappear if connection is lost. 
                    _mqttClient.Subscribe(new String[] { "/testing" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                }
                catch
                {
                    _logger.LogInformation("No connection to...{0}");
                }
                connected = _mqttClient.IsConnected;
                if(connected == true){
                    _logger.LogInformation("Connection reconnected");
                }
         
                await Task.Delay(3000, cancellationToken);
            }
        }



        private void client_recievedMessage(object sender, MqttMsgPublishEventArgs e)
        {
            var message = System.Text.Encoding.Default.GetString(e.Message);
            _logger.LogInformation("Message received:" + message);
        }


        public MqttClient MqttServiceClient
            { get { return _mqttClient; }
        }

        public bool Connected()
        {
            return _mqttClient.IsConnected;
        }

        public bool SendMessage(string message)
        {
            try
            {
                _mqttClient.Publish("/testing", Encoding.ASCII.GetBytes(message));
            }
            catch(Exception e)
            {
                _logger.LogError("Message received:" + e);
                return false;
            }
            return true;
            
        }
    }



}
