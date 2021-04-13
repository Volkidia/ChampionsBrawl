using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AbilityWindExplosion : _AbilityParentClass
{

    //Initialisation parameters----------------------------
    public string abName = "ExplosionDeVentP4";
    public float maxCooldown = 10.0f;
    public int nbActivation = 1;
    public float CastingTime = 0.5f;
    [Space(5)]
    public float[] WaitingTimes;
    //Ability Effect utilities------------------------------

    [Space(15)]
    [Header("Abilities Utilities", order = 1)]
    [Space(10)]
    [Header("Wind Explosion parameters", order = 2)]

    public HitInfos WindExplosionScript;
    public GameObject WindExplosionGO;
    private GameObject WindExplosionInstance;

    public float WindExplosionDuration;
    public int WindExplosionDamages;

    private Vector3 originalScale;
    public Vector3 destinationScale;

    [Header("Wind Blade parameters", order = 3)]

    public HitInfos WindBladeScript;
    public Vector3[] DirectionTable;
    public GameObject WindBladeGO;

    public float spawnDistance;
    public float forceInit;
    public float WindBladeDuration;
    public int WindBladeDamages;

    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
        //charCtrl = gameObject.GetComponent<CharacterController>();
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    { 
        if (!WindExplosionScript && WindExplosionGO)
        {
            WindExplosionScript = WindExplosionGO.GetComponent<HitInfos>();
        }
        if (WindExplosionScript)
        {
           WindExplosionScript.damages = WindExplosionDamages;
        }
        if (!WindBladeScript && WindBladeGO)
        {
            WindBladeScript = WindBladeGO.GetComponent<HitInfos>();
        }
        if (WindBladeScript)
        {
            WindBladeScript.damages = WindBladeDamages;
        }
    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        switch (actualsActivations)
        {
            case 0:
                #region
                StartCoroutine(_rWindExplosionAppear(WindExplosionDuration - 0.05f));
                #endregion
                break;
            default:
                break;
        }
    }

    IEnumerator _rWindExplosionAppear(float duration)
    {
        Cmd_WindExplosion(NetIDSource);
        Cmd_WindBlades(NetIDSource);
        yield return new WaitForSeconds(duration);
    }

    [Command]
    void Cmd_WindExplosion(NetworkInstanceId _idsource)
    {
        WindExplosionInstance = Instantiate(WindExplosionGO, transform.position, transform.rotation) as GameObject;
        WindExplosionInstance.transform.parent = transform;
        NetworkServer.SpawnWithClientAuthority(WindExplosionInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(WindExplosionInstance,WindExplosionDuration);
    }
    
    [Command]
    void Cmd_WindBlades(NetworkInstanceId _idsource)
    {
        if (WindBladeGO)
        {
            for (int i = 0; i < DirectionTable.Length; i++)
            {
                Vector3 _tmpSpawnPos = transform.position + (DirectionTable[i] * spawnDistance);
                GameObject _tmpWindBlade = Instantiate(WindBladeGO, _tmpSpawnPos, Quaternion.identity) as GameObject;
                _tmpWindBlade.transform.forward = DirectionTable[i];
                _tmpWindBlade.GetComponent<Rigidbody>().velocity = forceInit * _tmpWindBlade.transform.forward;
                NetworkServer.SpawnWithClientAuthority(_tmpWindBlade, NetworkServer.FindLocalObject(_idsource));
                Destroy(_tmpWindBlade, WindBladeDuration);

            }
        }
    }
}
