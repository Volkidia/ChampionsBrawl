using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour {

    public Animator anim;
    public Vector3 bumpDir;
    public float bumpSpeed;
    public LayerMask playerLayers;


	// Use this for initialization
	void Start () {
        anim = anim ? anim : GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.A))
            setTrigger();
	}

    void OnTriggerEnter(Collider coll)
    {
        if(PhysicsBox.isInlayerMask(coll.gameObject, playerLayers))
        {
            setTrigger();
            coll.SendMessage("cMove_Impulse", bumpDir.normalized * bumpSpeed, SendMessageOptions.DontRequireReceiver);
        }
    }

    void setTrigger()
    {
        anim.SetTrigger("Bump");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, .3f);
        Gizmos.DrawSphere(transform.position + bumpDir, .25f);
    }
}
