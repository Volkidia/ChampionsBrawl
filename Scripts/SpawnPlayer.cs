using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpawnPlayer : NetworkBehaviour {
    public GameObject PlayerPrefab;
    public Vector3 Spawn;
	// Use this for initialization
	
	// Update is called once per frame
    [Command]
	void Cmd_Spawn()
    {
        Debug.Log("spawn");
        GameObject Prefab = (GameObject)Instantiate(PlayerPrefab, Spawn , Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(Prefab, connectionToClient);
        
        
	
	}
}
