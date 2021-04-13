using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShieldScript : NetworkBehaviour {

    public AbilityShield PlayerAbility;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update() { 

	}

    public void Touched(HitInfos hInfos)
    {
        /*if (hInfos.idSource != netId)
        {
            PlayerAbility.ShieldTriggered();
            hInfos.DestroyThis();
            Destroy(this.gameObject);
        }*/
    }
}
