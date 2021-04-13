using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// Have to be defined somewhere in a runtime script file
public class BitMaskAttribute: PropertyAttribute
{
    public System.Type propType;
    public BitMaskAttribute(System.Type aType)
    {
        propType = aType;
    }
}

public class CharacterMove : NetworkBehaviour
{

    // ===============================PUBLIC ===============================
    #region Movement Parameter
    [Header("Move Parameters", order = 0)]
    public float MoveSpeed;
    public float speedMod = 1.0f;
    public float minSpeedMod = .2f;
    public float maxSpeedMod = 1.0f;
    public float throughDuration;


    [Header("Jump Parameters", order =1)]
    public int NbJump;
    public float JumpHeight;
    public float JumpGravity;
    public float JumpGravityMod;

    [Header("Fall / FastFall Parameters",order = 2)]
    public float FastFallingMod;
    public float minFastFallSpeed;
    public float FallSpeed;
    public float fallAniticipationTime = .2f;
    public float gravityMod = 1.0f;

    [Header("Dodge Parameters", order = 3)]
    public float dodgeDuration;
    public float DodgeCooldown;
    public float dodgeSpeed;
    public int nbAirDodge;

    [Header("Charge Parameters", order = 4)]
    public float chargeDuration;
    public float chargeSpeed;
    #endregion

    #region Avatar Orientation
    [Header("Orientation Position parameters", order = 5)]
    public Quaternion rightRot;
    public Quaternion leftRot;
    public GameObject charAvatar;

    [SyncVar]
    public int dirChar = 1;

    public int DirChar {
        get { return dirChar; }
    }
    #endregion

    #region Components References
    [Header("GameObject Components references (self init if null)", order = 6)]
    public CharacterController CharCtrl;
    public Coordinator coor;
    #endregion

    #region routine Behavior Parameters
    [Header("Active Message for state update", order = 7)]
    public bool stateUpdateEvent = false;

    public enum CharMoveState
    {
        _cmNull = 0,
        _cmHMove = (1 << 0),
        _cmJump = (1 << 1),
        _cmFastFall = (1 << 2),
        _cmDodge = (1 << 3),
        _cmFall = (1 << 4),
        _cmIsGrounded = (1 << 5),
        _cmNoPhysics = (1 << 6),
        _cmNoInputs = (1 << 7),
        _cmCharge = (1 << 8),
        _cmImpulse = (1 << 9),
        _cmLayerThrough = (1 << 10),
    }

    [Header("Current CharMoveState of this gameObject", order = 8)]
    [BitMask(typeof(CharMoveState))]
    public CharMoveState _cmState;

    [BitMask(typeof(CharMoveState))]
    public CharMoveState _throughStateMask;
    [BitMask(typeof(CharMoveState))]
    public CharMoveState[] _throughExceptions;

    private CharMoveState temp_cmState;
    #endregion
    [Header("Networking Infos", order = 9)]

    // ================================PRIVATE=============================
    private Vector3 MoveVector;
    private float jumpTime = -1.0f;
    private int lastDir = 1; //-1 = left rotation, 0 = no Change, 1 = Right Rotation

    //[SyncVar]
    private Vector3 _syncMoveV;

    //[Command]
    void Cmd_SyncMove(Vector3 syncMove, Vector3 originPos) {
        transform.position = originPos;
        _syncMoveV = syncMove;
    }

    #region Var dodge & Charge
    private int currentAirDodge = 1;
    private Vector3 dodgeVector;
    private Vector3 dashVector;
    #endregion

    #region Var jump/Gravity
    private int CurrentJump = 0;
    private bool FallMod = false;
    private float fallClamp;
    public float _currentGravity;
    #endregion

    #region Var routine results
    private IEnumerator _rChargeRef = null;
    #endregion

    // Use this for initialization
    void Start()
    {
        if (!CharCtrl)
        {
            this.GetComponent<CharacterController>();
        }

        if (stateUpdateEvent)
            StartCoroutine(stateUpdate());

        
        MoveVector.y = -.2f;
        _currentGravity = _gravityUpdate();
        temp_cmState = _cmState;
        if(coor && !coor.InptsCtrl.isLocalPlayer)
            _cmSetState(CharMoveState._cmNoPhysics);

    }

    public void avatarSet(GameObject avatar)
    {
        charAvatar = avatar;
        rightRot = charAvatar.transform.localRotation;
    }

    // Update is c alled once per frame
    void Update()
    {
        #region Old
        /*
        if (jumpTime <= Time.time)
        {
            MoveVector.y -= (Mathf.Abs(FallSpeed) * Time.deltaTime)*5;
            if(MoveVector.y <= -Mathf.Abs(FallSpeed))
            {
                MoveVector.y = -Mathf.Abs(FallSpeed);
            }
            if ((lastFrameState & 2) != 0) Coor.noJump();
        }
        else
        {
            MoveVector.y = JumpSpeed;
        }

        if (MoveVector.y >= 0 || CharCtrl.isGrounded) FastFallSign = true;
        if (MoveVector.y <0 && !CharCtrl.isGrounded && FastFallSign)
        {
            FastFallSign = false;
            Debug.Log("Can FastFall");
        }

        if(MoveVector.y>0 && Physics.SphereCast(new Ray(transform.position, transform.up * 1), 1, .2f))
        {
            MoveVector.y = 0;
        }

        Vector3 endVector = MoveVector;
        if (MoveVector.y < 0 && FallMod) endVector.y *= FastFallingMod;


        if (CharCtrl.isGrounded) MoveVector.y = -0.1f;
        if (MoveVector.y > 0 || Time.time <= endThroughTime)
        {
            this.gameObject.layer = LayerMask.NameToLayer(LayerThrough);
        }
        else gameObject.layer = LayerMask.NameToLayer(baseLayer);
        CharCtrl.Move(endVector * Time.deltaTime);



        lastFrameState = Coor.currentState;
        */
        #endregion

        #region old v.2

        /*
        fallClamp = FallMod ? FallSpeed * FastFallingMod : FallSpeed;


        //Model Rotation & Collider Positionning
        if(MoveVector.z != 0 && Mathf.Sign(MoveVector.z) != lastDir)
        {
            lastDir = (int)Mathf.Sign(MoveVector.z);

            if (lastDir < 0)
                charAvatar.transform.rotation = leftRot;
            else
                charAvatar.transform.rotation = rightRot;

            CharCtrl.center = CharCtrl.center*-1;
        }


        if (CharCtrl.isGrounded)
        {
            currentAirDodge = nbAirDodge;

            if (MoveVector.y < 0) 
            {
                MoveVector.y = -.1f;
                CurrentJump = NbJump;
            }
            Coor.s_UnsetFall();
        }
        else
        {
            if (MoveVector.y < 0)
            {
                Coor.s_SetFall();
                if (_cmGetState(CharMoveState._cmFastFall))
                    MoveVector.y *= FastFallingMod;
            }
            else
                Coor.s_UnsetFall();
        }
        
        if (MoveVector.y > 0 || endThroughTime > Time.time)
        {
            MoveVector.y -= JumpGravity * gravityMod * Time.deltaTime;
            gameObject.layer = LayerMask.NameToLayer(LayerThrough);
            Coor.s_UnsetThroughPlatform();
        }
        else
        {
            MoveVector.y -= FallSpeed * gravityMod * Time.deltaTime;
            gameObject.layer = LayerMask.NameToLayer(baseLayer);
        }

        if (MoveVector.y <= 0 && Coor.testState(Coordinator.State.Jump))
            Coor.s_UnsetJump();
            
        
        dodgeVector = Time.time < endDodgeTime ? dodgeVector : Vector3.zero;

        if (Time.time > endDodgeTime)
            Coor.s_UnsetDodge();
        else
            gameObject.layer = LayerMask.NameToLayer(DodgeLayer); <-------------------------------------------- Physics Script
        
        */

        #endregion

        #region Update Move
        /*
        if(!_cmGetState(CharMoveState._cmCharge) && !_cmGetState(CharMoveState._cmDodge))
        {

            if(!_cmGetState(CharMoveState._cmIsGrounded) && CharCtrl.isGrounded)
            {
                _ResetGround();
                _cmSetState(CharMoveState._cmIsGrounded);
            } else if(!CharCtrl.isGrounded && _cmGetState(CharMoveState._cmIsGrounded))
            {
                _cmUnsetState(CharMoveState._cmIsGrounded);
                StartCoroutine(airTests());
            }
        }

        if (!_cmGetState(CharMoveState._cmNoPhysics))
        {
            if (!_cmGetState(CharMoveState._cmIsGrounded))
                MoveVector.y -= _currentGravity * Time.deltaTime;

            if (_cmGetState(CharMoveState._cmNoInputs))
                MoveVector = Vector3.zero;

            MoveVector.y = Mathf.Clamp(MoveVector.y, -Mathf.Abs(fallClamp), MoveVector.y);
            CharCtrl.Move((_cmGetState(CharMoveState._cmDodge) ? dodgeVector : _cmGetState(CharMoveState._cmCharge) ? dashVector : MoveVector) * Time.deltaTime);
        }
        */
        #endregion

        /*
        Vector3 dir = new Vector3(0,.5f,.5f);

        if(Input.GetKeyDown(KeyCode.H))
            impulse(dir.normalized, 10);
        if(Input.GetKeyDown(KeyCode.J))
            StopCharge();
        */
    }

    void FixedUpdate()
    {
        #region Update Move
        if(!_cmGetState(CharMoveState._cmNoPhysics))
        {

            if(!_cmGetState(CharMoveState._cmCharge) && !_cmGetState(CharMoveState._cmDodge))
            {

                if(!_cmGetState(CharMoveState._cmIsGrounded) && CharCtrl.isGrounded)
                {
                    _ResetGround();
                    _cmSetState(CharMoveState._cmIsGrounded);
                } else if(!CharCtrl.isGrounded && _cmGetState(CharMoveState._cmIsGrounded))
                {
                    _cmUnsetState(CharMoveState._cmIsGrounded);
                    StartCoroutine(airTests());
                }
            }

            if(!_cmGetState(CharMoveState._cmIsGrounded))
                MoveVector.y -= _currentGravity * Time.deltaTime;
            if(_cmGetState(CharMoveState._cmNoInputs) && !_cmGetState(CharMoveState._cmImpulse))
                MoveVector.z = 0;
            MoveVector.y = Mathf.Clamp(MoveVector.y, -Mathf.Abs(fallClamp), MoveVector.y);
            _syncMoveV = (_cmGetState(CharMoveState._cmDodge) ? dodgeVector : MoveVector);
            Cmd_SyncMove(_syncMoveV, transform.position);

            CharCtrl.Move(_syncMoveV * speedMod * Time.deltaTime);
        }
        #endregion
        if(charAvatar)
            charAvatar.transform.rotation = dirChar > 0 ? rightRot : leftRot;

    }


    public bool anticipattionGrounded()
    {
        bool retV = false;
        if(MoveVector.y <= 0)
        {
            Vector3 _originPos = transform.position;
            _originPos.y -= CharCtrl.height / 2;
            Vector3 _endPos = _originPos;
            _endPos.y -= MoveVector.y * fallAniticipationTime;
            Ray _r = new Ray(_originPos,Vector3.down);
            if(Physics.Raycast(_r, .7f))
                retV = true;
        }
        _UpdateLayer();
        return retV;
    }

    /// <summary>
    /// Change the current gravity speed, as well as the current state of the character
    /// </summary>
    /// <returns>Returns the current gravity to apply</returns>
    float _gravityUpdate()
    {
        float currentGravity = 0;
        if (!_cmGetState(CharMoveState._cmNoPhysics))
        {
            currentGravity = FallSpeed;
            if (MoveVector.y <= 0)
            {
                if (!_cmGetState(CharMoveState._cmIsGrounded))
                {
                    _cmSetState(CharMoveState._cmFall);
                    _cmUnsetState(CharMoveState._cmJump);
                    
                    coor.s_SetFall(); // coor update
                    coor.s_UnsetJump(); // Coor update
                    if(anticipattionGrounded())
                    {
                        //Debug.Log("Anticipation Falling : " + anticipattionGrounded());
                        coor.land();
                    }
                    if(_cmGetState(CharMoveState._cmFastFall))
                    {
                        currentGravity = FallSpeed * FastFallingMod;
                    }
                }
            }
            else
            {
                _cmUnsetState(CharMoveState._cmFall);
                coor.s_UnsetFall(); // coor Update

                if(_cmGetState(CharMoveState._cmJump))
                {
                    currentGravity = JumpGravity;
                    coor.physicsCtrl.changeLayerToThrough();
                } else
                {
                    _cmUnsetState(CharMoveState._cmJump);
                    coor.s_UnsetJump(); // coor update
                }
            }

            currentGravity *= gravityMod;
        }
        _UpdateLayer();
        return currentGravity;
    }

    /// <summary>
    /// Reset the value of needed var to the grounded state
    /// </summary>
    void _ResetGround()
    {
        if (MoveVector.y < 0)
        {
            MoveVector.y = -.1f;
            CurrentJump = NbJump;
            currentAirDodge = nbAirDodge;

            _cmUnsetState(CharMoveState._cmJump);
            _cmUnsetState(CharMoveState._cmFall);
            _cmUnsetState(CharMoveState._cmFastFall);
            _cmUnsetState(CharMoveState._cmImpulse);
            _cmSetState(CharMoveState._cmIsGrounded);

            coor.s_UnsetJump();
            coor.s_UnsetFall();
            coor.s_UnsetFastFall();
        }
        _UpdateLayer();
    }


    //Input move value
    public void Move(float InputValue)
    {
        if (InputValue != 0)
        {
            _cmSetState(CharMoveState._cmHMove);
            if(_cmGetState(CharMoveState._cmImpulse) && MoveVector.y <= 0)
                _cmUnsetState(CharMoveState._cmImpulse);
            /*
            if (dirChar != Mathf.Sign(InputValue))
            {
                dirChar = (int)Mathf.Sign(InputValue);
                charAvatar.transform.rotation = Mathf.Sign(InputValue) > 0 ? rightRot : leftRot;
                CharCtrl.center *= -1;
            }*/
        }
        else
            _cmUnsetState(CharMoveState._cmHMove);

        if(CanInput() )
        {
            MoveVector.z = InputValue * MoveSpeed;
            if(dirChar != Mathf.Sign(InputValue) && _cmGetState(CharMoveState._cmHMove))
            {
                if (isLocalPlayer) Cmd_TurnChar(InputValue);
                //charAvatar.transform.rotation = dirChar > 0 ? rightRot : leftRot;
                CharCtrl.center *= -1;
            }
        }
    }

    [Command]
    void Cmd_TurnChar(float _InputValue)
    {
        dirChar = (int)Mathf.Sign(_InputValue);
    }

    public bool CanRun()
    {
        return _cmGetState(CharMoveState._cmIsGrounded);
    }

    /// <summary>
    /// Test if the player can send inputs actions to the player
    /// </summary>
    /// <returns>the possibility to send actions to the player</returns>
    public bool CanInput()
    {
        return !(_cmGetState(CharMoveState._cmNoInputs) || _cmGetState(CharMoveState._cmNoPhysics));
    }

    /// <summary>
    /// Test if the current number of jumps allow the character to jump
    /// </summary>
    /// <returns>the possibility to jump</returns>
    public bool CanJump()
    {
        return CurrentJump > 0 && (CanInput() || (_cmGetState(CharMoveState._cmImpulse) && MoveVector.y <= 0));
    }

    //Jump Action
    public void Jump()
    {
        //Debug.Log("jump");
        if (CanJump())
        {
            MoveVector.y = Mathf.Sqrt(2 * JumpHeight * JumpGravity) + .1f;
            CurrentJump--;
            _cmSetState(CharMoveState._cmJump);
            _cmUnsetState(CharMoveState._cmImpulse);
        }
        _UpdateLayer();
    }

    //Activate fast fall
    public bool FastFalling()
    {
        bool retB = !CharCtrl.isGrounded;
        if (retB)
        {
            _cmSetState(CharMoveState._cmFastFall);
            fallClamp = FallSpeed * FastFallingMod;
        }
        _UpdateLayer();
        return retB;
    }

    //Revert Fall speed on demand
    public void StopFastFalling()
    {
        _cmUnsetState(CharMoveState._cmFastFall);
        coor.s_UnsetFastFall(); // Coor Update
        fallClamp = FallSpeed;
        _UpdateLayer();
    }

    public void impulse(Vector3 direction, float initForce)
    {
        StartCoroutine(_rImpulse(direction, initForce));
        _UpdateLayer();
    }

    /*
    //Apply Bumber Modifier to Character
    public void Bumper(Vector3 VelocityBump)
    {
        MoveVector = Vector3.zero;
        MoveVector += VelocityBump;
    }*/

    /// <summary>
    /// Start the dodge, must be manually stopped
    /// </summary>
    /// <param name="direction">dodge direction</param>
    /// <returns>the possibility to actually dodge</returns>
    public float Dodge(Vector3 direction)
    {
        bool retV;
        if (retV = (CanInput() && !_cmGetState(CharMoveState._cmDodge) && currentAirDodge > 0))
        {
            _cmSetState(CharMoveState._cmDodge);
            dodgeVector = direction.normalized * dodgeSpeed;
            StartCoroutine(_rDodge(dodgeDuration));
        }
        _UpdateLayer();
        return dodgeDuration;
    }

    /// <summary>
    /// Start the dodge, with the internal duration gesture
    /// </summary>
    /// <param name="direction">dodge direction</param>
    /// <param name="duration">dodge duration</param>
    /// <returns>the possibility to actually dodge</returns>
    public float Dodge(Vector3 direction, float duration)
    {
        bool retV;
        if (retV = (CanInput() && !_cmGetState(CharMoveState._cmDodge) && currentAirDodge > 0))
            _cmSetState(CharMoveState._cmDodge);
            dodgeVector = direction.normalized * dodgeSpeed;
            StartCoroutine(_rDodge(duration));

        _UpdateLayer();
        return duration;
    }

    /// <summary>
    /// Stop the dodge, either it was with a routine or not
    /// </summary>
    /// <returns>if the char was dodging</returns>
    public bool EndDodge()
    {
        if (_cmGetState(CharMoveState._cmDodge))
        {
            StopCoroutine("_rDodge");
            if (!_cmGetState(CharMoveState._cmIsGrounded))
                currentAirDodge--;
            _cmUnsetState(CharMoveState._cmDodge);
            coor.s_UnsetDodge(); //Coor update
        }
        _UpdateLayer();
        return _cmGetState(CharMoveState._cmDodge);
    }

    /// <summary>
    /// Start the charge,  must be manually stopped
    /// </summary>
    /// <param name="direction">dodge direction</param>
    /// <param name="duration">dodge duration</param>
    /// <returns>the possibility to actually dodge</returns>
    public float Charge(Vector3 direction)
    {
        bool retV;
        if(retV = (CanInput() && !_cmGetState(CharMoveState._cmCharge) && currentAirDodge > 0))
            _cmSetState(CharMoveState._cmCharge);
        dashVector = direction.normalized * chargeSpeed;
        StartCoroutine(_rCharge(chargeDuration));
        _UpdateLayer();
        return chargeDuration;
    }

    /// <summary>
    /// Start the charge, with the internal duration gesture
    /// </summary>
    /// <param name="direction">dodge direction</param>
    /// <param name="duration">dodge duration</param>
    /// <returns>the possibility to actually dodge</returns>
    public float Charge(Vector3 direction, float duration)
    {
        bool retV;
        if(retV = (CanInput() && !_cmGetState(CharMoveState._cmCharge) && currentAirDodge > 0))
            _cmSetState(CharMoveState._cmCharge);
        dashVector = direction.normalized * chargeSpeed;
        StartCoroutine(_rCharge(duration));
        _UpdateLayer();
        return duration;
    }

    /// <summary>
    /// Start a charging move, to the specified direction, with the set speed and duration.
    /// The character won't be impacted by gravity or other physic interraction, except hard collisions, for the full duration of the charge
    /// </summary>
    /// <param name="direction">direction of the charging movement</param>
    /// <param name="duration">duration of the charging movement</param>
    /// <param name="speed">speed of the movement</param>
    /// <returns>if the charge is currently possible</returns>
    public bool Charge(Vector3 direction, float duration,float speed)
    {
        bool retV = false;
        if(direction != Vector3.zero && duration > 0 && speed > 0 
                && !_cmGetState(CharMoveState._cmCharge) 
                && !_cmGetState(CharMoveState._cmNoPhysics) 
                && !_cmGetState(CharMoveState._cmNoInputs))
        {
            _rChargeRef = _rTimedCharge(duration);
            StartCoroutine(_rChargeRef);
            StartCoroutine(_rMoveCharge(direction, speed));
            _gravityUpdate();
            retV = true;
        }
        _UpdateLayer();
        return retV;
    }

    /// <summary>
    /// Stop the current charging movement and reset the impact of gravity and physics application to the character
    /// </summary>
    /// <returns>if the character was charging</returns>
    public bool StopCharge()
    {
        bool retV = _cmGetState(CharMoveState._cmCharge);
        if(_rChargeRef != null)
            StopCoroutine(_rChargeRef);
        _cmUnsetState(CharMoveState._cmCharge);
        _UpdateLayer();
        return retV;
    }

    /// <summary>
    /// Stop the charge, either it was with a routine or not
    /// </summary>
    /// <returns>if the char was dodging</returns>
    public bool EndCharge()
    {
        if(_cmGetState(CharMoveState._cmCharge))
        {
            StopCoroutine("_rCharge");
            _cmUnsetState(CharMoveState._cmCharge);
            coor.s_UnsetCharge(); //Coor update
        }
        _UpdateLayer();
        return _cmGetState(CharMoveState._cmCharge);
    }

    private bool _UpdateLayer()
    {
        CharMoveState _tmpMask = _throughStateMask;
        foreach(CharMoveState st in _throughExceptions)
        {
            if(_cmGetValue(_throughStateMask, st) == (int)st)
                _tmpMask = _cmUnsetState(_tmpMask, st);
        }

        if(_cmGetState(_tmpMask) != _cmGetState(CharMoveState._cmLayerThrough))
            if(_cmGetState(CharMoveState._cmLayerThrough))
                _cmUnsetState(CharMoveState._cmLayerThrough);
            else
                _cmSetState(CharMoveState._cmLayerThrough);

        coor._UpdateLayerThrough(_cmGetState(CharMoveState._cmLayerThrough), Coordinator.State.CharMoveThrough);
        return _cmGetState(CharMoveState._cmLayerThrough);
    }

    public void applyStun()
    {
        _cmSetState(CharMoveState._cmNoPhysics & CharMoveState._cmNoInputs);
    }

    public void revertStun()
    {
        _cmUnsetState(CharMoveState._cmNoInputs & CharMoveState._cmNoPhysics);
    }

    public void PublicUnset(CharMoveState State)
    {
        _cmUnsetState(State);
    }

    // =================== STATE ==========================
    #region _cmState Gesture
    void _cmSetState(CharMoveState State)
    {
        _cmState |= State;
    }

    CharMoveState _cmSetState(CharMoveState sourceState, CharMoveState State)
    {
        return sourceState |= State;
    }

    void _cmUnsetState(CharMoveState State)
    {
        _cmState &= ~State;
    }

    CharMoveState _cmUnsetState(CharMoveState sourceState, CharMoveState State)
    {
        return sourceState &= ~State;
    }

    bool _cmGetState(CharMoveState State)
    {
        return (_cmState & State) != 0;
    }

    bool _cmGetState(CharMoveState sourceState, CharMoveState State)
    {
        return (sourceState & State) != 0;
    }

    int _cmGetValue(CharMoveState State)
    {
        return (int)(_cmState & State);
    }

    int _cmGetValue(CharMoveState sourceState, CharMoveState State)
    {
        return (int)(sourceState & State);
    }

    /*
    /// <summary>
    /// send the current state of the character move, separated by every actions
    /// </summary>
    /// <param name="isHmove">moving action</param>
    /// <param name="isDodge">dodging action</param>
    /// <param name="isJump">jumping action</param>
    /// <param name="isFall">falling state</param>
    /// <param name="isFastFall">fastfalling action</param>
    /// <param name="isGrounded">isGrounded state</param>
    /// <returns></returns>
    public bool getFullState(out bool isHmove, out bool isDodge, out bool isJump, out bool isFall, out bool isFastFall, out bool isGrounded)
    {
        bool retV;
        retV = temp_cmState != _cmState;
        isHmove = _cmGetState(CharMoveState._cmHMove);
        isDodge = _cmGetState(CharMoveState._cmDodge);
        isJump = _cmGetState(CharMoveState._cmJump);
        isFall = _cmGetState(CharMoveState._cmFall);
        isFastFall = _cmGetState(CharMoveState._cmFastFall);
        isGrounded = _cmGetState(CharMoveState._cmIsGrounded);

        if (retV)
            temp_cmState = _cmState;

        return retV;
    }

    public CharMoveState getChanges()
    {
        CharMoveState retV = _cmState ^ temp_cmState;
        return retV;
    }
    */
    #endregion

    
    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 _originPoint = transform.position;
        _originPoint.y -= CharCtrl.height / 2;
        Gizmos.DrawRay(_originPoint, Vector3.down * _currentGravity * fallAniticipationTime);
    }
    */

    //========================================================================== Coroutine ==================================================================

    /// <summary>
    /// update the gravity speed when in the air
    /// </summary>
    /// <returns>EndOfFrame</returns>
    IEnumerator airTests()
    {
        _cmUnsetState(CharMoveState._cmIsGrounded);
        while (!_cmGetState(CharMoveState._cmIsGrounded))
        {
            _currentGravity = _gravityUpdate();
            yield return new WaitForEndOfFrame();
        }
        _currentGravity = _gravityUpdate();

        yield return null;
    }

    /// <summary>
    /// used to get the dodge going for the specified duration
    /// </summary>
    /// <param name="duration">the dodge duration</param>
    /// <returns>WaitForSeconds(duration)</returns>
    IEnumerator _rDodge(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndDodge();
    }

    /// <summary>
    /// used to get the charge going for the specified duration
    /// </summary>
    /// <param name="duration">the charge duration</param>
    /// <returns>WaitForSeconds(duration)</returns>
    IEnumerator _rCharge(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndCharge();
    }

    /// <summary>
    /// Add an impulse force to the character
    /// </summary>
    /// <param name="direction">direction of the impulsion</param>
    /// <param name="initForce">force of the impulsion</param>
    /// <returns></returns>
    IEnumerator _rImpulse(Vector3 direction, float initForce)
    {
        direction.Normalize();
        _cmSetState(CharMoveState._cmImpulse);
        _cmSetState(CharMoveState._cmNoInputs);
        MoveVector = direction * initForce;
        do
        {
            yield return new WaitForEndOfFrame();
        } while(_cmGetState(CharMoveState._cmImpulse));
        _cmUnsetState(CharMoveState._cmImpulse);
        _cmUnsetState(CharMoveState._cmNoInputs);
        _UpdateLayer();

        yield return null;
    }


    /// <summary>
    /// Charge Movement applied each frame (cancellation of physics or input interraction)
    /// </summary>
    /// <param name="direction">direction of the charge</param>
    /// <param name="chargeSpeed">speed of the charge</param>
    /// <returns></returns>
    IEnumerator _rMoveCharge(Vector3 direction, float chargeSpeed)
    {
        _cmSetState(CharMoveState._cmNoPhysics);
        _cmSetState(CharMoveState._cmNoInputs);
        do
        {
            CharCtrl.Move(direction * chargeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        } while(_cmGetState(CharMoveState._cmCharge));

        _cmUnsetState(CharMoveState._cmNoPhysics);
        _cmUnsetState(CharMoveState._cmNoInputs);
        _UpdateLayer();
        yield return null;
    }

    /// <summary>
    /// Set the charging state to true, and set it back to false after the specified duration
    /// </summary>
    /// <param name="duration">waiting time to revert the charging state</param>
    /// <returns></returns>
    IEnumerator _rTimedCharge(float duration)
    {
        _cmSetState(CharMoveState._cmCharge);
        yield return new WaitForSeconds(duration);
        _cmUnsetState(CharMoveState._cmCharge);
        _UpdateLayer();
    }

    /// <summary>
    /// send a message to every script on the same object when the state of the character movement changes
    /// </summary>
    /// <returns>WaitForEndOfFrame</returns>
    IEnumerator stateUpdate()
    {
        CharMoveState _tempState = _cmState;
        while (stateUpdateEvent)
        {
            if (_tempState != _cmState)
            {
                SendMessage("CharacterMoveStateUpdate", _cmState);
                _tempState = _cmState;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
