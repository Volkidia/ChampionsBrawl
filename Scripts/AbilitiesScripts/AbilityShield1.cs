using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityShield1 : _AbilityParentClass {

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

    private Vector3 originalShieldScale;
    private Vector3 destinationShieldScale;
    private CharacterController charCtrl;
    private bool isTriggered;

    [Header("Projectiles parameters", order = 3)]

    public GameObject lightningGO = null;
    public Rigidbody lightningRB = null;
    public Transform lightningTransform;

    public float lightningSpeed;
    public float lightningDuration;
    public int lightningDamages = 20;

    public HitInfos lightningScript;
    public Vector3 lightningSpawnPos = new Vector3(0, 0, 1.5f);

    private Rigidbody lightningInstanceRB = null;
    private GameObject lightningInstance = null;

    private Vector3 originalLightningScale;
    private Vector3 destinationLightningScale;

    public override void AbilityInit(int playerId)
    {
        /*
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
        charCtrl = gameObject.GetComponent<CharacterController>();
        shieldGO.GetComponent<SphereCollider>();**/
    }

    // Use this for initialization
    void Start()
    {
        //destinationShieldScale = new Vector3(charCtrl.height + offsetShield, charCtrl.height + offsetShield, charCtrl.height + offsetShield);
        //isTriggered = false;
    }
	// Update is called once per frame
	void Update ()
    {
        if (!lightningScript && lightningGO)
        {
            lightningScript = lightningGO.GetComponent<HitInfos>();
        }
        if (lightningScript)
        {
            lightningScript.damages = lightningDamages;
        }
    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);
        /*
        switch (actualsActivations)
        {
            case 0:
                #region
                isTriggered = false;
                GameObject shieldInstance = Instantiate(shieldGO, transform.position, transform.rotation) as GameObject;
                shieldInstance.GetComponent<ShieldScript>().PlayerAbility = this;
                shieldInstance.transform.localScale = destinationShieldScale;
                shieldInstance.transform.parent = transform;
                #endregion
                break;
            case 1:
                #region
                if (isTriggered)
                {
                    SpawnLightning();
                    isTriggered = false;
                }
                #endregion
                break;
            default:
                break;
        }
        */
    }

    IEnumerator _rScaleLightning(float duration)
    {
        originalLightningScale = lightningInstance.transform.localScale;
        destinationLightningScale = new Vector3(originalLightningScale.x, originalLightningScale.y, lightningSpeed);
        float currentTime = 0.0f;
        do
        {
            lightningInstance.transform.localScale = Vector3.Lerp(originalLightningScale, destinationLightningScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);
        Destroy(lightningInstance);
    }

    private void SpawnLightning()
    {
        lightningInstanceRB = Instantiate(lightningRB, transform.position + lightningSpawnPos, transform.rotation) as Rigidbody;
        lightningInstance = lightningInstanceRB.gameObject;
        lightningInstanceRB.velocity = lightningSpeed * lightningTransform.forward;
        StartCoroutine(_rScaleLightning(lightningDuration));
    }

    public void ShieldTriggered()
    {
        isTriggered = true;
    }
}
