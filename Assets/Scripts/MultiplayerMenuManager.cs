using UnityEngine;
using UnityEngine.UI;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public class MultiplayerMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public InputField playerNameInput;
    public InputField serverIPInput;
    public Button createServerButton;
    public Button connectClientButton;
    public Text localIPText;
    public Text statusText;
    
    [Header("Network Settings")]
    public NetworkManager networkManager;
    
    [Header("Room System")]
    public SimpleRoomManager roomManager;
    
    private string playerName = "";
    private bool isServer = false;
    
    void Start()
    {
        // Initialize UI
        SetupUI();
        
        // Get local IP address
        DisplayLocalIP();
        
        // Subscribe to network events
        if (networkManager != null)
        {
            networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
        }
    }
    
    void SetupUI()
    {
        // Set default values
        serverIPInput.gameObject.SetActive(false); // Hide IP input initially
        
        // Setup button listeners
        createServerButton.onClick.AddListener(OnCreateServerClicked);
        connectClientButton.onClick.AddListener(OnConnectClientClicked);
        
        // Setup input field listeners
        playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
    }
    
    void DisplayLocalIP()
    {
        try
        {
            string localIP = GetLocalIPAddress();
            localIPText.text = $"Tu IP Local: {localIP}";
        }
        catch (System.Exception e)
        {
            localIPText.text = "Tu IP Local: No disponible";
            Debug.LogError($"Error getting local IP: {e.Message}");
        }
    }
    
    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip));
        return ipAddress?.ToString() ?? "127.0.0.1";
    }
    
    void OnPlayerNameChanged(string name)
    {
        playerName = name.Trim();
        
        // Enable/disable buttons based on name input
        bool hasName = !string.IsNullOrEmpty(playerName);
        createServerButton.interactable = hasName;
        connectClientButton.interactable = hasName;
        
        // Show/hide IP input based on selection
        if (hasName)
        {
            serverIPInput.gameObject.SetActive(true);
        }
    }
    
    void OnCreateServerClicked()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            UpdateStatus("Por favor ingresa tu nombre", Color.red);
            return;
        }
        
        isServer = true;
        serverIPInput.gameObject.SetActive(false); // Hide IP input for server
        
        UpdateStatus("Iniciando servidor...", Color.yellow);
        
        // Start server
        if (networkManager != null)
        {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection(); // Server also acts as client
        }
    }
    
    void OnConnectClientClicked()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            UpdateStatus("Por favor ingresa tu nombre", Color.red);
            return;
        }
        
        string serverIP = serverIPInput.text.Trim();
        if (string.IsNullOrEmpty(serverIP))
        {
            UpdateStatus("Por favor ingresa la IP del servidor", Color.red);
            return;
        }
        
        isServer = false;
        
        UpdateStatus("Conectando al servidor...", Color.yellow);
        
        // Set server address and connect
        if (networkManager != null)
        {
            // Set the server address in the transport
            var transport = networkManager.TransportManager.Transport;
            if (transport != null)
            {
                transport.SetClientAddress(serverIP);
            }
            
            networkManager.ClientManager.StartConnection();
        }
    }
    
    void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            UpdateStatus($"Servidor iniciado - Esperando jugadores", Color.green);
            DisableMenuButtons();
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            UpdateStatus("Servidor detenido", Color.red);
            EnableMenuButtons();
        }
    }
    
    void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            if (isServer)
            {
                UpdateStatus($"Conectado como Servidor/Host", Color.green);
            }
            else
            {
                UpdateStatus($"Conectado al servidor", Color.green);
            }
            DisableMenuButtons();
            
            // Show room panel after successful connection
            if (roomManager != null)
            {
                roomManager.ShowRoomPanel(playerName);
            }
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            if (isServer)
            {
                UpdateStatus("Desconectado del servidor", Color.red);
            }
            else
            {
                UpdateStatus("Desconectado del servidor", Color.red);
            }
            EnableMenuButtons();
        }
    }
    
    void UpdateStatus(string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
        Debug.Log($"Status: {message}");
    }
    
    void DisableMenuButtons()
    {
        createServerButton.interactable = false;
        connectClientButton.interactable = false;
        playerNameInput.interactable = false;
        serverIPInput.interactable = false;
    }
    
    void EnableMenuButtons()
    {
        bool hasName = !string.IsNullOrEmpty(playerName);
        createServerButton.interactable = hasName;
        connectClientButton.interactable = hasName;
        playerNameInput.interactable = true;
        serverIPInput.interactable = true;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (networkManager != null)
        {
            networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
        }
    }
    
    // Public method to disconnect
    public void Disconnect()
    {
        if (networkManager != null)
        {
            if (networkManager.IsServer)
            {
                networkManager.ServerManager.StopConnection(true);
            }
            if (networkManager.IsClient)
            {
                networkManager.ClientManager.StopConnection();
            }
        }
    }
}