using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Class2
{
    public class NetworkSetupScreen : MonoBehaviour
    {
        [SerializeField] private TMP_InputField serverIpField;
        [SerializeField] private TMP_InputField serverPortField;
        [SerializeField] private Button startSeverButton;
        [SerializeField] private Button connectToServerButton;
        [SerializeField] private GameObject chatScreen;


        void Awake ()
        {
            chatScreen.SetActive(false);
            startSeverButton.onClick.AddListener(OnStartServer);
            connectToServerButton.onClick.AddListener(OnConnectToServer);
        }

        void OnDestroy()
        {
            startSeverButton.onClick.RemoveListener(OnStartServer);
            connectToServerButton.onClick.RemoveListener(OnConnectToServer);
        }


        private void OnStartServer ()
        {
            int port = Convert.ToInt32(serverPortField.text);
            TcpManager.Instance.StartServer(port);

            MoveToChatScreen();
        }

        private void OnConnectToServer ()
        {
            IPAddress ipAddress = IPAddress.Parse(serverIpField.text);
            int port = Convert.ToInt32(serverPortField.text);

            TcpManager.Instance.StartClient(ipAddress, port);
            TcpManager.Instance.OnClientConnected += MoveToChatScreen;
        }

        private void MoveToChatScreen ()
        {
            gameObject.SetActive(false);
            chatScreen.SetActive(true);
        }
    }
}