using UnityEngine;
using System.Collections;

public class SlowArea : _EFfectParent {

    float appliedSlow = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void RemoveSlowAreaEffect(_EFfectParent ef)
    {
        if(ef.GetType() == this.GetType())
        {
            if(ef.idSource == idSource)
            {
                coor.efRevertSlow(appliedSlow);
                EndEffect();
            }
        }
    }

    public override void Effect()
    {
        if(effectValues.Length >= 1)
            appliedSlow = coor.efApplySlow(effectValues[0]);
    }
}
