namespace ExternalClient.Model;

using MQTTnet.Client;
using MQTTnet;
using Domain.Interfaces;
using Domain.Model.Client;
using Domain.Model.Common;
using System.Text.Json;

public class PitoMqttClient : IClientMqtt
{
    private readonly MqttFactory mqttFactory;
    private readonly IMqttClient? mqttClient;
    private readonly ClientMqttOptions? pitoMqttOptions;
    private MqttClientOptions? mqttClientOptions;
    private bool connectToPrimaryHMI;

    public PitoMqttClient()
    {
        this.mqttFactory = new MqttFactory();
        this.mqttClient = this.mqttFactory.CreateMqttClient();

        using (StreamReader file = File.OpenText(Path.Combine("ClientConfig.json")))
        {
            this.pitoMqttOptions = JsonSerializer.Deserialize<ClientMqttOptions>(file.ReadToEnd());
        }

        this.connectToPrimaryHMI = true;
    }

    public Action? MqttClientCallback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action? MqttConnectionLostCallback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void ConnectToBroker(CancellationToken cancellationToken = default)
    {
        try
        {
            string brokerIp = string.Empty;

            if (this.connectToPrimaryHMI)
            {
                brokerIp = this.pitoMqttOptions!.Phmi_Ip;
            }
            else
            {
                brokerIp = this.pitoMqttOptions!.Shmi_Ip;
            }

            Console.WriteLine($"    - BrokerIp: {brokerIp}");
            this.mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(brokerIp, this.pitoMqttOptions.Port).Build();
            using CancellationTokenSource timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(this.pitoMqttOptions!.Timeout));
            this.mqttClient!.ConnectAsync(this.mqttClientOptions, timeoutToken.Token).Wait(cancellationToken);
            Console.WriteLine($"    - Connected");
        }
        catch (Exception ex)
        {
            this.connectToPrimaryHMI = !this.connectToPrimaryHMI;
            throw new Exception($"Error PitoMqttClient Connecting", ex);
        }
    }
    public void PublishFile()
    {
        string metaFileSerialized = this.ConvertFileToMetaFile(this.pitoMqttOptions!.FileToTransfer);
        MqttApplicationMessage? applicationMessage = new MqttApplicationMessageBuilder()
             .WithTopic(this.pitoMqttOptions!.Topic)
             .WithPayload(metaFileSerialized)
             .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
             .WithRetainFlag(true)
             .Build();

        try
        {
            using CancellationTokenSource timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(this.pitoMqttOptions!.Timeout));
            this.mqttClient!.PublishAsync(applicationMessage, CancellationToken.None).Wait();
        }
        catch (Exception ex)
        {
            this.connectToPrimaryHMI = !this.connectToPrimaryHMI;
            throw new Exception($"Error PitoMqttClient Publishing", ex);
        }
    }

    public string ConvertFileToMetaFile(string? filePath, int numOfTotalFiles = 1, int indexOfFile = 1)
    {
        MetaFile metaFile = new(filePath)
        {
            NumOfTotalFiles = numOfTotalFiles,
            IndexOfFile = indexOfFile,
        };

        return JsonSerializer.Serialize(metaFile);
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

    public void PublishMetaFile()
    {
        throw new NotImplementedException();
    }

    public void SubscribeToTopic()
    {
        throw new NotImplementedException();
    }

    public void PublishFile(string content)
    {
        throw new NotImplementedException();
    }
}
