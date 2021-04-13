using UnityEngine;
using System.Collections;

public class AnimControllerOld : MonoBehaviour {
    [System.Serializable]
    public class AnimationClipInfos
    {
        public string clipStateName;
        public AnimationClip clipToPlay;
        public float animSpeed = 1;
        public animBool aState;
        public string boolParamName;
        public string speedParamName;
    }

    [SerializeField]

    public AnimationClipInfos[] aInfos;

    public Animator charAnimator;
    public enum animBool
    #region anim state bit enumarator
    {
        _run =              (1 << 0),
        _jump =             (1 << 1),
        _fall =             (1 << 2),
        _attack =           (1 << 3),
        _dodge =            (1 << 4),
        _charge =           (1 << 5),
        _spell_launch =     (1 << 6),
        _spell_return =     (1 << 7),
        _stand =                0,
    }
    #endregion
    
    public animBool aBool = 0;

    string[] animator_Parameters =
    #region animator parameters List
    {
        "a_run",
        "a_jump",
        "a_fall",
        "a_attack",
        "a_dodge",
        "a_charge",
        "a_spell_launch",
        "a_spell_return",
    };
    #endregion

    // ###########################################  MonoBehavior ##############################################################

    // Use this for initialization
    void Start () {
        charAnimator = charAnimator ? charAnimator : GetComponentInChildren<Animator>();
        anim_initiate();

    }
	
	// Update is called once per frame
	void Update () {

        //------- run anim test --------------------------
        if (Input.GetKeyDown(KeyCode.RightArrow))
            anim_play(animBool._run, true);
        else if (Input.GetKeyUp(KeyCode.RightArrow))
            anim_play(animBool._run, false);

        //------- attack anim test ------------------------
        if (Input.GetKeyDown(KeyCode.Space))
            anim_play(animBool._attack, .583f);

        //------- jump anim test ------------------------
        if (Input.GetKeyDown(KeyCode.UpArrow))
            anim_play(animBool._jump, 3.0f);

        //------- jump anim test ------------------------
        if (Input.GetKeyDown(KeyCode.DownArrow))
            anim_play(animBool._fall, true);
        if (Input.GetKeyUp(KeyCode.DownArrow))
            anim_play(animBool._fall, false);

        //------- spell anim test ------------------------
        if (Input.GetKeyDown(KeyCode.S))
            anim_play(animBool._spell_launch, 0.9f);

        //------- Dodge anim test ------------------------
        if (Input.GetKeyDown(KeyCode.D))
            anim_play(animBool._dodge, .792f);
    }


    // ###########################################  FUNCTION ##############################################################
    #region Functions

    // ============== PRIVATE =====================
    #region PRIVATE FUNCTIONS    

    void anim_initiate()
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = charAnimator.runtimeAnimatorController;

        foreach (AnimationClipInfos aC_infos in aInfos)
        {
            overrideController[aC_infos.clipStateName] = aC_infos.clipToPlay;
        }

        overrideController.name = "new anim Controller";
        charAnimator.runtimeAnimatorController = overrideController;
    }

    /// <summary>
    /// Simple anim state bit toggle 1 & 0
    /// </summary>
    /// <param name="anim">animBool state bit to toggle</param>
    void anim_toggleState(animBool anim)
    {
        if (anim_getState(anim))
            anim_UnsetState(anim);
        else
            anim_SetState(anim);
    }

    /// <summary>
    /// Simple anim state bit toggle 1 & 0, after testing if the wanted result changes from the current anim state machine
    /// </summary>
    /// <param name="anim">animBool state bit to toggle</param>
    /// <param name="wantedState">wanted final state</param>
    void anim_toggleState(animBool anim, bool wantedState) // OVERLOAD 1
    {
        if(anim_getState(anim) != wantedState)
        {
            if (wantedState)
                anim_SetState(anim);
            else
                anim_UnsetState(anim);
        }
    }

    /// <summary>
    /// Test if the specified anim state bit is already set to 1
    /// if not : Start the coroutine "_r_anim_frame" with the specified anim as parameter
    /// </summary>
    /// <param name="anim">animBool to use as parameter of the coroutine</param>
    void StartCoroutine(animBool anim)
    {
        if (!anim_getState(anim))
            StartCoroutine(_r_anim_frame(anim));
    }

    /// <summary>
    /// Test if the specified anim state bit is already set to 1
    /// if not : Start the coroutine "_r_anim_delay" with the specified anim and delay as parameters
    /// </summary>
    /// <param name="anim">animBool to use as parameter of the coroutine</param>
    /// <param name="delayDuration">delay to use as parameter of the coroutine</param>
    void StartCoroutine(animBool anim, float delayDuration)
    {
        if (!anim_getState(anim))
            StartCoroutine(_r_anim_delay(anim, delayDuration));
    }
    #endregion

    // ============== PUBLIC =====================
    #region PUBLIC FUNCTIONS

    /// <summary>
    /// Set specified anim state to 1 in the global anim state machine (aBool)
    /// </summary>
    /// <param name="state">animBool state to set at 1</param>
    public void anim_SetState(animBool anim)
    {
        aBool |= anim;
        animVar_Update();
    }


    /// <summary>
    /// Set specified anim state to 0 in the global anim state machine (aBool)
    /// </summary>
    /// <param name="state">animBool state to set at 0</param>
    public void anim_UnsetState(animBool anim)
    {
        aBool &= ~anim;
        animVar_Update();
    }

    /// <summary>
    /// Update the Animator (charAnimator) Parameters based on the current anim state machine (aBool)
    /// </summary>
    public void animVar_Update()
    {
        for (int i = 0; i <= animator_Parameters.Length - 1; i++)
        {
            string anim_param = animator_Parameters[i];
            bool stateTest = (aBool & (animBool)(1 << i)) != 0;
            bool boolV = (charAnimator.GetBool(anim_param) != stateTest);

            if (boolV)
                charAnimator.SetBool(anim_param, stateTest);

            //Debug.Log("animVarUpdate N° : " + i + " - animation Parameter : " + anim_param + " - isDifferent : " + boolV + " - Value : " + stateTest + " - animBool i Value : " + (animBool)(1 << i));
        }
    }

    /// <summary>
    /// Get the current value of the specified state bit
    /// </summary>
    /// <param name="state">tested animBool state</param>
    /// <returns>boolean state of the bit (1 = true, 0 = false)</returns>
    public bool anim_getState(animBool anim)
    {
        return (aBool & anim) != 0;
    }

    /// <summary>
    /// toggle animator parameter of the specified anim to the wanted state if different from the current
    /// </summary>
    /// <param name="anim">specified animBool state bit to toggle</param>
    /// <param name="state">wanted state for the anim state bit</param>
    public void anim_play(animBool anim, bool state)
    {
        anim_toggleState(anim, state);
    }

    /// <summary>
    /// Set the animator parameter of the specified anim to 1, and reset it during the next FRAME
    /// </summary>
    /// <param name="anim">specified animBool state bit to toggle</param>
    public void anim_play(animBool anim)
    {

        StartCoroutine(anim);
    }

    // #######################################################################################################################################

    /// <summary>
    /// Set the animator parameter of the specified animState to 1, and reset it when it ends
    /// </summary>
    /// <param name="anim">the animBool state bit to toggle</param>
    /// <returns>the duration of the animation</returns>
    public float anim_autoPlay(AnimationClipInfos anim)
    {
        switch (anim.aState)
        {
            case animBool._run:
                break;
            case animBool._jump:
                break;
            case animBool._fall:
                break;
            case animBool._attack:
                break;
            case animBool._dodge:
                break;
            case animBool._charge:
                break;
            case animBool._spell_launch:
                break;
            case animBool._spell_return:
                break;
            case animBool._stand:
                break;
            default:
                break;
        }

        return anim.clipToPlay.length / anim.animSpeed;
    }

    // #######################################################################################################################################


    /// <summary>
    /// Set the animator parameter of the specified anim to 1, and reset it after the specified DELAY
    /// </summary>
    /// <param name="anim">specified animBool state bit to toggle</param>
    /// <param name="delayDuration">delay duration before reseting the anim state bit to 0</param>
    public void anim_play(animBool anim, float delayDuration)
    {
        StartCoroutine(anim, delayDuration);
    }

    #endregion
    #endregion

    #region testFunction (Class AnimationClipInfos)
    private void _testSartCoroutine(AnimationClipInfos infos)
    {
       /* if (!anim_getState(infos.aState))
            StartCoroutine(_rTest_anim_frame(infos)); */

    }

    private void setParameters(AnimationClipInfos infos)
    {
        charAnimator.SetFloat(infos.speedParamName, infos.animSpeed);
    }

    #endregion



    // ###########################################  ROUTINE ##############################################################
    #region Anim routines

    /// <summary>
    /// Set to 1 the specified anim state bit and wait for the specified delay to revert it
    /// </summary>
    /// <param name="anim">anim state bit to "toggle"</param>
    /// <param name="delayDuration">delay before reverting the toggle</param>
    /// <returns></returns>
    IEnumerator _r_anim_delay(animBool anim, float delayDuration)
    {
        Debug.Log(anim.ToString() + " delay Routine - Delay : " + delayDuration);
        anim_SetState(anim);
        yield return new WaitForSeconds(delayDuration);
        anim_UnsetState(anim);
        yield return null;
    }

    /// <summary>
    /// Set to 1 the specified anim state bit and revert it during the next frame
    /// </summary>
    /// <param name="anim">anim state bit to "toggle"</param>
    /// <returns></returns>
    IEnumerator _r_anim_frame(animBool anim)
    {
        Debug.Log(anim.ToString() + " next Frame Routine");
        anim_SetState(anim);
        yield return new WaitForEndOfFrame();
        anim_UnsetState(anim);
        yield return null;
    }

    /*
    IEnumerator _rTest_anim_frame(AnimationClipInfos animationInfos)
    {
        charAnimator.SetFloat(animationInfos.speedParamName, animationInfos.animSpeed);

        yield return null;
    }
    */

    /*
    /// <summary>
    /// Set to 1 the specified first_anim state, and after a delay reset it and set the next_anim state to 1 and reset it after a
    /// </summary>
    /// <param name="first_anim"></param>
    /// <param name="next_anim"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator _r_anim_chained(animBool first_anim, animBool next_anim, float delay)
    {
        yield return null;
    }
    */
    #endregion
}
