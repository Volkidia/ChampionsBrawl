using UnityEngine;
using System.Collections;

public class Ability01 : _AbilityParentClass {

    //Initialisation parameters----------------------------
    public string abName = "TeleportationP10";
    public float maxCooldown = 10.0f;
    public int nbActivation = 2;
    public float CastingTime = 1.5f;
    [Space(5)]
    public float[] WaitingTimes;


    //Ability Effect utilities------------------------------

    [Space(15)]
    [Header("Abilities Utilities", order = 1)]
    [Space(10)]

    [Header("Return Position parameter (1st & 2nd Activation)", order = 2)]
    //public
    public GameObject returnPosPrefab = null;
    public float range = 3.0f;
    public Color _returnPosDebugColor = Color.green;


    [Header("AoE Slow Area (start point) 1st Activation", order = 3)]
    public GameObject slowAreaPrefab = null;
    public float slowValue = .2f;
    public Vector3 slowAreaSpawnPos = Vector3.zero;
    public float slowAreaDuration = 2.5f;
    public Color _slowAreaDebugColor = new Color(0,.5f,0,.3f);

    [Header("Splash Damages Impact (end point) 1st Activation", order = 4)]
    public GameObject splashDamagesPrefab = null;
    public int aoeDamages = 10;
    public Vector3 splashDamagesSpawnPos = Vector3.zero;
    public float splashDamagesFxsDuration = .5f;
    public Color _splashDamagesImpactDebugColor = new Color(1,0,0,.5f);

    //private
    private GameObject _refReturnPosGO = null;
    private IEnumerator _rDestroyBackPos_routineRef = null;

    private SlowArea _slowAreaScriptRef = null;
    private GameObject _refSlowArea = null;
    private IEnumerator _rDestroySlowArea_routineRef = null;

    private SplashDamagesAoE _sAoEScriptRef = null;
    private GameObject _refSplashDmgFXs = null;
    private IEnumerator _rDestroySlashDamagesFXs_routineRef = null;

    


    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName,playerId ,maxCooldown, nbActivation, CastingTime, WaitingTimes);
    }

    // Use this for initialization
    void Start () {
        if(doDebug)
            StartCoroutine(_rDebug(.5f));

        if(splashDamagesPrefab)
            _sAoEScriptRef = splashDamagesPrefab.GetComponent<SplashDamagesAoE>();

        if(slowAreaPrefab)
            _slowAreaScriptRef = slowAreaPrefab.GetComponent<SlowArea>();
     }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        switch(actualsActivations)
        {
            case 0:
                #region SlowArea Part
                if(slowAreaPrefab && _slowAreaScriptRef)
                {
                    _refSlowArea = Instantiate(slowAreaPrefab, transform.position + slowAreaSpawnPos, Quaternion.identity) as GameObject;

                    if(_rDestroySlowArea_routineRef != null)
                    {
                        StopCoroutine(_rDestroySlowArea_routineRef);
                        _rDestroySlowArea_routineRef = null;
                    }

                    _rDestroySlowArea_routineRef = _rDestroySlowArea(slowAreaDuration);
                    StartCoroutine(_rDestroySlowArea_routineRef);
                }

                #endregion

                #region position Part
                if(returnPosPrefab)
                {
                    if(_rDestroyBackPos_routineRef != null)
                        StopCoroutine(_rDestroyBackPos_routineRef);

                    if(!_refReturnPosGO)
                    {
                        Destroy(_refReturnPosGO);
                        _refReturnPosGO = null;
                    }

                    _refReturnPosGO = Instantiate(returnPosPrefab, transform.position, transform.rotation) as GameObject;

                    _rDestroyBackPos_routineRef = _rDestroyBackPos(WaitingTimes[0]);
                    StartCoroutine(_rDestroyBackPos_routineRef);
                }

                transform.parent.localPosition += JoystickInput.normalized * range;
                #endregion

                #region Impact AoE Damage
                if(splashDamagesPrefab && _sAoEScriptRef)
                {
                    _sAoEScriptRef.Damages = aoeDamages;
                    _refSplashDmgFXs = Instantiate(splashDamagesPrefab, transform.position + splashDamagesSpawnPos, Quaternion.identity) as GameObject;

                    if(_rDestroySlashDamagesFXs_routineRef != null)
                    {
                        StopCoroutine(_rDestroySlashDamagesFXs_routineRef);
                        _rDestroySlashDamagesFXs_routineRef = null;
                    }

                    _rDestroySlashDamagesFXs_routineRef = _rDestroySplashDamagesFx(splashDamagesFxsDuration);
                    StartCoroutine(_rDestroySlashDamagesFXs_routineRef);
                }
                #endregion
                break;
            case 1:
                #region position Part
                if(_refReturnPosGO)
                {
                    transform.parent.position = _refReturnPosGO.transform.position;
                    transform.parent.rotation = _refReturnPosGO.transform.rotation;

                    Destroy(_refReturnPosGO);
                    _refReturnPosGO = null;
                }
                
                if(_rDestroyBackPos_routineRef != null)
                    StopCoroutine(_rDestroyBackPos_routineRef);
                #endregion
                break;
            default:
                break;
        }
    }

    IEnumerator _rDestroyBackPos(float duration)
    {
        yield return new WaitForSeconds(duration);
        if(_refReturnPosGO)
        {
            Destroy(_refReturnPosGO);
            _refReturnPosGO = null;
        }
    }

    IEnumerator _rDestroySlowArea(float duration)
    {
        yield return new WaitForSeconds(duration);
        if(_refSlowArea)
        {
            Destroy(_refSlowArea);
            _refSlowArea = null;
        }
        yield return null;
    }

    IEnumerator _rDestroySplashDamagesFx(float duration)
    {
        yield return new WaitForSeconds(duration);
        if(_refSplashDmgFXs)
        {
            Destroy(_refSplashDmgFXs);
            _refSplashDmgFXs = null;
        }
        yield return null;
    }

    IEnumerator _rDebug(float updateRate)
    {
        do
        {
            //Debug.Log("Game Time : " + Time.time + " || Current Cooldown = " + getCurrentCD() + " || Current Activation Number = " + currentNbActivations);
            yield return new WaitForSeconds(updateRate);
        } while(doDebug);
    }


    void OnDrawGizmos()
    {
        if(doDebug)
        {
            Vector3[] _directions = {
            new Vector3(0,1,0),
            new Vector3(0,-1,0),
            new Vector3(0,0,1),
            new Vector3(0,0,-1),
            new Vector3(0,.5f,.5f),
            new Vector3(0,.5f,-.5f),
            new Vector3(0,-.5f,.5f),
            new Vector3(0,-.5f,-.5f)};

            Gizmos.color = _returnPosDebugColor;

            foreach(Vector3 dir in _directions)
            {
                Gizmos.DrawWireSphere(transform.position + (dir.normalized * range), .5f);
            }

            if(splashDamagesPrefab)
            {
                Gizmos.color = _splashDamagesImpactDebugColor;       
                
                Vector3 prefabScale = splashDamagesPrefab.transform.localScale;
                float rad = splashDamagesPrefab.GetComponent<SphereCollider>().radius;
                float modifier = Mathf.Max(prefabScale.x,Mathf.Max(prefabScale.y,prefabScale.z));

                Gizmos.DrawSphere(transform.position + splashDamagesSpawnPos, rad * modifier);
            }

            if(slowAreaPrefab)
            {
                Gizmos.color = _slowAreaDebugColor;

                Vector3 slowPrefabScale = slowAreaPrefab.transform.localScale;
                float slowRad = slowAreaPrefab.GetComponent<SphereCollider>().radius;
                float slowMod = Mathf.Max(new float[]  {slowPrefabScale.x, slowPrefabScale.y, slowPrefabScale.z });

                Gizmos.DrawSphere(transform.position + slowAreaSpawnPos, slowRad * slowMod);
            }
        }
    }
}
