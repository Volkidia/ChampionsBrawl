using UnityEngine;
using System.Collections;

public class OffensiveChargeP0 : _AbilityParentClass {

    //Initialisation parameters----------------------------
    public string abName = "ChargeOffensiveP0";
    public float maxCooldown = 10.0f;
    public int nbActivation = 1;
    public float CastingTime = 0.5f;
    [Space(5)]
    public float[] WaitingTimes;

    [Header("Ability Parameters", order = 1)]
    public float chargeInitSpeed = 5.0f;
    public float maxDuration = 1.5f;
    public Coordinator coor;

    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
        coor = coor ? coor : GetComponentInParent<Coordinator>();
    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        if(actualsActivations == 0)
            coor.cMove_Charge(JoystickInput.normalized, chargeInitSpeed, maxDuration);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
