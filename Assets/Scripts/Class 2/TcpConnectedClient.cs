using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Class2
{
    public class TcpConnectedClient
    {
        private TcpClient client;
        private Queue<byte[]> dataReceived = new Queue<byte[]>();
        private byte[] readBuffer = new byte[5000];
        private object readHandler = new object();

        private NetworkStream NetworkStream => client?.GetStream();


        public TcpConnectedClient (TcpClient client)
        {
            this.client = client;

            if (TcpManager.Instance.IsServer)
                NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }

        private void OnRead (IAsyncResult asyncResult)
        {
            if (NetworkStream.EndRead(asyncResult) == 0)
            {
                TcpManager.Instance.DisconnectClient(this);
                return;
            }

            lock (readHandler)
            {
                byte[] data = readBuffer.TakeWhile(b => (char)b != '\0').ToArray();
                dataReceived.Enqueue(data);
            }

            Array.Clear(readBuffer, 0, readBuffer.Length);
            NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }

        public void SendData (byte[] data)
        {
            NetworkStream.Write(data, 0, data.Length);
        }

        public void FlushReceivedData ()
        {
            lock (readHandler)
            {
                while (dataReceived.Count > 0)
                {
                    byte[] data = dataReceived.Dequeue();
                    TcpManager.Instance.ReceiveData(data);
                }
            }
        }

        public void OnEndConnection (IAsyncResult asyncResult)
        {
            client.EndConnect(asyncResult);
            NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }

        public void CloseClient ()
        {
            client.Close();
        }
    }
}