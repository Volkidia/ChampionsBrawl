using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Weapon : HitInfos {


    private List<Collider> _collList= new List<Collider>();

    [SyncVar]
    private bool _isAttacking = false;

    private IEnumerator _refRAttack = null;

    public Transform weaponGrip;
    public Collider thisCollider;

    public bool isAttacking {
        get { return _isAttacking; }
    }

    [Command]
    void Cmd_SetEnable(bool _value)
    {
        _isAttacking = _value;
        Debug.Log("command");
    }

	// Use this for initialization
	void Start () {
        thisCollider = thisCollider ? thisCollider : GetComponent<Collider>();
        thisCollider.enabled = false;

	}
	
    public void ignoreOwnerCollider(Collider ownerCollider)
    {
        Physics.IgnoreCollision(thisCollider, ownerCollider, true);
    }

    public void Attack()
    {
        Debug.Log("Attack");
        _isAttacking = true;
    }

    public void Attack(float attackDuration)
    {
        _isAttacking = true;
        _refRAttack = _rAttack(attackDuration);
        StartCoroutine(_refRAttack);
    }

    public void endAttack()
    {
        if(_refRAttack != null)
        {
            StopCoroutine(_refRAttack);
        }
        _refRAttack = null;

        _isAttacking = false;
        Debug.Log("unset Weapon ");
        _collList = new List<Collider>();
    }

    void OnTriggerEnter(Collider coll)
    {
        if(_isAttacking && !_collList.Contains(coll))
        {
            _collList.Add(coll);
            coll.gameObject.SendMessage("Touched", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    IEnumerator _rAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        endAttack();
    }


	// Update is called once per frame
	void Update () {
        thisCollider.enabled = _isAttacking;
    }
}
