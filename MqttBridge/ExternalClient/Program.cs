using Domain.Interfaces;
using Domain.Model.Common;
using ExternalClient.Model;

MqttStates state = MqttStates.Initializing;
CancellationTokenSource cts = new();
IClientMqtt pitoMqttClient = new PitoMqttClient();

state = MqttStates.Connecting;

int loopCounter = 0;

while (!cts.Token.IsCancellationRequested)
{
    Console.WriteLine($" - PITO LoopCounter: {++loopCounter}, state: {state.ToString()}");
    switch (state)
    {
        case MqttStates.Connecting:
            try
            {
                pitoMqttClient.ConnectToBroker();
                state = MqttStates.Publishing;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Console.WriteLine("Error in ExternalClient Connection");
            }

            break;

        case MqttStates.Publishing:
            try
            {
                pitoMqttClient.PublishFile();
                state = MqttStates.Success;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Console.WriteLine("Error in ExternalClient Publishing");
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
                //Console.WriteLine(ex.ToString());
                Console.WriteLine("Error in ExternalClient Finishing");
            }

            break;

        default:
            cts.Cancel();
            break;
    }

    await Task.Delay(1000);
}
