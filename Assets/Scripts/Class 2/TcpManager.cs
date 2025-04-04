using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Class2
{
    public class TcpManager : MonoBehaviourSingleton<TcpManager>
    {
        private readonly List<TcpConnectedClient> serverClients = new List<TcpConnectedClient>();
        private TcpConnectedClient connectedClient;
        private TcpListener listener;
        private bool clientJustConnected;

        public bool IsServer { get; private set; }

        public event Action<byte[]> OnDataReceived;
        public event Action OnClientConnected;


        void Update ()
        {
            if (IsServer)
                UpdateServer();
            else
                UpdateClient();
        }

        void OnDestroy()
        {
            listener?.Stop();

            foreach (TcpConnectedClient client in serverClients)
                client.CloseClient();

            connectedClient.CloseClient();
        }


        private void UpdateServer ()
        {
            foreach (TcpConnectedClient client in serverClients)
                client.FlushReceivedData();
        }

        private void UpdateClient ()
        {
            if (clientJustConnected)
                OnClientConnected?.Invoke();

            connectedClient?.FlushReceivedData();
        }

        private void OnClientConnectToServer (IAsyncResult asyncResult)
        {
            TcpClient client = listener.EndAcceptTcpClient(asyncResult);
            TcpConnectedClient connectedClient = new TcpConnectedClient(client);

            serverClients.Add(connectedClient);
            listener.BeginAcceptTcpClient(OnClientConnectToServer, null);
        }

        private void OnConnectClient (IAsyncResult asyncResult)
        {
            connectedClient.OnEndConnection(asyncResult);
            clientJustConnected = true;
        }

        public void StartServer (int port)
        {
            IsServer = true;
            listener = new TcpListener(IPAddress.Any, port);

            listener.Start();
            listener.BeginAcceptTcpClient(OnClientConnectToServer, null);
        }

        public void StartClient (IPAddress serverIp, int port)
        {
            IsServer = false;

            TcpClient client = new TcpClient();

            connectedClient = new TcpConnectedClient(client);

            client.BeginConnect(serverIp, port, OnConnectClient, null);
        }

        public void ReceiveData (byte[] data)
        {
            OnDataReceived?.Invoke(data);
        }

        public void DisconnectClient (TcpConnectedClient client)
        {
            if (serverClients.Contains(client))
                serverClients.Remove(client);
        }

        public void BroadcastData (byte[] data)
        {
            foreach (TcpConnectedClient client in serverClients)
                client.SendData(data);
        }

        public void SendDataToServer (byte[] data)
        {
            connectedClient.SendData(data);
        }
    }
}