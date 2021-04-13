using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilitiesIndex : MonoBehaviour {

    public _AbilityParentClass[] abilitiesList;
    static public Dictionary<string,_AbilityParentClass> abilities = new Dictionary<string, _AbilityParentClass>();
    static public bool initState = false;

    void Awake()
    {
    }

	// Use this for initialization
	void Start ()
    {
        abilitiesList = GetComponents<_AbilityParentClass>();
        abilities = initDictionary(abilitiesList);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private Dictionary<string, _AbilityParentClass> initDictionary(_AbilityParentClass[] sourceList)
    {
        Dictionary<string,_AbilityParentClass> _rDictionary = new Dictionary<string, _AbilityParentClass>();

        foreach(_AbilityParentClass ab in sourceList)
        {
            if(!_rDictionary.ContainsKey(ab.abilityName))
            {
                ab.AbilityInit(-1);
                _rDictionary.Add(ab.abilityName, ab);
                Debug.Log(ab.abilityName);
            }                
        }
        initState = true;

        //Debug.Log(_rDictionary.Count);
        return _rDictionary;
    }

    private _AbilityParentClass _getAbility(string key)
    {
        return null;
    }

    static public _AbilityParentClass getAbiliy(string abilityName)
    {
        _AbilityParentClass retV = null;

        if(abilities.ContainsKey(abilityName))
            retV = abilities[abilityName];

        return retV;
    }
}
