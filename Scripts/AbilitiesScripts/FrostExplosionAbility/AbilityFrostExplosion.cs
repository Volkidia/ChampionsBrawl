using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AbilityFrostExplosion : _AbilityParentClass
{

    //Initialisation parameters----------------------------
    public string abName = "ExplosionDeGivreP10";
    public float maxCooldown = 10.0f;
    public int nbActivation = 1;
    public float CastingTime = 0.5f;
    [Space(5)]
    public float[] WaitingTimes;
    //Ability Effect utilities------------------------------

    [Space(15)]
    [Header("Abilities Utilities", order = 1)]
    [Space(10)]
    [Header("Frost Explosion parameters", order = 2)]

    public HitInfos FrostExplosionScript;
    public GameObject FrostExplosionGO;
    private GameObject FrostExplosionInstance;

    public float FrostExplosionDuration;
    public int FrostExplosionDamages;

    private Vector3 originalScale;
    public Vector3 ScaleExplosionMax;

    [Header("Frost Explosion Afterglow parameters", order = 3)]

    public GameObject AfterglowFrostGO;
    private GameObject AfterglowFrostInstance;

    public float AfterglowFrostDuration;

    private Vector3 AfterglowOriginalScale;
    //private Vector3 AfterglowDestinationScale;


    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
        //charCtrl = gameObject.GetComponent<CharacterController>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!FrostExplosionScript && FrostExplosionGO)
        {
            FrostExplosionScript = FrostExplosionGO.GetComponent<HitInfos>();
        }
        if (FrostExplosionScript)
        {
            FrostExplosionScript.damages = FrostExplosionDamages;
        }
    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        switch (actualsActivations)
        {
            case 0:
                #region
                StartCoroutine(_rFrostExplosionAppear(FrostExplosionDuration - 0.05f));
                StartCoroutine(_rAfterglowFrostExplosion(FrostExplosionDuration - 0.05f + FrostExplosionDuration + AfterglowFrostDuration));
                #endregion
                break;
            default:
                break;
        }
    }

    IEnumerator _rFrostExplosionAppear(float duration)
    {
        yield return new WaitForSeconds(duration);
        Cmd_FrostExplosion(NetIDSource);
    }

    [Command]
    void Cmd_FrostExplosion(NetworkInstanceId _idsource)
    {
        FrostExplosionInstance = Instantiate(FrostExplosionGO, transform.position, transform.rotation) as GameObject;
        FrostExplosionInstance.transform.parent = transform;
        
        NetworkServer.SpawnWithClientAuthority(FrostExplosionInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(FrostExplosionInstance, FrostExplosionDuration);
        
    }

    IEnumerator _rAfterglowFrostExplosion(float duration)
    {
        Cmd_AfterglowFrostExplosion(NetIDSource);
        yield return new WaitForSeconds(duration);
    }

    /*IEnumerator _rScaleFrostExplosion(float duration)
    {
        originalScale = FrostExplosionInstance.transform.localScale;
        float currentTime = 0.0f;
        do
        {
            FrostExplosionInstance.transform.localScale = Vector3.Lerp(originalScale, ScaleExplosionMax, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);
        Destroy(FrostExplosionInstance);
        StartCoroutine(_rAfterglowFrostExplosion(FrostExplosionDuration + AfterglowFrostDuration));
    }*/

    [Command]
    void Cmd_AfterglowFrostExplosion(NetworkInstanceId _idsource)
    {
        AfterglowFrostInstance = Instantiate(AfterglowFrostGO, transform.position, transform.rotation) as GameObject;
        AfterglowFrostInstance.transform.localScale = ScaleExplosionMax;
        NetworkServer.SpawnWithClientAuthority(AfterglowFrostInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(AfterglowFrostInstance, AfterglowFrostDuration);
    }
}
