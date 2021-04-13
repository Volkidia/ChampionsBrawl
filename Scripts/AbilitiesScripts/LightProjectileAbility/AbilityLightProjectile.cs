using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AbilityLightProjectile : _AbilityParentClass
{

    //Initialisation parameters----------------------------
    public string abName = "ProjectileDeLumiereP10";
    public float maxCooldown = 10.0f;
    public int nbActivation = 1;
    public float CastingTime = 0.5f;
    [Space(5)]
    public float[] WaitingTimes;
    //Ability Effect utilities------------------------------

    [Space(15)]
    [Header("Abilities Utilities", order = 1)]
    [Space(10)]
    [Header("Light Projectile parameters", order = 2)]

    public HitInfos LightProjectileScript;
    public GameObject LightProjectileGO = null;
    public GameObject ProjectileSignGO = null;

    public float LightProjectileDuration;
    public float SignDuration;
    public int LightProjectileDamages = 20;

    private GameObject LightProjectileInstance = null;
    private GameObject ProjectileSignInstance = null;

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
        if (!LightProjectileScript && LightProjectileGO)
        {
            LightProjectileScript = LightProjectileGO.GetComponent<HitInfos>();
        }
        if (LightProjectileScript)
        {
            LightProjectileScript.damages = LightProjectileDamages;
        }
    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        switch (actualsActivations)
        {
            case 0:
                #region
                Cmd_SpawnSignProjectile(JoystickInput, NetIDSource);
                StartCoroutine(_rDestroySignProjectile(SignDuration, JoystickInput));
                #endregion
                break;
            default:
                break;
        }
    }

    [Command]
    void Cmd_SpawnSignProjectile(Vector3 dir, NetworkInstanceId _idsource)
    {
        ProjectileSignInstance = Instantiate(ProjectileSignGO, transform.position, transform.rotation) as GameObject;
        ProjectileSignInstance.transform.forward = dir.normalized;
        NetworkServer.SpawnWithClientAuthority(ProjectileSignInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(ProjectileSignInstance, SignDuration);
    }

    IEnumerator _rDestroySignProjectile(float duration, Vector3 dir)
    {
        yield return new WaitForSeconds(duration);
        Cmd_SpawnLightProjectile(dir, NetIDSource);
    }
    
    [Command]
    void Cmd_SpawnLightProjectile(Vector3 dir, NetworkInstanceId _idsource)
    {
        LightProjectileInstance = Instantiate(LightProjectileGO, transform.position, transform.rotation) as GameObject;
        LightProjectileInstance.transform.forward = dir.normalized;
        NetworkServer.SpawnWithClientAuthority(LightProjectileInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(LightProjectileInstance, LightProjectileDuration);
    }
}
