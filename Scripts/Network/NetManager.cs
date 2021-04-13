using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class NetManager : NetworkManager {
    [Header ("InfosNetwork")]
    public int MaxPlayers;
    public int Port;
    public static bool IsHost = false;

    [Space(2)]
    [Header("Time")]
    public int Timer;
    public int GameTime;
    public int LobbyTime;
    public int ChainKillTime;
    public GameObject PrefabTimer;
    Timer InstanceTimer;

    [Space(2)]
    [Header("PrefabPlayer")]
    public GameObject PrefabPlayer;
    public GameObject PrefabPlayerLobby;
    public GameObject PrefabPlayerHUD;
    public GameObject PrefabPlayerScore;

    [Space(2)]
    [Header("PrefabAnnounce")]
    public GameObject TripleKill;
    public GameObject DoubleKill;

    Transform CurrentParentPlayer;

    int ChainKill = 0;
    
    static KillerDeadTable DataDie;

    void Start()
    {
        
        this.playerPrefab = PrefabPlayerLobby;

        networkAddress = GetLocalIPv4(NetworkInterfaceType.Ethernet);

        Timer = LobbyTime;
        DontDestroyOnLoad(this);
        
    }



    #region Parent & PlayerPrefab Manager

    //ChangeParent and PlayerPrefab----------------------
    public Transform GetParentPlayer()
    {
        return GameObject.FindGameObjectWithTag("Root").transform;
    }

    #endregion



    // Take Pc s Ip Adress -------------------------------
    public string GetLocalIPv4(NetworkInterfaceType _type)
    {
        string output = "";
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        output = ip.Address.ToString();
                    }
                }
            }
        }
        return output;
    }




    #region UDP Manager
    public void SendUDP()
    {
        UDPSend sender = new UDPSend();
        sender.Init(networkAddress, Port);
        StartCoroutine(sendIP(sender));
    }
    public void ListenUDP()
    {
        UDPListen Listener = new UDPListen();
        Listener.Init(Port);
        StartCoroutine(listenIP(Listener));
    }

    //Coroutine For Send Or Listen Ip Address
    IEnumerator sendIP(UDPSend sender)
    {
        while (this.numPlayers != MaxPlayers)
        {
            Debug.Log("send");
            sender.sendData();
            yield return new WaitForSeconds(2);
        }
        Debug.Log("go");
        ReadyTime();
        StopCoroutine(sendIP(sender));
    }

    IEnumerator listenIP (UDPListen listener)
    {
        string IPAddress = "";
        while (IPAddress == "")
        {
            IPAddress = listener.GetUDPinfo();
            yield return null;
        }
        networkAddress = IPAddress;
        StopCoroutine(listenIP(listener));
    }
    #endregion

    // Time wait before change lobby scene to play scene
    void ReadyTime()
    {
        NetworkServer.SpawnObjects();
    }


    #region Server fct Callback
    //Server Callback --------------------

    public override void OnStartServer()
    {
        base.OnStartServer();
        SendUDP();
        CurrentParentPlayer = GetParentPlayer();
        NetworkServer.RegisterHandler(Message.SpawnPrefab, SpawnPrefab);
        NetworkServer.RegisterHandler(Message.Die, SetDieData);
        DataDie = new KillerDeadTable();

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        CallOnSceneChange("LobbyScene");

    }
 
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        switch (sceneName)
        {
            case "LobbyScene":
                playerPrefab = PrefabPlayerLobby;
                SendUDP();
                Timer = LobbyTime;
                break;

            case "MainScene":
                playerPrefab = PrefabPlayer;
                Timer = GameTime;
                NetworkServer.SpawnObjects();
                break;

            case "ScoreScene":
                playerPrefab = PrefabPlayerScore;
                int[] Order = DataDie.GetDeathOrder();
                foreach (int clientId in Order)
                {
                    NetworkServer.SendToClient(clientId, Message.SetScore, new SetScoreMessage(System.Array.IndexOf(Order, clientId),DataDie.GetNbKillOf(clientId),DataDie.GetNbDeathOf(clientId), DataDie.GetNemesis(clientId)));
                }
                break; 
        }
    }
    #endregion

    #region Client Fct CallBack
    //ClientCallback --------------------
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        client.RegisterHandler(Message.SetScore, GetDieData);
    }

    #endregion

    #region Fct Message Handler

    // ChangeScene --------------------------------------------------------

    public void CallOnSceneChange (string Name)
    {
        
        ServerChangeScene(Name);
    }

    void SpawnOnServer(GameObject _prefab, Vector3 _pos)
    {

        GameObject Obj = Instantiate(_prefab, _pos, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(Obj);
    }

    // Spawn the object on client scene wich have a NetworkIdentity -------
    private void SpawnPrefab (NetworkMessage netMsg)
    {
        SpawnPrefabMessage msg = netMsg.ReadMessage<SpawnPrefabMessage>();
        GameObject obj;
        obj = (GameObject)GameObject.Instantiate(PrefabPlayerHUD, msg.Position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(obj, netMsg.conn);
    }



    private void SetDieData(NetworkMessage netMsg)
    {
        
        DieMessage msg = netMsg.ReadMessage<DieMessage>();
        int conId = netMsg.conn.connectionId;
        DataDie.AddKill(conId, msg.KillerId, Timer, msg.LastLife);
        Debug.Log("jenvoie le message");
        if (ChainKill == 0)
        {
            StartCoroutine(TimeBetweenKill());
            ChainKill = 1;
        }
        else
        {
            ChainKill++;
            switch (ChainKill)
            {
                case 2:
                    SpawnOnServer(DoubleKill, Vector3.zero);
                    break;
                case 3:
                    SpawnOnServer(TripleKill, Vector3.zero);
                    break;
            }
            
        }
        if (Network.connections.Length == 1)
        {
            CallOnSceneChange("ScoreScene");
        }
        
    }

    private void GetDieData(NetworkMessage netMsg)
    {
        SetScoreMessage msg = netMsg.ReadMessage<SetScoreMessage>();
        StaticData.Nemesis = msg.Nemesis;
        StaticData.NbKill = msg.NbKill;
        StaticData.NbDead = msg.NbDead;
        StaticData.Order = msg.order;
        
        

    }
    #endregion

    IEnumerator TimeBetweenKill()
    {
        yield return new WaitForSeconds(ChainKillTime);
        ChainKill = 0;
    }

}
