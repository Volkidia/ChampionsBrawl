using UnityEngine;
using System.Collections;

public class LDManager : MonoBehaviour {
    GameObject[] objEmisive;
    public int time;
	// Use this for initialization
	void Start () {
        objEmisive = GameObject.FindGameObjectsWithTag("ObjEmissive");
            
            iTween.ValueTo(gameObject, iTween.Hash("from", new Color(0, 160, 255),"to", new Color(255, 0, 0), "time", time, "looptype", iTween.LoopType.pingPong, "onupdate","SetColor"));

	}
	
	// Update is called once per frame
	void SetColor (Color color) {
        foreach (GameObject obj in objEmisive)
        {
            Renderer ObjMat = obj.GetComponent<Renderer>();
            ObjMat.material.SetColor("_GlowColor", color);
        }

    }
}
