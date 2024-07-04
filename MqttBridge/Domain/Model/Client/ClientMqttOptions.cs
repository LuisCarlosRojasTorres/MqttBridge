namespace Domain.Model.Client;

public class ClientMqttOptions
{
    public string Phmi_Ip { get; set; } = "1.1.1.141";
    public string Shmi_Ip { get; set; } = "1.1.1.142";
    public int? Port { get; set; } = 1183;
    public string Topic { get; set; } = "AlarmTopic";
    public string FileToTransfer { get; set; } = "./alarms.json";
    public int Timeout { get; set; } = 5;
}