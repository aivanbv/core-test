using uPLibrary.Networking.M2Mqtt;

namespace GenericHost
{
    public interface IMqttService
    {
        MqttClient MqttServiceClient { get; }

        bool Connect();
        bool Connected();
        bool SendMessage(string message,string topic);
    }
}