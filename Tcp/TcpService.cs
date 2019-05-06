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
        private static TcpListener serverSocket;

        public TcpClient TcpServiceClient => throw new NotImplementedException();

        public TcpService()
        {

            StartServer();
        }



        public static void StartServer()

        {

         

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 8888);

            serverSocket = new TcpListener(ipEndPoint);

            serverSocket.Start();

            Console.WriteLine("Asynchonous server socket is listening at: " + ipEndPoint.Address.ToString());

            WaitForClients();

        }

        private static void WaitForClients()

        {

            serverSocket.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnected), null);

        }

        private static void OnClientConnected(IAsyncResult asyncResult)

        {

            try

            {

                TcpClient clientSocket = serverSocket.EndAcceptTcpClient(asyncResult);

                if (clientSocket != null)

                    Console.WriteLine("Received connection request from: " + clientSocket.Client.RemoteEndPoint.ToString());

                HandleClientRequest(clientSocket);

            }

            catch

            {

                throw;

            }

            WaitForClients();

        }

        private static void HandleClientRequest(TcpClient clientSocket)
        {
            Console.WriteLine("Test");
        }


        public bool Connected()
        {
            return false;
        }

        public bool SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        bool ITcpService.Connect()
        {
            throw new NotImplementedException();
        }
    }
}
