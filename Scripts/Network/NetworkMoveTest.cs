using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkMoveTest : NetworkBehaviour {

	float movespeed = 5;
	Vector3 move;

 	[SyncVar]
	public float movez;

	[Command]
	public void Cmd_Move ( float z){
		movez = z;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			Cmd_Move (Input.GetAxis ("LJoystick_H"));
		}
		move = new Vector3 (0, 0, movez * movespeed);

		this.transform.Translate (move * Time.deltaTime);

	}
}
