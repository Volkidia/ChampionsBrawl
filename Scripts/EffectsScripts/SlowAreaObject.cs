using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowAreaObject : HitInfos
{
    private List<Collider> collList = new List<Collider>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnTriggerEnter(Collider coll)
    {
        base.OnTriggerEnter(coll);
        collList.Add(coll);
    }

    void OnTriggerExit(Collider coll)
    {
        if(collList.Contains(coll))
        {
            coll.SendMessage("RemoveSlowAreaEffect", Effects[0], SendMessageOptions.DontRequireReceiver);
            collList.Remove(coll);
        }
    }

    void OnDestroy()
    {
        foreach(Collider coll in collList)
           coll.SendMessage("RemoveSlowAreaEffect", Effects[0], SendMessageOptions.DontRequireReceiver);
    }
}
