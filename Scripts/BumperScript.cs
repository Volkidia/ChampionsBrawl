using UnityEngine;
using System.Collections;

public class BumperScript : MonoBehaviour {

    public Vector3 VelocityBumper;

	// Use this for initialization
	void Start ()
    {

	}
	
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.SendMessage("Bumper", VelocityBumper, SendMessageOptions.DontRequireReceiver);
        }
    }
}

