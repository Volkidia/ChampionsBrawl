using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AbilityShield : _AbilityParentClass {

    //Initialisation parameters----------------------------
    public string abName = "BouclierDeFoudreP5";
    public float maxCooldown = 10.0f;
    public int nbActivation = 2;
    public float CastingTime = 0.5f;
    [Space(5)]
    public float[] WaitingTimes;
    //Ability Effect utilities------------------------------

    [Space(15)]
    [Header("Abilities Utilities", order = 1)]
    [Space(10)]
    [Header("Shield parameters", order = 2)]
    public GameObject shieldGO;
    public GameObject playerGO;

    public float offsetShield;
    public float shieldDuration;

    private Vector3 originalShieldScale;
    private Vector3 destinationShieldScale;
    private CharacterController charCtrl;
    private bool isTriggered;

    [Header("Projectiles parameters", order = 3)]

    public GameObject lightning = null;

    public float lightningDuration;
    public float LightningScale;
    public float lightningSpawnDist;
    public int lightningDamages = 20;

    public HitInfos lightningScript;

    private GameObject lightningInstance = null;

    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
        charCtrl = gameObject.GetComponentInParent<CharacterController>();
        destinationShieldScale = new Vector3(charCtrl.height + offsetShield, charCtrl.height + offsetShield, charCtrl.height + offsetShield);
        shieldGO.GetComponent<SphereCollider>();
    }

    // Use this for initialization
    void Start()
    {
        isTriggered = false;
    }
	// Update is called once per frame
	void Update ()
    {
        if (!lightningScript && lightning)
        {
            lightningScript = lightning.GetComponent<HitInfos>();
        }
        if (lightningScript)
        {
            lightningScript.damages = lightningDamages;
        }
    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        switch (actualsActivations)
        {
            case 0:
                #region
                isTriggered = false;
                Cmd_Shield(NetIDSource);
                #endregion
                break;
            case 1:
                #region
                Cmd_SpawnLightning(JoystickInput, NetIDSource);
                if (isTriggered)
                { 
                    isTriggered = false;
                }
                #endregion
                break;
            default:
                break;
        }
    }

    [Command]
    void Cmd_Shield(NetworkInstanceId _idsource)
    {
        GameObject shieldInstance = Instantiate(shieldGO, transform.position, transform.rotation) as GameObject;
        shieldInstance.GetComponent<ShieldScript>().PlayerAbility = this;
        shieldInstance.transform.localScale = destinationShieldScale;
        shieldInstance.transform.parent = transform;
        NetworkServer.SpawnWithClientAuthority(shieldInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(shieldInstance, shieldDuration);
    }

    [Command]
    void Cmd_SpawnLightning(Vector3 dir, NetworkInstanceId _idsource)
    {

        lightningInstance = Instantiate(lightning, transform.position + dir * lightningSpawnDist, transform.rotation) as GameObject;
        lightningInstance.transform.forward = dir.normalized;

        NetworkServer.SpawnWithClientAuthority(lightningInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(lightningInstance, lightningDuration);
    }

    public void ShieldTriggered()
    {
        isTriggered = true;
    }
}
