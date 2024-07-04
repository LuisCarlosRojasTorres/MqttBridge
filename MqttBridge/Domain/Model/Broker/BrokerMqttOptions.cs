namespace Domain.Model.Broker;

public class BrokerMqttOptions
{
    public int Port { get; set; } = 1183;
    public string Topic { get; set; } = "AlarmTopic";
}
