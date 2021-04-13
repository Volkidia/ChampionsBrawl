using UnityEngine;
using System.Collections;

public class LightMark : _EFfectParent {
    private bool isStart = false;
    private float _endEffectTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Effect()
    {
        isStart = true;
    }

    IEnumerator _rDestroyTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndEffect();
    }

    /*public override void Touched(HitInfos hInfos)
    {
        if(isStart && hInfos.idSource == idSource)
        {
            coor.receiveHit((int)effectValues[0], idSource);
            EndEffect();
        }
    }*/

    public override void RefreshEffect(_EFfectParent newData)
    {
        _endEffectTime = Time.time + newData.effectDuration;
        effectValues = newData.effectValues;
        effectDuration = newData.effectDuration;
    }
}
