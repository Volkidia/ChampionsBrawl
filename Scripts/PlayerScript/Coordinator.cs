using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Coordinator : MonoBehaviour
{
    [Header("Debug Behavior parameters", order =1)]
    public bool doDebug = false;
    public float debugLogTime = .25f;
    private float nextDebugLogTime = -1.0f;

    [Header("Script references (self init if null)", order = 2)]
    public CharacterMove CharMove;
    public InputsController InptsCtrl;
    public AnimController animCtrl;
    public AbilityController abilityCtrl;
    public PhysicsController physicsCtrl;
    public HealthController healthCtrl;
    public EffectsController effectCtrl;
    public WeaponController weaponCtrl;

    [Header("Player infos", order = 3)]
    public int idPlayer = -1;
    public float baseAttackCD = 2.0f;
    public float ultimateAnimDuration = 3.0f;

    [Header("Actions Masks (impossible override actions)", order = 4)]
    //Mask of states blocking the wanted actions
    #region Actions Masks
    [BitMask(typeof(State))]
    public State hMoveMask = 0;
    [BitMask(typeof(State))]
    public State jumpMask = 0;
    [BitMask(typeof(State))]
    public State fastFallMask = 0;
    [BitMask(typeof(State))]
    public State throughPlatformMask = 0;
    [BitMask(typeof(State))]
    public State dodgeMask = 0;
    [BitMask(typeof(State))]
    public State baseAttackMask = 0;
    [BitMask(typeof(State))]
    public State abilityMask;
    [BitMask(typeof(State))]
    public State ultimateMask;
    [BitMask(typeof(State))]
    public State bumpMask;
    #endregion

    [Header("Current Character State (coordinator only)", order = 5)]
    //Current state of the character
    [BitMask(typeof(State))]
    public State _currentState;

    private State _lastFrameState;
    //Public Accessor of the _currentState var, Read Only Permission
    public State CurrentState {
        get { return _currentState; }
    }

    public enum State
    {
        nullState = 0,
        hMove = (1 << 0),             
        Jump = (1 << 1),              
        FastFall = (1 << 2),          
        Dodge = (1 << 3),             
        Fall = (1 << 4),             
        BaseAttack = (1 << 5),       
        AbilityLB = (1 << 6),          
        Ultimate = (1 << 7),         
        Bump = (1 << 8),            
        Stun = (1 << 9),           
        Slow = (1 << 10),          
        ThroughPlatform = (1 << 11),
        BaseAttack_onCD = (1 << 12),
        AbilityRB = (1 << 13),
        AbilityRT = (1 << 14),
        Charge = (1 << 15),
        Land = (1 << 16),
        CurrLayerThrough = (1 << 17),
        CharMoveThrough = (1 << 18),
    };

    [BitMask(typeof(State))]
    public State throughStateMask;


    

    // Use this for initialization
    void Start()
    {
        abilityCtrl = abilityCtrl ? abilityCtrl : GetComponent<AbilityController>();
        CharMove = CharMove ? CharMove : GetComponent<CharacterMove>();
        InptsCtrl = InptsCtrl ? InptsCtrl : GetComponent<InputsController>();
        animCtrl = animCtrl ? animCtrl : GetComponent<AnimController>();
        physicsCtrl = physicsCtrl ? physicsCtrl : GetComponent<PhysicsController>();
        healthCtrl = healthCtrl ? healthCtrl : GetComponent<HealthController>();
        effectCtrl = effectCtrl ? effectCtrl : GetComponent<EffectsController>();
        weaponCtrl = weaponCtrl ? weaponCtrl : GetComponent<WeaponController>();

        //jumpMask = State.Dodge | State.BaseAttack;
        _lastFrameState = CurrentState;

        
        //Debug.Log(jumpMask);
    }


    // Update is called once per frame
    void Update()
    {
        if (doDebug && Time.time >= nextDebugLogTime)
        {
            //Debug.Log(_currentState);
            nextDebugLogTime = Time.time + debugLogTime;
        }

        _lastFrameState = CurrentState;
    }

    /// <summary>
    /// Send input values (0..1) to CharacterMove Script
    /// If the current State of Character allow it
    /// </summary>
    /// <param name="InputValue">value of horizontal movement Input (0 to 1 as float)</param>
    public void Move(float InputValue)
    {
        if (!testState(hMoveMask) && InputValue != 0 && CharMove.CanRun())
        {
            if (!testState(State.hMove))
            {
                animCtrl.anim_play(animCtrl.aInfos[1], true);
                s_SetHMove();
            }
        }
        else if(testState(State.hMove))
        {
            animCtrl.anim_play(animCtrl.aInfos[1], false);
            s_UnsetHMove();
        }

        CharMove.Move(InputValue);
    }

    /// <summary>
    /// Send jump instruction to CharacterMove Script
    /// If the Current State of Character allow it
    /// </summary>
    public void Jump()
    {
        if ((jumpMask & _currentState) == 0 && CharMove.CanJump())
        {
            s_SetJump();
            StartCoroutine(_r_Jump());
        }
    }

    public void Hit()
    {
        if (!testState(baseAttackMask))
            StartCoroutine(_r_Hit());
    }

    public void SpellLaunch(int spellId)
    {
        Debug.Log("SpellLaunch");
        if(!testState(abilityMask))
        {
            IEnumerator abilityRoutine = null;
            switch (spellId)
            {
                case 1:
                    abilityRoutine = _r_Spell(State.AbilityLB);
                    break;
                case 2:
                    abilityRoutine = _r_Spell(State.AbilityRB);
                    break;
                case 3:
                    abilityRoutine = _r_Spell(State.AbilityRT);
                    break;
                default:
                    break;
            }
            StartCoroutine(abilityRoutine);
        }
    }
    
    public void UltimateLaunch()
    {
        Debug.Log("Ultimate Launch");
        if (!testState(ultimateMask))
            StartCoroutine(_r_Ultimate());
    }

    /// <summary>
    /// Send FastFall instructions to do so or not
    /// If the Current State of Character allow it
    /// </summary>
    /// <param name="doFastFall">If do FastFall (true), or don't (false)</param>
    public void FastFalling(bool doFastFall)
    {
            if(doFastFall && CharMove.FastFalling())
            {
                if(InptsCtrl.isLocalPlayer)
                    s_SetFastFall();
                physicsCtrl.changeLayerToThrough();
            } else if((_lastFrameState & State.FastFall )!=0)
            {
                if(InptsCtrl.isLocalPlayer)
                    CharMove.StopFastFalling();
                _UpdateLayerThrough();
                s_UnsetFastFall();
            }
    }

    /// <summary>
    /// Send Dodge instruction to CharacterMove Script
    /// </summary>
    /// <param name="dirAxis">Direction of the dodge (-1 to 1)</param>
    public void dodge(float xAxis, float yAxis)
    {
        s_SetDodge();
        animCtrl.anim_play(animCtrl.aInfos[5], CharMove.Dodge(new Vector3(0, -yAxis, xAxis)));
    }

    public void charge(float xAxis, float yAxis)
    {
        StartCoroutine(_r_Charge(new Vector3(0, -yAxis, xAxis)));
    }

    /// <summary>
    /// Send Through instruction to CharacterMove Script
    /// </summary>
    /// <param name="through">No Used at the moment</param>
    public void GoThroughPlatform(bool through)
    {
        physicsCtrl.startThrough(CharMove.throughDuration);
    }

    /// <summary>
    /// Return the maxcooldown of all Abilities in the current kit (from ability Controller)
    /// </summary>
    /// <returns>float array : CDs of abilities in order</returns>
    public float[] getAbilitiesCD()
    {
        return abilityCtrl.abiliesCD();
    }

    /// <summary>
    /// Dispatch the hitInfos datas to the the differents script using it
    /// </summary>
    /// <param name="hInfos">Information from the Received hit</param>
    public void receiveHit(HitInfos hInfos)
    {
        if(healthCtrl)
            healthCtrl.TakeDamages(hInfos.idSource, hInfos.damages);

        if(effectCtrl)
            effectCtrl.addEffects(hInfos.Effects);
    }

    /// <summary>
    /// Dispatch the hit Informations to the differents scripts
    /// </summary>
    /// <param name="dmgs">(healthController) the amount of damages from the hit</param>
    /// <param name="idSource">(healthcontroller) the hit source</param>
    /// <param name="effects">(effectController) all the effects qttqched to the hit</param>
    public void receiveHit(int dmgs, NetworkInstanceId idSource, _EFfectParent[] effects = null)
    {
        if(healthCtrl)
            healthCtrl.TakeDamages(idSource, dmgs);

        if(effectCtrl && effects != null)
            effectCtrl.addEffects(effects);
    }

    public void land()
    {
        animCtrl.anim_play(animCtrl.aInfos[8],.2f);
        animCtrl.anim_play(animCtrl.aInfos[2], false);

    }

    /// <summary>
    /// Physics Layer updates
    /// </summary>
    /// <returns>if has changed</returns>
    private bool _UpdateLayerThrough()
    {
        if(testState(throughStateMask) != testState(State.CurrLayerThrough))
        {
            if(testState(throughStateMask))
                setState(State.CurrLayerThrough);
            else
                unsetState(State.CurrLayerThrough);

            physicsCtrl.updateLayer(testState(State.CurrLayerThrough), CharMove.throughDuration);
        }
        return testState(State.CurrLayerThrough);
    }

    /// <summary>
    /// Physics Layer update (public)
    /// </summary>
    /// <param name="extCallThroughNeeds">Needed state from caller</param>
    /// <param name="extCallState">Caller State</param>
    /// <returns>if has changed</returns>
    public bool _UpdateLayerThrough(bool extCallThroughNeeds, State extCallState)
    {
        if(extCallThroughNeeds)
            setState(extCallState);
        else
            unsetState(extCallState);

        _UpdateLayerThrough();

        return testState(State.CurrLayerThrough);
    }

    // ===================== Character Move Functions Links ====================
    public void cMove_Charge(Vector3 direction, float Speed, float duration)
    {
        CharMove.Charge(direction, duration, Speed);
    }

    /*
    public void cMove_Impulse(Vector3 direction, float initForce)
    {
        CharMove.impulse(direction, initForce);
    }
    */

    public void cMove_Impulse(Vector3 dirForceV)
    {
        CharMove.impulse(dirForceV.normalized, Vector3.Magnitude(dirForceV));
    }

    // ====================== Applying Effects =================================
    public void efApplyStun()
    {
        setState(State.Stun);
        CharMove.applyStun();
        animCtrl.Pause();
    }

    public void efEndStun()
    {
        if(testState(State.Stun))
        {
            unsetState(State.Stun);
            CharMove.revertStun();
            animCtrl.Unpause();
        }
    }

    public float efApplySlow(float slowAmount)
    {
        float _tmpSpeedMod = CharMove.speedMod;
        CharMove.speedMod = Mathf.Clamp(CharMove.speedMod - Mathf.Abs(slowAmount), CharMove.minSpeedMod, CharMove.maxSpeedMod);
        return _tmpSpeedMod - CharMove.speedMod;
    }

    public float efRevertSlow(float initSlowAmount)
    {
        float _tmpSpeedMod = CharMove.speedMod;
        CharMove.speedMod = Mathf.Clamp(CharMove.speedMod + Mathf.Abs(initSlowAmount), CharMove.minSpeedMod, CharMove.maxSpeedMod);
        return CharMove.speedMod - _tmpSpeedMod;
    }

    /*
    public void efApplyDoT(int dmgs, int idSource)
    {
        healthCtrl.TakeDamages(idSource, dmgs);
    }
    */

    public void efBump(Vector3 dir, float initForce)
    {
        CharMove.impulse(dir, initForce);
    }

    #region Coroutine Actions

    //=============== Coroutine Actions =============
    IEnumerator _r_Jump()
    {
        animCtrl.anim_play(animCtrl.aInfos[2], true);
        yield return new WaitForSeconds(animCtrl.aInfos[2].delay_action);
        CharMove.Jump();
        do
        {
            yield return new WaitForEndOfFrame();
        }
        while (!testState(State.Fall));
        _UpdateLayerThrough();
        animCtrl.anim_play(animCtrl.aInfos[2], false);
        yield return null;
    }

    IEnumerator _r_Hit()
    {
        Debug.Log("hit");
        s_SetBaseAttack();
        animCtrl.anim_play(animCtrl.aInfos[7]);
        weaponCtrl.WeaponAttack();
        StartCoroutine(_r_HitCD(baseAttackCD));
        yield return new WaitForSeconds(animCtrl.aInfos[7].clipToPlay.length / animCtrl.aInfos[7].animSpeed);
        s_UnsetBaseAttack();
        weaponCtrl.ForcedAttackEnd();
        yield return null;
    }

    IEnumerator _r_HitCD(float cd)
    {
        setState(State.BaseAttack_onCD);
        yield return new WaitForSeconds(cd);
        unsetState(State.BaseAttack_onCD);
    }

    IEnumerator _r_Spell(State ability)
    {
        StartCoroutine(_r_SpellAnim(animCtrl.aInfos[6], ability));
        yield return new WaitForSeconds(animCtrl.aInfos[6].delay_action);
        int  _idAb = -1;
        switch(ability)
        {
            case State.AbilityRB:
                _idAb = 0;
                break;
            case State.AbilityLB:
                _idAb = 1;
                break;
            case State.AbilityRT:
                _idAb = 2;
                break;
        }
        abilityCtrl.UseAbility(_idAb);

    }

    IEnumerator _r_SpellAnim(AnimController.AnimationClipInfos anim, State ability)
    {
        setState(ability);
        animCtrl.anim_play(anim);
        yield return new WaitForSeconds(anim.clipToPlay.length / anim.animSpeed);
        unsetState(ability);
    }

    IEnumerator _r_Ultimate()
    {
        StartCoroutine(_r_UltimateAnim(animCtrl.aInfos[6], ultimateAnimDuration));
        yield return new WaitForSeconds(animCtrl.aInfos[6].delay_action);
    }

    IEnumerator _r_UltimateAnim(AnimController.AnimationClipInfos anim, float duration)
    {
        s_SetUltimate();
        animCtrl.anim_play(anim, duration);
        yield return new WaitForSeconds(duration);
        s_UnsetUltimate();
    }

    IEnumerator _r_Charge(Vector3 chargeDir)
    {
        s_SetCharge();
        animCtrl.anim_play(animCtrl.aInfos[4], true);
        yield return new WaitForSeconds(animCtrl.aInfos[4].delay_action);
        yield return new WaitForSeconds(CharMove.Charge(chargeDir));
        s_UnsetCharge();
        animCtrl.anim_play(animCtrl.aInfos[4], false);
    }
    #endregion


    #region _currentState functions

    //=============== GetState Global =============
    public bool testState(State testState)
    {
        return (_currentState & testState) != 0;
    }

    private void setState(State state)
    {
        _currentState |= state;
    }

    private void unsetState(State state)
    {
        _currentState &= ~state;
    }

    //Horizontal Move State =======
    #region HMove
    public void s_SetHMove()
    {
        _currentState |= State.hMove;
    }

    public void s_UnsetHMove()
    {
        _currentState &= ~State.hMove;
    }
    #endregion

    //Jump State ==================
    #region Jump
    public void s_SetJump()
    {
        _currentState |= State.Jump;
    }

    public void s_UnsetJump()
    {
        _currentState &= ~State.Jump;
    }
    #endregion

    //FastFall State ===============
    #region FastFall
    public void s_SetFastFall()
    {
        _currentState |= State.FastFall;
    }

    public void s_UnsetFastFall()
    {
        _currentState &= ~State.FastFall;
    }
    #endregion

    //Through Platform State =======
    #region ThroughPlatform
    public void s_SetThroughPlatform()
    {
        _currentState |= State.ThroughPlatform;
    }

    public void s_UnsetThroughPlatform()
    {
        _currentState &= ~State.ThroughPlatform;
    }
    #endregion

    //Dodge State ==================
    #region Dodge
    public void s_SetDodge()
    {
        _currentState |= State.Dodge;
    }

    public void s_UnsetDodge()
    {
        _currentState &= ~State.Dodge;
    }
    #endregion

    //Base Attack State ============
    #region BaseAttack
    public void s_SetBaseAttack()
    {
        _currentState |= State.BaseAttack;
    }

    public void s_UnsetBaseAttack()
    {
        _currentState &= ~State.BaseAttack;
    }
    #endregion

    //Ability State ================
    #region Ability
    public void s_SetAbility()
    {
        _currentState |= State.AbilityLB;
    }

    public void s_UnsetAbility()
    {
        _currentState &= ~State.AbilityLB;
    }
    #endregion

    //Ultimate State ===============
    #region Ultimate
    public void s_SetUltimate()
    {
        _currentState |= State.Ultimate;
    }

    public void s_UnsetUltimate()
    {
        _currentState &= ~State.Ultimate;
    }
    #endregion

    //Bump State ===================
    #region Bump
    public void s_SetBump()
    {
        _currentState |= State.Bump;
    }

    public void s_UnsetBump()
    {
        _currentState &= ~State.Bump;
    }
    #endregion

    //Stun State ==================
    #region Stun
    public void s_SetStun()
    {
        _currentState |= State.Stun;
    }

    public void s_UnsetStun()
    {
        _currentState &= ~State.Stun;
    }
    #endregion

    //Slow State =================
    #region Slow
    public void s_SetSlow()
    {
        _currentState |= State.Slow;
    }

    public void s_UnsetSlow()
    {
        _currentState &= ~State.Slow;
    }
    #endregion

    //Fall State =================
    #region Fall
    public void s_SetFall()
    {
        _currentState |= State.Fall;
        if(animCtrl)
            animCtrl.anim_play(animCtrl.aInfos[3], true);
    }

    public void s_UnsetFall()
    {
        _currentState &= ~State.Fall;
        if(animCtrl)
            animCtrl.anim_play(animCtrl.aInfos[3], false);
    }
    #endregion

    //Charge State =================
    #region Charge
    public void s_SetCharge()
    {
        _currentState |= State.Charge;
    }

    public void s_UnsetCharge()
    {
        _currentState &= ~State.Charge;
    }
    #endregion

    #endregion

    void OnDrawGizmos()
    {

    }
}
