using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class goKitUse : NetworkBehaviour {

    public _AbilityParentClass[] abArray;

    // Use this for initialization
    void Start () {

        
        if (hasAuthority)
        {  GameObject[] players = GameObject.FindGameObjectsWithTag("player");
        
            foreach (GameObject player in players)
            {

                if (player.GetComponent<NetworkIdentity>().isLocalPlayer == true)
                {
                    player.GetComponent<AbilityController>()._kitUse = this;
                    this.transform.SetParent(player.transform);
                }
             }

        }
            
        abArray = abArray.Length > 0 ? abArray : GetComponents<_AbilityParentClass>();
        foreach(_AbilityParentClass ab in abArray)
        {
            ab.AbilityInit(-1);
        }
        //GetComponentInParent<AbilityController>().SetKitUseGO(this);
	}
	
    public bool useAbility(int id, NetworkInstanceId idSource,Vector3 dir)
    {
        bool retV = false;
        if(abArray.Length > id)
        {
            retV = abArray[id].Use(idSource, dir);
            Debug.Log("goKitUse - usAbility N° : " + id);
        }
        return retV;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
