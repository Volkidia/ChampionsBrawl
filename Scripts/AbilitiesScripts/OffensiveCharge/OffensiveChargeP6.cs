using UnityEngine;
using System.Collections;

public class OffensiveChargeP6 : _AbilityParentClass {

    //Initialisation parameters----------------------------
    public string abName = "ChargeOffensiveP0";
    public float maxCooldown = 10.0f;
    public int nbActivation = 1;
    public float CastingTime = 0.5f;
    [Space(5)]
    public float[] WaitingTimes;

    [Header("Ability Parameters", order = 1)]
    public float chargeSpeed = 5.0f;
    public float chargeDuration = 1.0f;
    public LayerMask playerMask;
    public LayerMask levelMask;

    private bool _hasTouched = false;
    private IEnumerator _rDestroy = null;
    private CharacterController _charCtrl;
    private CapsuleCollider _capsCollider = null;
    private bool _isCharging = false;
    public float backDist = .5f;
    public Coordinator coor;


    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
        coor = coor ? coor : GetComponentInParent<Coordinator>();
        _charCtrl = GetComponentInParent<CharacterController>();

        CapsuleCollider _coll = gameObject.AddComponent<CapsuleCollider>();
        _coll.height = _charCtrl.height;
        _coll.radius = _charCtrl.radius;
        _coll.center = _charCtrl.center;
        _coll.isTrigger = true;
        _capsCollider = _coll;
    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        switch(actualsActivations)
        {
            case 0:
                _charge(JoystickInput.normalized);
                break;
            default:
                break;
        }
    }

    void _charge(Vector3 dir)
    {
        coor.cMove_Charge(dir.normalized, chargeSpeed, chargeDuration);
        _rDestroy = _rDestroyColl(chargeDuration);
        StartCoroutine(_rDestroy);
    }

    IEnumerator _rDestroyColl(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isCharging = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        if(_isCharging) {
            if(PhysicsBox.isInlayerMask(coll.gameObject, playerMask))
            {
                _isCharging = false;
                _hasTouched = true;
                if(_rDestroy != null)
                    StopCoroutine(_rDestroy);

                coor.CharMove.StopCharge();
                _goBehind(coll.gameObject.GetComponent<CharacterMove>());
            }

            if(PhysicsBox.isInlayerMask(coll.gameObject, levelMask))
            {
                if(_rDestroy != null)
                    StopCoroutine(_rDestroy);
                coor.CharMove.StopCharge();
            }
        }      
    }

    void _goBehind(CharacterMove ennemyCharMove)
    {
        coor.CharMove.dirChar = ennemyCharMove.DirChar;
        transform.parent.position = ennemyCharMove.transform.forward * ennemyCharMove.DirChar * backDist;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
