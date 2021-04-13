using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AnimController : NetworkBehaviour {
    [System.Serializable]
    public class AnimationClipInfos
    {
        public enum animBool
        {
            _run =              (1 << 0),
            _jump =             (1 << 1),
            _fall =             (1 << 2),
            _attack =           (1 << 3),
            _dodge =            (1 << 4),
            _charge =           (1 << 5),
            _spell =            (1 << 6),
            _land =             (1 << 7),
            _elecStun =         (1 << 8),
            _standOrNull =          0,
        }
        public string clipStateName;
        public AnimationClip clipToPlay;
        public float animSpeed = 1;
        public float delay_action = .2f;
        public animBool aState;
        public string boolParamName;
        public string speedParamName;
    }
    [SerializeField]

    private string[] _aBoolParams = {"a_run","a_jump","a_fall","a_attack","a_dodge","a_charge","a_spell","a_Land"};

    //Clips Informattion
    public AnimationClipInfos[] aInfos;    

    ///Animator of the character
    public Animator charAnimator;

    //Public accessor to the aBool (current animation state), !!! Read Only Permission !!!
    public AnimationClipInfos.animBool AnimationStateBool
    {
        get { return aBool; }
    }

    //Current animation state
    [SyncVar]
    private AnimationClipInfos.animBool aBool = 0;

    [Command]
    void Cmd_update_States(AnimationClipInfos.animBool newState)
    {
        aBool = newState;
    }

    private AnimationClipInfos.animBool _lastABool = 0;

    private IEnumerator _rTimedAnimPause;
    

    // ###########################################  MonoBehavior ##############################################################

    // Use this for initialization
    void Start () {
        charAnimator = charAnimator ? charAnimator : GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        /*
        if(Input.GetKeyDown(KeyCode.O))
            toggleAnimPlay();

        if (Input.GetKeyDown(KeyCode.A))
            anim_autoPlay(AnimationClipInfos.animBool._attack);

        if (Input.GetKeyDown(KeyCode.Z))
            anim_autoPlay(AnimationClipInfos.animBool._spell, 3);

        bool runTest = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

        if (runTest != anim_getState(AnimationClipInfos.animBool._run))
            anim_toggleState(aInfos[2]);

        if (Input.GetKey(KeyCode.UpArrow) != anim_getState(AnimationClipInfos.animBool._jump))
            anim_autoPlay(AnimationClipInfos.animBool._jump);

        if (Input.GetKey(KeyCode.DownArrow) != anim_getState(AnimationClipInfos.animBool._fall))
            anim_autoPlay(AnimationClipInfos.animBool._fall);
            */
        
        if(!isLocalPlayer && _lastABool != aBool)
        {
            syncAnimValue();
            _lastABool = aBool;
        }
    }

    public void syncAnimValue (){
        AnimationClipInfos.animBool _tmpABool = getDiffPos(_lastABool);
        for(int i = 0; i < _aBoolParams.Length; i++)
        {
            if(compareState(_tmpABool, (AnimationClipInfos.animBool)(1 << i)))
                charAnimator.SetBool(_aBoolParams[i], anim_getState((AnimationClipInfos.animBool)(1 << i)));
        }
    }

    public void importAnimInfos(AnimationClipInfos[] animationsInfos)
    {
        aInfos = animationsInfos;
        anim_initiate();
    }

    public void toggleAnimPlay()
    {
        charAnimator.enabled = !charAnimator.enabled;
    }

    public void Pause()
    {
        charAnimator.enabled = false;
    }

    public void Pause(float duration)
    {
        if(_rTimedAnimPause != null)
        {
            StopCoroutine(_rTimedAnimPause);
        }

        _rTimedAnimPause = _TimedPause(duration);
        StartCoroutine(_rTimedAnimPause);
    }

    public void Unpause()
    {
        charAnimator.enabled = true;
    }

    IEnumerator _TimedPause(float duration)
    {
        Pause();
        yield return new WaitForSeconds(duration);
        Unpause();
    }

    // ###########################################  FUNCTION ##############################################################
    #region Functions

    // ============== PRIVATE =====================
    #region PRIVATE FUNCTIONS    
    
    /// <summary>
    /// Replace the empty anim state of the animator with the clips set in the Inspector
    /// CALLED ONCE, in the START
    /// </summary>
    public void anim_initiate()
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = charAnimator.runtimeAnimatorController;

        foreach (AnimationClipInfos aC_infos in aInfos)
        {
            overrideController[aC_infos.clipStateName] = aC_infos.clipToPlay;
            if(aC_infos.speedParamName != "")
                charAnimator.SetFloat(aC_infos.speedParamName, aC_infos.animSpeed);
        }

        overrideController.name = gameObject.name + "override Controller";
        charAnimator.runtimeAnimatorController = overrideController;
    }

    //############# anim_toggleState() OVERLOADS
    #region anim_toggleState OVERLOADS
    /// <summary>
    /// Simple anim state bit toggle 1 & 0
    /// </summary>
    /// <param name="anim">AnimationClipInfos.animBool state bit to toggle</param>
    void anim_toggleState(AnimationClipInfos anim)
    {
        if (anim_getState(anim.aState))
            anim_UnsetState(anim);
        else
            anim_SetState(anim);
    }

    /// <summary>
    /// Simple anim state bit toggle 1 & 0, after testing if the wanted result changes from the current anim state machine
    /// </summary>
    /// <param name="anim">AnimationClipInfos.animBool state bit to toggle</param>
    /// <param name="wantedState">wanted final state</param>
    void anim_toggleState(AnimationClipInfos anim, bool wantedState) // OVERLOAD 1
    {
        if(anim_getState(anim.aState) != wantedState)
        {
            if (wantedState)
                anim_SetState(anim);
            else
                anim_UnsetState(anim);
        }
    }
    #endregion

    //############# StartCoroutine() OVERLOADS
    #region StartCoroutine OVERLOADS
    /// <summary>
    /// Test if the specified anim state bit is already set to 1
    /// if not : Start the coroutine "_r_anim_frame" with the specified anim as parameter
    /// </summary>
    /// <param name="anim">AnimationClipInfos.animBool to use as parameter of the coroutine</param>
    void StartCoroutine(AnimationClipInfos anim, bool isFrame = false)
    {

        if (!anim_getState(anim.aState))
        {
            IEnumerator r = isFrame ? _r_anim_frame(anim) : _r_anim_nativeDuration(anim);
            StartCoroutine(r);
        }
    }

    /// <summary>
    /// Test if the specified anim state bit is already set to 1
    /// if not : Start the coroutine "_r_anim_delay" with the specified anim and delay as parameters
    /// </summary>
    /// <param name="anim">AnimationClipInfos.animBool to use as parameter of the coroutine</param>
    /// <param name="delayDuration">delay to use as parameter of the coroutine</param>
    void StartCoroutine(AnimationClipInfos anim, float forcedDuration)
    {
        if (!anim_getState(anim.aState))
            StartCoroutine(_r_anim_forcedDuration(anim, forcedDuration));
    }
    #endregion
    #endregion

    // ============== PUBLIC =====================
    #region PUBLIC FUNCTIONS


    //############# State Gesture function (get, set, unset)
    #region State Gesture function (get, set, unset)
    /// <summary>
    /// Set specified anim state to 1 in the global anim state machine (aBool)
    /// </summary>
    /// <param name="state">AnimationClipInfos.animBool state to set at 1</param>
    public void anim_SetState(AnimationClipInfos anim)
    {
        aBool |= anim.aState;
        //Debug.Log(anim.aState + " " + anim_getState(anim.aState));
        if(anim.speedParamName != "" && anim.animSpeed > 0 && charAnimator)
            charAnimator.SetFloat(anim.speedParamName, anim.animSpeed);

        if (charAnimator)
            charAnimator.SetBool(anim.boolParamName, true);

        Cmd_update_States(aBool);
    }


    /// <summary>
    /// Set specified anim state to 0 in the global anim state machine (aBool)
    /// </summary>
    /// <param name="state">AnimationClipInfos.animBool state to set at 0</param>
    public void anim_UnsetState(AnimationClipInfos anim)
    {
        aBool &= ~anim.aState;
        //Debug.Log(anim.aState + " " + anim_getState(anim.aState));
        charAnimator.SetBool(anim.boolParamName, false);

        Cmd_update_States(aBool);
    }

    /// <summary>
    /// Get the current value of the specified state bit
    /// </summary>
    /// <param name="state">tested AnimationClipInfos.animBool state</param>
    /// <returns>boolean state of the bit (1 = true, 0 = false)</returns>
    public bool anim_getState(AnimationClipInfos.animBool anim)
    {
        return (aBool & anim) != 0;
    }

    public AnimationClipInfos.animBool getDiffPos(AnimationClipInfos.animBool comparedState)
    {
        return aBool ^ comparedState;
    }

    public bool compareState(AnimationClipInfos.animBool baseState, AnimationClipInfos.animBool testState)
    {
        return (baseState & testState) != 0;
    }
    #endregion

    //############# Anim_Play() Overloads

    #region Anim_Play OVERLOADS
    /// <summary>
    /// toggle animator parameter of the specified anim to the wanted state if different from the current
    /// </summary>
    /// <param name="anim">specified animBool state bit to toggle</param>
    /// <param name="state">wanted state for the anim state bit</param>
    public void anim_play(AnimationClipInfos anim, bool state)
    {
        //Debug.Log(anim.clipStateName + "current state (animCtrl) : " + anim_getState(anim.aState) + "  | Wanted State : " + state );
        anim_toggleState(anim, state);
    }

    /// <summary>
    /// Set the animator parameter of the specified anim to 1, and reset it during the next FRAME
    /// </summary>
    /// <param name="anim">specified AnimationClipInfos.animBool state bit to toggle</param>
    public void anim_play(AnimationClipInfos anim)
    {
        StartCoroutine(anim);
    }

    /// <summary>
    /// Set the animator parameter of the specified anim to 1, and reset it after the specified DELAY
    /// </summary>
    /// <param name="anim">specified AnimationClipInfos.animBool state bit to toggle</param>
    /// <param name="delayDuration">delay duration before reseting the anim state bit to 0</param>
    public void anim_play(AnimationClipInfos anim, float delayDuration)
    {
        StartCoroutine(anim, delayDuration);
    }
    #endregion

    //!!!!!!!!! anim_autoPlay() INCOMPLETE !!!!!!

    #region !!!!!!!!! anim_autoPlay() INCOMPLETE !!!!!!
    public float anim_autoPlay(AnimationClipInfos.animBool animBool, float forcedDuration = 0)
    {
        AnimationClipInfos anim = aInfos[0];

        foreach (AnimationClipInfos ai in aInfos)
        {
            if (animBool == ai.aState)
                anim = ai;
        }

        IEnumerator rt = forcedDuration > 0 ? _r_anim_forcedDuration(anim, forcedDuration) : _r_anim_nativeDuration(anim);

        Debug.Log(anim.clipToPlay.name);

        switch (anim.aState)
        {
            case AnimationClipInfos.animBool._run:
            case AnimationClipInfos.animBool._jump:
            case AnimationClipInfos.animBool._fall:
                anim_toggleState(anim);
                break;
            case AnimationClipInfos.animBool._charge:
            case AnimationClipInfos.animBool._attack:
            case AnimationClipInfos.animBool._dodge:
            case AnimationClipInfos.animBool._spell:
                StartCoroutine(rt);
                break;
            default:
                break;
        }

        return forcedDuration > 0 ? forcedDuration : anim.clipToPlay.length / anim.animSpeed;
    }
    #endregion

    #endregion
    #endregion



    // ###########################################  ROUTINE ##############################################################
    #region Anim routines

    /// <summary>
    /// Set to 1 the specified anim state bit and wait for the specified delay to revert it
    /// </summary>
    /// <param name="anim">anim state bit to "toggle"</param>
    /// <param name="delayDuration">delay before reverting the toggle</param>
    /// <returns></returns>
    IEnumerator _r_anim_forcedDuration(AnimationClipInfos anim, float newDuration)
    {
        //Debug.Log(anim.clipToPlay.name + " forced Duration Routine - Delay : " + newDuration);
        anim_SetState(anim);
        yield return new WaitForSeconds(newDuration);
        anim_UnsetState(anim);
        yield return null;
    }

    /// <summary>
    /// Set to 1 the specified anim state bit and wait for the specified delay to revert it
    /// </summary>
    /// <param name="anim">anim state bit to "toggle"</param>
    /// <param name="delayDuration">delay before reverting the toggle</param>
    /// <returns></returns>
    IEnumerator _r_anim_nativeDuration(AnimationClipInfos anim)
    {
        Debug.Log(anim.clipToPlay.name + " native Duration Routine - Delay : " + anim.clipToPlay.length / anim.animSpeed);
        anim_SetState(anim);
        yield return new WaitForSeconds(anim.clipToPlay.length / anim.animSpeed);
        anim_UnsetState(anim);
        yield return null;
    }

    /// <summary>
    /// Set to 1 the specified anim state bit and revert it during the next frame
    /// </summary>
    /// <param name="anim">anim state bit to "toggle"</param>
    /// <returns></returns>
    IEnumerator _r_anim_frame(AnimationClipInfos anim)
    {
        Debug.Log(anim.clipToPlay.name + " next Frame Routine");
        anim_SetState(anim);
        yield return new WaitForEndOfFrame();
        anim_UnsetState(anim);
        yield return null;
    }

    #endregion
}
