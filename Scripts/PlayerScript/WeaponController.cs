using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WeaponController : NetworkBehaviour {
    public Weapon _weaponScript;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChargeWeapon(Weapon newWeapon)
    {
        _weaponScript = newWeapon;
        
    }

  
    public void WeaponAttack()
    {

        if(_weaponScript)
        {
            _weaponScript.Attack();
        }
    }

    public void WeaponAttack(float attackDuration)
    {
        if(_weaponScript)
        {
            _weaponScript.Attack(attackDuration);
        }
    }

    public void ForcedAttackEnd()
    {
            _weaponScript.endAttack();
    }
}
