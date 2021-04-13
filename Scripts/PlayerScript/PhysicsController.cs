using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[System.Serializable]
public class HitInfos:NetworkBehaviour
{
    [SerializeField]
    public NetworkInstanceId idSource;
    public int id = 0;
    [SerializeField]
    public int damages = 0;
    [SerializeField]
    public GameObject[] Effects = null;
    
    public virtual void OnTriggerEnter(Collider coll)
    {
        if (hasAuthority)
            id = -1;
        else
            id = 0;

       
      coll.gameObject.SendMessage("Touched", this, SendMessageOptions.DontRequireReceiver);
        
        
    }
    
    public void init(NetworkInstanceId Source, int dmgs, GameObject[] allEffects)
    {
        //idSource = Source;
        damages = dmgs;
        Effects = allEffects;
        init();
    }

    public void init()
    {
        /*
        if(Effects.Length > 0)
            foreach(GameObject _ef in Effects)
                _ef.GetComponent<_EFfectParent>().idSource = idSource;
        */
    }

    virtual public void _didHit()
    {
        Debug.Log("Base _didHit of " + this.gameObject);
    }

    public void _outHit()
    {
        Debug.Log("Base _outHit of " + this.gameObject);
    }

    virtual public void DestroyThis()
    {

    }

    virtual public void testVirtual()
    {

    }
}
[SerializeField]

public class PhysicsController : NetworkBehaviour {

    public enum PhysicsEnum
    {
       BasePlayer = 5, 
       ThroughPlayer = 6,
       Projectiles = 7,
       FullCollisionPlatform = 8,
       ThroughPlatform = 9,
    }

    public Coordinator coor;
    public PhysicsEnum layerThrough = PhysicsEnum.ThroughPlayer;
    public PhysicsEnum baseLayer = PhysicsEnum.BasePlayer;
    public float platformCollideCheckSensibility = .2f;
    public LayerMask platformCollideMask;
    public float throughCheckRate = .2f;

    private IEnumerator _tempCheckIn =null;
    private CharacterController _cCollider;
    private bool isthrough = false;

    [SyncVar]
    int currLayer;

   [Command]
    void Cmd_SyncLayer(int newLayer)
    {
        currLayer = newLayer;
    }

	// Use this for initialization
	void Start () {
        coor = coor ? coor : GetComponent<Coordinator>();
        _cCollider = GetComponent<CharacterController>();
        currLayer = LayerMask.NameToLayer(baseLayer.ToString());
        revertToBaseLayer();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.A))
        {
            changeLayerToThrough();
        }
        if(Input.GetKeyDown(KeyCode.Z))
            revertToBaseLayer();

        if(Input.GetKeyDown(KeyCode.Space))
            Debug.Log(isInLevel());

        gameObject.layer = currLayer;
	}
        

    public void Touched(HitInfos hInfos)
    {
        //Debug.Log("Hit Damages Received" + hInfos.damages);
        Debug.Log(" le mec qui lance:" + hInfos.idSource + "    netIDPlayer: " + netId);
        
        if (isLocalPlayer && hInfos.id != -1)
        {             
            hInfos._didHit();
            coor.receiveHit(hInfos);
        }
        
    }

    public void ExitTouch(HitInfos hinfos)
    {
        //Debug.Log("Hit Out");
        hinfos._outHit();
    }

    public void startThrough(float throughDuration)
    {
        StartCoroutine(_rTimedThrough(throughDuration));
    }

    public void inCheckStart()
    {
        if(_tempCheckIn != null)
            StopCoroutine(_tempCheckIn);

        _tempCheckIn=_rCheckStillIn();
        StartCoroutine(_tempCheckIn);
    }

    public void changeLayerToThrough()
    {
        if(currLayer != LayerMask.NameToLayer(layerThrough.ToString()))
        {
            if(isLocalPlayer)
            {
                Cmd_SyncLayer(LayerMask.NameToLayer(layerThrough.ToString()));
            }
            isthrough = true;

            gameObject.layer = currLayer;
        }
    }

    public void revertToBaseLayer()
    {
        if(currLayer != LayerMask.NameToLayer(baseLayer.ToString()))
        {
            if(isLocalPlayer)
            {
                Cmd_SyncLayer(LayerMask.NameToLayer(baseLayer.ToString()));
            }
            isthrough = false;
            gameObject.layer = currLayer;
        }
    }

    /// <summary>
    /// Update through Layer function
    /// </summary>
    /// <param name="wantedThroughState">The asked through state</param>
    /// <param name="throughDuration">The min through duration wanted</param>
    /// <returns>If the state changed</returns>
    public bool updateLayer(bool wantedThroughState, float throughDuration = 0)
    {
        bool retV = false;
        if(isthrough != wantedThroughState)
        {
            isthrough = wantedThroughState;
            retV = true;
            
            if(wantedThroughState)
                startThrough(throughDuration);
            else
                inCheckStart();
        }
        return retV;
    }

    public bool isInLevel()
    {
        Vector3 _rOrigin = transform.position;
        _rOrigin.y += (_cCollider.height / 2) + platformCollideCheckSensibility;
        _rOrigin.z -= _cCollider.radius;
        Ray _r = new Ray(_rOrigin,Vector3.down);
        _rOrigin.z += 2 * _cCollider.radius;
        Ray _r2 = new Ray(_rOrigin,Vector3.down);

        return (Physics.Raycast(_r, _cCollider.height + (2 * platformCollideCheckSensibility),platformCollideMask) || Physics.Raycast(_r2, _cCollider.height + (2 * platformCollideCheckSensibility), platformCollideMask));
    }

    IEnumerator _rCheckStillIn()
    {
        do
        {
            yield return new WaitForEndOfFrame();
        } while(isInLevel());

        revertToBaseLayer();
        yield return null;
    }

    IEnumerator _rTimedThrough(float baseDuration)
    {
        changeLayerToThrough();
        yield return new WaitForSeconds(baseDuration);
        StartCoroutine(_rCheckStillIn());
    }

    IEnumerator _rRevertThrough(float baseDuration)
    {
        yield return null;
    }

    /*
    void OnDrawGizmos()
    {
        Vector3 _rOrigin = transform.position;
        _rOrigin.y += _cCollider.height / 2 + platformCollideCheckSensibility;

        Vector3 _Direction = Vector3.down * (_cCollider.height + (2 * platformCollideCheckSensibility));
        Gizmos.DrawWireSphere(_rOrigin, .2f);
        Gizmos.DrawWireSphere(_rOrigin + _Direction, .2f);

        _rOrigin.z -= _cCollider.radius;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_rOrigin,Vector3.down * (_cCollider.height + (2 * platformCollideCheckSensibility)));

        _rOrigin.z += 2 * _cCollider.radius;
        Gizmos.DrawRay(_rOrigin, Vector3.down * (_cCollider.height + (2 * platformCollideCheckSensibility)));
    }
    */
}
