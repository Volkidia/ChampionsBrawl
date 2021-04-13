using UnityEngine;
using System.Collections;

public class scrollbackground : MonoBehaviour {

    public float speed = 0.5f;

    private Vector2 offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        offset = new Vector2(Time.time * speed,0);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
	}
}
