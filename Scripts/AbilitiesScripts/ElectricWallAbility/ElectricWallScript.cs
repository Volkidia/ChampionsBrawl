using UnityEngine;
using System.Collections;

public class ElectricWallScript : HitInfos {

    public GameObject ElectricWallGO;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(other.gameObject);
        }
    }

    public void Touched(HitInfos hInfos)
    {
        if (hInfos.idSource != idSource)
        {
            hInfos.DestroyThis();
        }
    }

    override public void DestroyThis()
    {
        if(gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(gameObject);
        }
    }
}
