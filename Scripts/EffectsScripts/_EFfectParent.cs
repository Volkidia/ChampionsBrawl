using UnityEngine;
using System.Collections;

public class _EFfectParent : MonoBehaviour {
    
    public int idSource;
    public float effectDuration = .5f;
    public float[] effectValues = null;
    public GameObject[] effectObjects = null;
    public bool isUnique = false;
    public bool isRefresh = false;
    public EffectsController efCtrl;

    public Coordinator coor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InitEffect(_EFfectParent efDatas)
    {
        idSource = efDatas.idSource;
        effectDuration = efDatas.effectDuration;
        effectValues = efDatas.effectValues;
        effectObjects = efDatas.effectObjects;
        isUnique = efDatas.isUnique;
        isRefresh = efDatas.isRefresh;

        coor = efDatas.coor ? efDatas.coor : GetComponent<Coordinator>();
        efCtrl = efDatas.efCtrl ? efDatas.efCtrl : GetComponent<EffectsController>();
    }

    public void InitEffect(Coordinator pCoor, EffectsController effectCtrl)
    {
        coor = pCoor;
        efCtrl = effectCtrl;
    }

    public void InitEffect(float effectBaseDuration, 
        bool isEffectUnique = true, bool isEffectRefresh = false, float[] effectAdditionalValues = null, 
        GameObject[] effectLinkedObject = null, EffectsController efController = null, Coordinator charCoordinator = null)
    {
        effectDuration = effectBaseDuration;
        isUnique = isEffectUnique;
        isRefresh = isEffectRefresh;

        efCtrl = efController ? efController : GetComponent<EffectsController>();
        coor = charCoordinator ? charCoordinator : GetComponent<Coordinator>();
        effectValues = effectAdditionalValues;
        effectObjects = effectLinkedObject;
    }

    public void StartEffect()
    {
        coor = coor ? coor : GetComponent<Coordinator>();
        Effect();
    }
    
    public void EndEffect()
    {
        if(!efCtrl)
            efCtrl = GetComponent<EffectsController>();

        efCtrl.removeEffect(this.GetType());
    }

    public void effectDestroy()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }


    public virtual void Effect()
    {
        //Debug.Log("Test_EffectParent");
    }

    public virtual void RefreshEffect(_EFfectParent newData)
    {

    }

    public virtual void Touched(HitInfos hInfos)
    {

    }

    public static bool operator ==(_EFfectParent a, _EFfectParent b)
    {

        return (a.idSource == b.idSource
                && a.effectDuration == b.effectDuration
                && a.effectValues == b.effectValues 
                && a.effectObjects == b.effectObjects);
    }

    public static bool operator !=(_EFfectParent a, _EFfectParent b)
    {
        return !(a == b);
    }
}
