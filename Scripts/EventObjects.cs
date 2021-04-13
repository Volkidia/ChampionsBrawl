using UnityEngine;
using System.Collections;

public class EventObjects : MonoBehaviour {


	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    virtual public void Event(int noEvent)
    {
        Debug.Log(noEvent);
    }
}
