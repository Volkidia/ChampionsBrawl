using UnityEngine;
using System.Collections;

public class net_aIndex : MonoBehaviour {

    public GameObject[] goKits;
    public static GameObject[] _goKits;

	// Use this for initialization
	void Start () {
        _goKits = goKits;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static GameObject getGoKit(int id)
    {
        Debug.Log("added");
        if(_goKits[id])
            return _goKits[id];
        else
            return null;
    }
}
