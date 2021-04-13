using UnityEngine;
using System.Collections;

public class Test_Projectiles : HitInfos {

    public Rigidbody rb;
    


	// Use this for initialization
	void Start () {
        init();
        rb = rb ? rb : GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        int side = 1;
        if(Input.GetKey(KeyCode.RightShift))
            side = -1;

        if(Input.GetKey(KeyCode.Space))
            rb.velocity = new Vector3(0, 0, 3 * side);
        else
            rb.velocity = Vector3.zero;
	}
}
