using Domain.Interfaces;
using Domain.Model.Common;
using Domain.Model.Client;
using HMI.Model;

MqttStates state = MqttStates.Initializing;
Broker broker = new Broker();

