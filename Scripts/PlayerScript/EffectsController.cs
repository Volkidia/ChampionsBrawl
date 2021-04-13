using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class EffectsController : NetworkBehaviour {

    public Coordinator coor;

    public Dictionary<System.Type,_EFfectParent> efDico = new Dictionary<System.Type, _EFfectParent>();

    public List<_EFfectParent> _efList = new List<_EFfectParent>();

    /*
    [Command]
    void Cmd_AddEffect(GameObject efObj, _EFfectParent ef)
    {
        bool _doAdd = true;
        if(ef.isUnique && efDico.ContainsKey(ef.GetType()))
            if(ef.isRefresh)
            {
                _doAdd = false;
                efDico[ef.GetType()].RefreshEffect(ef);
            } else
            {
                ef.EndEffect();
            }

        if(_doAdd)
        {
            GameObject _go = Instantiate(efObj,transform.position, Quaternion.identity) as GameObject;
            _EFfectParent _efp = _go.GetComponent<_EFfectParent>();
            if(_efp)
            {
                _go.transform.parent = transform;
                efDico.Add(_efp.GetType(), _efp);
                _efp.InitEffect(coor, this);
                NetworkServer.SpawnWithClientAuthority(_go, gameObject);
            } else
                Destroy(_go);
        }
    }
    */
	// Use this for initialization
	void Start () {
        coor = coor ? coor : GetComponent<Coordinator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Add / Refresh Multiple effect scripts at once
    /// </summary>
    /// <param name="efs">effect array to process</param>
    public void addEffects(_EFfectParent[] efs)
    {
        foreach(_EFfectParent _ef in efs)
        {
            addEffect(_ef);
        }
    }

    public void addEffects(GameObject[] efObjs)
    {
        foreach(GameObject _efGo in efObjs)
        {
            addEffect(_efGo);
        }
    }

    private bool alreadyExist(List<_EFfectParent> listCheck, _EFfectParent ef, out _EFfectParent outEf)
    {
        outEf = null;
        bool retV = false;
        foreach(_EFfectParent _ef in listCheck)
        {
            if(_ef.GetType().Equals(ef.GetType()))
            {
                outEf = _ef;
                retV = true;
            }
        }
        return retV;
    }

    /// <summary>
    /// Add / Refresh an effect to the player
    /// </summary>
    /// <param name="ef">the effect to add / Refresh</param>
    public void addEffect(_EFfectParent ef)
    {
        bool isUnique = ef.isUnique;
        bool isRefresh = ef.isRefresh;
        isUnique = isRefresh ? true : isUnique;
        bool doAdd = true;

        _EFfectParent _tempEf;

        Debug.Log(ef.GetType());

        if(isUnique && alreadyExist(_efList,ef,out _tempEf))
        {
            Debug.Log(isUnique);
            if(isRefresh)
            {
                _tempEf.RefreshEffect(ef);
                doAdd = false;
            }
            else
            {
                removeEffect(_tempEf);
                _efList.Remove(_tempEf);
            }
        }

        if(doAdd)
        {
            _tempEf = gameObject.AddComponent(ef.GetType()) as _EFfectParent;
            _tempEf.InitEffect(ef);
            _tempEf.StartEffect();
            _efList.Add(_tempEf);
        }
    }

    public void addEffect(GameObject efObject)
    {
        /*
        _EFfectParent _efP = efObject.GetComponent<_EFfectParent>();
        if(_efP)
            Cmd_AddEffect(efObject, _efP);
            */
    }

    /// <summary>
    /// Remove the given _EffectParent from the actuals effects list, and End it
    /// </summary>
    /// <param name="ef">effect to remove</param>
    public void removeEffect(_EFfectParent ef)
    {
        if(_efList.Contains(ef))
        {
            _EFfectParent _tempEf = _efList.Find(item => item == ef);
            _tempEf.effectDestroy();
            _efList.Remove(_tempEf);
        }
    }

    public void removeEffect(System.Type type)
    {
        if(efDico.ContainsKey(type))
        {
            efDico[type].effectDestroy();
            efDico.Remove(type);
        }
    }
}
