using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetworkManager : MonoBehaviour {

    NetworkClient myClient;

    public string IpAdress;
    public int PortNumber;
    bool Start = false;
    public GameObject Prefab;
    public GameObject reference;
    
    // Use this for initialization
	void Update () {
        if (!Start)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SetSever();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                SetClient();

            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                SetSever();
                SetLocalClient();
            }
        }
	
	}
	
	// Update is called once per frame

    void SetSever()
    {
        NetworkServer.Listen(PortNumber);
        Debug.Log("server");
    }

    void SetClient()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.Connect(IpAdress, PortNumber);
        Debug.Log("client :" + myClient.serverIp + "," + myClient.serverPort);
        Start = true;
        reference.SendMessage("Cmd_Spawn",SendMessageOptions.DontRequireReceiver);
    }

    void SetLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        Debug.Log("Host");
        Start = true;
    }

    void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("connect to server");
        Start = true;
    }
}
