using System.Net.Sockets;
using uPLibrary.Networking.M2Mqtt;

namespace GenericHost
{
    public interface ITcpService
    {
        TcpClient TcpServiceClient { get; }

        bool Connect();
        bool Connected();
        bool SendMessage(string message);
    }
}