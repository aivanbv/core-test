using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace GenericHost
{
    public class TcpService : ITcpService
    {
        private ILogger<TcpService> _logger;
        private TcpClient _tcpClient;

        public TcpService(ILogger<TcpService> logger)
        {
            _logger = logger;
            Connect();
        }

        public TcpClient TcpServiceClient
        {
            get { return _tcpClient; }
        }
        public bool Connect()
        {
            if (_tcpClient?.Connected == true)
            {
                return true;
            }
            else
            {
                try
                {
                    if (_tcpClient != null && _tcpClient?.Connected == true)
                    {
                        _tcpClient.Close();
                    }

                    // Create the socket object
                    _tcpClient = new TcpClient();
                    _tcpClient.ReceiveTimeout = 4000;

                    // Define the Server address and port
                    IPEndPoint ModemEndPoint = new IPEndPoint(IPAddress.Parse("10.29.208.156"), 23);


                    var test = _tcpClient.BeginConnect("10.29.208.156", 8001, null, null);
                    var result = test.AsyncWaitHandle.WaitOne();
                    if (result && _tcpClient.Connected)
                    {
                        _logger.LogInformation("Tcp Connected");
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
                catch
                {
                    return false;
                }

            }
        }

        public bool Connected()
        {
            return _tcpClient?.Connected ?? false;
        }

        public bool SendMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
