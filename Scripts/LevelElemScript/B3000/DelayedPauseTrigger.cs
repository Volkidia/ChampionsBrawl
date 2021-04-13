using UnityEngine;
using System.Collections;

public class DelayedPauseTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider coll)
    {
        coll.SendMessage("delayedPause", SendMessageOptions.DontRequireReceiver);
    }

}
