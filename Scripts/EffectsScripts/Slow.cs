using UnityEngine;
using System.Collections;

public class Slow : _EFfectParent {
    float endEffectTime = -1;
    float _effectiveSlow = 0;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public override void RefreshEffect(_EFfectParent newData)
    {
        base.RefreshEffect(newData);
        endEffectTime = Time.time + newData.effectDuration;
        coor.efRevertSlow(_effectiveSlow);
        _effectiveSlow = coor.efApplySlow(newData.effectValues[0]);
        effectDuration = newData.effectDuration;
        effectValues = newData.effectValues;
    }

    public override void Effect()
    {
        if(effectValues.Length >= 1)
            StartCoroutine(_slowForduration(effectDuration, effectValues[0]));
        else
            Debug.Log("Value(s) Missing");
    }

    IEnumerator _slowForduration(float duration, float initSlowValue)
    {
        _effectiveSlow = coor.efApplySlow(initSlowValue);
        endEffectTime = Time.time + duration;
        do
            yield return new WaitForEndOfFrame();
        while(endEffectTime > Time.time);
        coor.efRevertSlow(_effectiveSlow);
        EndEffect();
    }
}
