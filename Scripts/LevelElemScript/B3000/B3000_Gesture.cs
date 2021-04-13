using UnityEngine;
using System.Collections;

public class B3000_Gesture : MonoBehaviour {

    public Animator anim;

    [Header("Rotor param", order = 1)]
    public string aSpeed_RotorParam = "aSpeed_Rotor";
    [Range(.1f,5)]
    public float rotorRotSpeed = 1.0f;

    [Header("Walk Param", order = 2)]
    public string aSpeed_WalkParam = "aSpeed_Walk";
    [Range(.1f,5)]
    public float walkSpeedMod = 1.0f;

    [Header("Stand Param", order = 3)]
    public string aSpeed_StandParam = "aSpeed_Stand";
    [Range(.1f,5)]
    public float standSpeed = 1.0f;

    [Header("Move Parameters")]
    public Vector3 moveAxis = new Vector3(0,0,1);
    public float maxMoveSpeed = 8;
    public float minMoveSpeed = 1;
    public float moveSpeed = 3.0f;
    public float maxPauseDelay = 10.0f;
    public float maxPauseDuration = 5.0f;
    public float standDuration = 5.5f;
    public GameObject swapDirGo;
    public GameObject delayedPauseGo;
    public Vector3[] swapDirPos;
    public Vector3[] delayedPausePos;

    private IEnumerator _rDelayedPause;
    private float _lastTotSpeedV;
    private bool isWalking = true;


	// Use this for initialization
	void Start () {
        if(swapDirGo)
            foreach(Vector3 _pos in swapDirPos)
            {
                Instantiate(swapDirGo, transform.position + _pos, Quaternion.identity);
            }

        if(delayedPauseGo)
            foreach(Vector3 _pos in delayedPausePos)
            {
                Instantiate(delayedPauseGo, transform.position + _pos, Quaternion.identity);
            }
	}
	
	// Update is called once per frame
	void Update () {
        if((rotorRotSpeed + walkSpeedMod * moveSpeed + standSpeed) != _lastTotSpeedV)
            updateAnimSpeed();
        anim.SetBool("isWalking", isWalking);

        if(isWalking)
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void updateAnimSpeed()
    {
        anim.SetFloat(aSpeed_RotorParam, rotorRotSpeed);
        anim.SetFloat(aSpeed_StandParam, standSpeed);
        anim.SetFloat(aSpeed_WalkParam, walkSpeedMod * moveSpeed);
        _lastTotSpeedV = rotorRotSpeed + walkSpeedMod * moveSpeed + standSpeed;
    }

    void swapSide()
    {
        transform.forward *= -1;
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);        
    }

    void delayedPause()
    {
        if(_rDelayedPause != null)
            StopCoroutine(_rDelayedPause);
        _rDelayedPause = _delayedPause(Random.Range(0, maxPauseDelay));
        StartCoroutine(_rDelayedPause);
    }

    IEnumerator _delayedPause(float delay)
    {
        yield return new WaitForSeconds(delay);
        float _pauseDuration = Random.Range(0,maxPauseDuration);
        StartCoroutine(_Pause(_pauseDuration));
    }

    IEnumerator _Pause(float pauseDuration)
    {
        isWalking = false;
        yield return new WaitForSeconds(pauseDuration);
        isWalking = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, .5f);
        foreach(Vector3 _pos in swapDirPos)
        {
            Gizmos.DrawWireCube(transform.position + _pos, Vector3.one);
        }

        Gizmos.color = new Color(0, 1, 0, .5f);
        foreach(Vector3 _pos in delayedPausePos)
        {
            Gizmos.DrawWireCube(transform.position + _pos, Vector3.one);
        }
    }
}
