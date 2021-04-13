using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class _AbilityParentClass: NetworkBehaviour {

    public bool doDebug = false;

    private string _abilityName = "AbilityParent";
    private float _maxCD = 1.0f;
    private float _endCooldownTime = -1.0f;
    private int _nbActivations = 1;
    private float _castTime = 1.0f;
    private float[] _reactivationWaitTimes = {.5f,5f };
    private int _sourcePlayerId = 0;
    public NetworkInstanceId NetIDSource;

    private int _currentNbActivations = 0;
    private IEnumerator _currRoutineWaitTime = null;

    private GameObject SkillInstance;

    //Public Accessors of local variables (Read Only)
    #region Read Only Public Accessors
    public string abilityName {
        get { return _abilityName; }
    }
    public float maxCD {
        get { return _maxCD; }
    }
    public int nbActivations {
        get { return _nbActivations; }
    }
    public float castTime {
        get { return _castTime; }
    }
    public int currentNbActivations {
        get { return _currentNbActivations; }
    }
    public int sourcePlayerId {
        get { return _sourcePlayerId; }
    }
    #endregion
    //End of Read Only Public Accessors --------------

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void initHitInfosId(GameObject[] objects)
    {
        foreach(GameObject _go in objects)
        {
            //_go.GetComponent<HitInfos>().idSource = netId;
        }
    }

    public virtual void AbilityInit(int playerId)
    {

    }

    /// <summary>
    /// Spell base parameters initialisation function
    /// </summary>
    /// <param name="Name">The Ability Visible Name</param>
    /// <param name="maximumCD">The Ability Global Cooldown</param>
    /// <param name="ConcecutiveActivations">Maximum Number of Reactivations possible (min 1)</param>
    /// <param name="CastTime">Casting Time needed for the ability</param>
    /// <param name="waitTimes">Maximum Waiting Time for Reactivations</param>
    public void _abilityInitialisation(string Name, int playerId,float maximumCD, int ConcecutiveActivations, float CastTime, float[] waitTimes)
    {
        _abilityName = Name;
        _sourcePlayerId = playerId;
        _maxCD = maximumCD;
        _nbActivations = ConcecutiveActivations;
        _castTime = CastTime;
        _reactivationWaitTimes = waitTimes;
    }

    /// <summary>
    /// The commun call function to use the ability activations (simple or multiple)
    /// </summary>
    /// <param name="currM_axCD">(out) the maxCD of the ability</param>
    /// <returns>if the ability is ready to use</returns>
    public bool Use(NetworkInstanceId sourcePlayerId, Vector3 joystickInput)
    {
        bool retV = CanUse();
        Debug.Log("blabla");
        NetIDSource = sourcePlayerId;
        if(retV)
        {
            abilityEffect(currentNbActivations, joystickInput);            

            if(_currRoutineWaitTime != null)
            {
                StopCoroutine(_currRoutineWaitTime);
            }
             
            _currentNbActivations++;

            if(currentNbActivations < nbActivations)
            {
                _currRoutineWaitTime = _ApplyWaitReactivationTime(_reactivationWaitTimes[_currentNbActivations-1]);
                StartCoroutine(_ApplyWaitReactivationTime(_reactivationWaitTimes[_currentNbActivations - 1]));
            } else
            {
                applyCD();
            }
        }
        return retV;
    }

    /// <summary>
    /// Set the endCooldownTime variable to the CurrentTime + cdDuration
    /// </summary>
    /// <param name="cdDuration">the applied Cooldown</param>
    public void applyCD()
    {
        _currRoutineWaitTime = null;
        _currentNbActivations = 0;
        _endCooldownTime = Time.time + maxCD;
    }

    /// <summary>
    /// Get the remaining time before the end of the cooldown
    /// </summary>
    /// <returns>the remaining time of the cooldown</returns>
    public float getCurrentCD()
    {
        float retV = -1;
        retV = _endCooldownTime - Time.time;
        return retV;
    }

    /// <summary>
    /// Get the usability state of the ability (cooldown wise), and reset it if usable
    /// </summary>
    /// <returns>the state</returns>
    public bool CanUse()
    {
        return getCurrentCD() <= 0 && currentNbActivations < nbActivations;
    }

    /// <summary>
    /// Reset the current ability using state of th
    /// </summary>
    public void resetAbility()
    {
        _endCooldownTime = 0;
        _currentNbActivations = 0;
    }

    /// <summary>
    /// The effect of the ability
    /// </summary>
    /// <param name="actualsActivations">the current numbers of activations</param>
    /// 

    public virtual void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        Debug.Log("GO : " + gameObject + " Ability : " + this.abilityName);
    }

    IEnumerator _ApplyWaitReactivationTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        applyCD();
    }

    [Command]
    public void Cmd_SpawnSkill(GameObject _prefab, Vector3 _pos, Quaternion _rot)
    {
        GameObject g = Instantiate(_prefab, _pos, _rot) as GameObject;
        NetworkServer.Spawn(g);
    }




    [Command]
    public void Cmd_DestroySkill(GameObject SkillPrefab)
    {
        NetworkServer.Destroy(SkillPrefab);
    }

    public GameObject GetPrefabSpawn()
    {
        return SkillInstance;
    }
}
