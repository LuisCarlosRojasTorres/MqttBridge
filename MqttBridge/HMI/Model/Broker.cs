namespace HMI.Model;

using Domain.Model.Broker;
using System.Net.Sockets;
using System.Net;
using MQTTnet;
using MQTTnet.Server;
using System.Text.Json;
using System.Text;
using Domain.Model.Common;

public class Broker
{
    private readonly MqttFactory mqttFactory;
    private readonly BrokerMqttOptions brokerMqttOptions;
    private readonly MqttServerOptions mqttServerOptions;
    private MqttServer mqttServer;
    public Action? AlarmReceivedCallback { get; set; }
    private string alarmPackageContent;
    private MetaFile alarmMetaFile;

    public Broker()
    { 
        this.mqttFactory = new MqttFactory();
        this.mqttServerOptions = this.mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();
        this.mqttServer = this.mqttFactory.CreateMqttServer(this.mqttServerOptions);
        this.mqttServer.InterceptingPublishAsync += this.InterceptApplicationMessagePublishAsync;

        using (StreamReader file = File.OpenText(Path.Combine("BrokerConfig.json")))
        {
            this.brokerMqttOptions = JsonSerializer.Deserialize<BrokerMqttOptions>(file.ReadToEnd())!;
        }

        this.alarmPackageContent = string.Empty;        
        this.alarmMetaFile = new MetaFile();

        this.Run_Minimal_Server(this.brokerMqttOptions!).Wait();
    }
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public async Task Run_Minimal_Server(BrokerMqttOptions brokerOptions)
    {
        //This sample starts a simple MQTT server which will accept any TCP connection.
        
        await mqttServer.StartAsync();
        
        Console.WriteLine($"Broker at {this.GetLocalIPAddress()}");                    
    }

    public string GetAlarmPackageContent() 
    {
        return this.alarmPackageContent;
    }

    private async Task InterceptApplicationMessagePublishAsync(InterceptingPublishEventArgs args)
    {
        try
        {
            args.ProcessPublish = true;
            if (args.ApplicationMessage != null)
            {
                if (args.ApplicationMessage.Topic == this.brokerMqttOptions.Topic)
                {
                    this.alarmPackageContent = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);

                    MetaFile dummyMetaFile = JsonSerializer.Deserialize<MetaFile>(this.alarmPackageContent);

                    if (alarmMetaFile.HashMd5 != dummyMetaFile!.HashMd5)
                    {

                        this.alarmMetaFile = dummyMetaFile;
                        this.AlarmReceivedCallback?.Invoke();
                        Console.WriteLine($"Broker: Received a NEW message with Topic: {this.brokerMqttOptions.Topic} and Hash: {this.alarmMetaFile.HashMd5}");
                    }
                    else 
                    {
                        Console.WriteLine($"Broker: Received a REPEATED message with Topic: {this.brokerMqttOptions.Topic} and Hash: {this.alarmMetaFile.HashMd5}");
                    }
                    
                }
            }            
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: {Exception}.", ex);
        }
    }
}
