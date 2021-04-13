using UnityEngine;
using System.Collections;

public class ColliderManager : MonoBehaviour {

    public Vector3 ModifScale;
	// Use this for initialization
	void Start () {

        transform.localScale = new Vector3(ModifScale.x,ModifScale.y, ModifScale.z);
    }
}
