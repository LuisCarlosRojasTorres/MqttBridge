namespace Domain.Interfaces;

public interface IClientMqtt
{
    public void ConnectToBroker(CancellationToken cancellationToken = default);

    public void SubscribeToTopic();

    public void PublishFile();

    public void PublishFile(string content);

    public void DisconnectToBroker();

    public void ProcessFile();

    public string ConvertFileToMetaFile(string? filePath, int numOfTotalFiles = 1, int indexOfFile = 1);

    public string ConvertMetaFileToFile(string? metaFileContent);


    /// <summary>
    /// Gets or Sets the method called to change the state of the services which use this client.
    /// </summary>
    Action? MqttClientCallback { get; set; }

    /// <summary>
    /// Gets or Sets the method called by the Updater service after a connection lost.
    /// </summary>
    Action? MqttConnectionLostCallback { get; set; }
}
