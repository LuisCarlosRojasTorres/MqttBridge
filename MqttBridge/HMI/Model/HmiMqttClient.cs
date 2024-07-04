namespace HMI.Model;

using Domain.Interfaces;
using Domain.Model.Client;
using MQTTnet.Client;
using MQTTnet;
using System;
using System.Threading;
using System.Text.Json;
using System.Net.Sockets;
using System.Net;

public class HmiMqttClient : IClientMqtt
{
    private readonly MqttFactory mqttFactory;
    private readonly IMqttClient? mqttClient;
    private readonly ClientMqttOptions? hmiMqttOptions;
    private MqttClientOptions? mqttClientOptions;

    public HmiMqttClient()
    {
        this.mqttFactory = new MqttFactory();
        this.mqttClient = this.mqttFactory.CreateMqttClient();

        using (StreamReader file = File.OpenText(Path.Combine("ClientConfig.json")))
        {
            this.hmiMqttOptions = JsonSerializer.Deserialize<ClientMqttOptions>(file.ReadToEnd());
        }

    }
    public Action? MqttClientCallback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action? MqttConnectionLostCallback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void ConnectToBroker(CancellationToken cancellationToken = default)
    {
        try
        {
            string brokerIp = string.Empty;

            if (this.GetLocalIPAddress() == this.hmiMqttOptions!.Phmi_Ip)
            {
                brokerIp = this.hmiMqttOptions!.Shmi_Ip;
            }
            else
            {
                brokerIp = this.hmiMqttOptions!.Phmi_Ip;
            }

            Console.WriteLine($"    - HmiMqttClient Connecting to: {brokerIp}");
            this.mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(brokerIp, this.hmiMqttOptions.Port).Build();
            using CancellationTokenSource timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(this.hmiMqttOptions!.Timeout));
            this.mqttClient!.ConnectAsync(this.mqttClientOptions, timeoutToken.Token).Wait(cancellationToken);
        }
        catch (Exception ex)
        {
            //throw new Exception($"HmiMqttClient Connecting", ex);
            throw new Exception($"HmiMqttClient Connecting");
        }
    }

    public void PublishFile(string content)
    {
        MqttApplicationMessage? applicationMessage = new MqttApplicationMessageBuilder()
             .WithTopic(this.hmiMqttOptions!.Topic)
             .WithPayload(content)
             .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
             .WithRetainFlag(true)
             .Build();

        try
        {
            using CancellationTokenSource timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(this.hmiMqttOptions!.Timeout));
            this.mqttClient!.PublishAsync(applicationMessage, CancellationToken.None).Wait();
        }
        catch (Exception ex)
        {
            //throw new Exception($"Error PitoMqttClient Publishing", ex);
            throw new Exception($"Error PitoMqttClient Publishing");
        }
    }

    public string ConvertFileToMetaFile(string? filePath, int numOfTotalFiles = 1, int indexOfFile = 1)
    {
        throw new NotImplementedException();
    }

    public string ConvertMetaFileToFile(string? metaFileContent)
    {
        throw new NotImplementedException();
    }

    public void DisconnectToBroker()
    {
        throw new NotImplementedException();
    }

    public void ProcessFile()
    {
        throw new NotImplementedException();
    }

    public void PublishFile()
    {
        throw new NotImplementedException();
    }    

    public void PublishMetaFile()
    {
        throw new NotImplementedException();
    }

    public void SubscribeToTopic()
    {
        throw new NotImplementedException();
    }

    private string GetLocalIPAddress()
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
}
