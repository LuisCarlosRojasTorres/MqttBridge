using Domain.Interfaces;
using Domain.Model.Common;
using Domain.Model.Client;
using HMI.Model;
using MQTTnet.Client;

MqttStates state = MqttStates.Initializing;
Broker broker = new Broker();
broker.AlarmReceivedCallback += SetStateToPublish;
CancellationTokenSource cts = new();

IClientMqtt hmiMqttClient = new HmiMqttClient();

state = MqttStates.Connecting;

int loopCounter = 0;

while (!cts.Token.IsCancellationRequested)
{
    Console.WriteLine($" - HmiMqttClient LoopCounter: {++loopCounter}, state: {state.ToString()}");
    switch (state)
    {
        case MqttStates.Connecting:
            try
            {
                hmiMqttClient.ConnectToBroker();
                state = MqttStates.Waiting;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            break;

        case MqttStates.Waiting:

            break;

        case MqttStates.Publishing:
            try
            {
                hmiMqttClient.PublishFile(broker.GetAlarmPackageContent());
                state = MqttStates.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            break;

        case MqttStates.Success:
        case MqttStates.Cancelling:
            try
            {
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            break;

        default:
            cts.Cancel();
            break;
    }

    await Task.Delay(1000);
}

void SetStateToPublish()
{
    state = MqttStates.Publishing;
}