using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net.NetworkInformation;


public class LobbySceneManager : MonoBehaviour {

    static bool ClientNotStart = true;
    NetManager NetManager;
    string IPAdress;

    public GameObject Loading;

    int Timer;
	// Use this for initialization
	void Start () {
        NetManager = GameObject.Find("NetworkingManager").GetComponent<NetManager>();
        if (ClientNotStart)
        {
            IPAdress = NetManager.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            if (NetManager.IsHost)
            {
                ClientNotStart = false;
                NetManager.StartHost();
                

            }
        }
	}


	// Update is called once per frame
	void FixedUpdate () {
       if (ClientNotStart & IPAdress != NetManager.networkAddress)
        {
            ClientNotStart = false;
            NetManager.StartClient();
            
        }
        Loading.SetActive(ClientNotStart);
	}
}
