namespace Domain.Model.Common;

public enum MqttStates
{
    Initializing = 0,

    CreatingBroker = 1,

    Connecting = 2,

    Subscribing = 3,

    Waiting = 4,

    Publishing = 5,

    Downloading = 6,

    Processing = 7,

    Success = 8,

    Cancelling = 9,
}


