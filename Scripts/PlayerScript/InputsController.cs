using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class InputsController : NetworkBehaviour
{




    public float LeftJoystickHOffset = .3f;
    public float LeftJoystickVOffset = .3f;
    public float RightJoystickHOffset = .3f;
    public float RightJoystickVOffset = .3f;
    public float TriggersOffset = .8f;
    public bool InputType = true;
    public Coordinator Coor;
    public Vector2 targetV2 = Vector2.zero;

    #region old
    /*
    private bool AButtonUsed = false;
	private bool JumpUsed = false;
	private bool BButtonUsed = false;
    private bool XButtonUsed = false;
    private bool YButtonUsed = false;
    private bool RBButtonUsed = false;
    private bool LBButtonUsed = false;
    private bool LTriggerUsed = false;
    private bool RTriggerUsed = false;
    private bool LJH_is0 = true;

    public bool LJV_Down = false;
    private bool isThrough = false;*/
    #endregion

    //Le serveur actualise les variables et les transmets aux clients

    //MOVE AXIS
	public float MoveZ;

	public enum boolInput {
        nullV = 0,
		a_button = (1 << 0),	// 1 	(0000 0000 0001)
		b_button = (1 << 1),	// 2 	(0000 0000 0010)
		x_button = (1 << 2),	// 4 	(0000 0000 0100)
		y_button = (1 << 3),	// 8 	(0000 0000 1000)
		rb_button = (1 << 4),	// 16 	(0000 0001 0000)
		lb_button = (1 << 5),	// 32	(0000 0010 0000)
		rt_axis = (1 << 6),		// 64	(0000 0100 0000)
		lt_axis = (1 << 7),		// 128	(0000 1000 0000)
		down = (1 << 8),		// 256	(0001 0000 0000)
	}

	
	public boolInput InputValue=0; // min : 0 (0000 0000 0000) - max :(0001 1111 1111)
    

	private boolInput _lastInputValue = 0;

	
	public void Cmd_InputSync(float h_move, boolInput _InValue){
		MoveZ = h_move;
		InputValue = _InValue;
		//Debug.Log ("sync v : " + InputValue);
	}

    // Use this for initialization
    void Start () {
        Coor.FastFalling(true);
    }
	
	// Update is called once per frame
	void Update () {
		float LJH = Input.GetAxis("LJoystick_H");
		float LJV = Input.GetAxis("LJoystick_V");
		float RJH = Input.GetAxis("RJoystick_H");
		float RJV = Input.GetAxis("RJoystick_V");

		#region Inputs
		if (isLocalPlayer){
			InputValue = Input.GetButtonDown("A_Button") ? InputValue | boolInput.a_button : InputValue & ~boolInput.a_button;

			InputValue = Input.GetButtonDown("B_Button") ? InputValue | boolInput.b_button : InputValue & ~boolInput.b_button;

			InputValue = Input.GetButtonDown("X_Button") ? InputValue | boolInput.x_button : InputValue & ~boolInput.x_button;

			InputValue = Input.GetButtonDown("Y_Button") ? InputValue | boolInput.y_button : InputValue & ~boolInput.y_button;

			InputValue = Input.GetButtonDown("LB_Button") ? InputValue | boolInput.lb_button : InputValue & ~boolInput.lb_button;

			InputValue = Input.GetButtonDown("RB_Button") ? InputValue | boolInput.rb_button : InputValue & ~boolInput.rb_button;

			InputValue = Input.GetAxis ("LTrigger") > TriggersOffset ? InputValue | boolInput.lt_axis : InputValue & ~boolInput.lt_axis;

			InputValue = Input.GetAxis ("RTrigger") > TriggersOffset ? InputValue | boolInput.rt_axis : InputValue & ~boolInput.rt_axis;

			InputValue = LJV >  LeftJoystickHOffset ? InputValue | boolInput.down : InputValue & ~boolInput.down;

            Vector2 _rJoystick = ToolBox2D.Set1Vector(new Vector2(-RJH,RJV));
            Vector2 _lJoystick = ToolBox2D.Set1Vector(new Vector2(-LJH,LJV));

            targetV2 = _rJoystick != Vector2.zero ? _rJoystick : _lJoystick;
            
            //Sync
            Cmd_InputSync(Movement(LJH), InputValue);

            #region old
            /*if (Input.GetButtonDown ("B_Button") && !BButtonUsed) {
				BButtonUsed = true;
				Debug.Log ("B Button Pressed");
			}

			if (Input.GetButtonUp ("B_Button") && BButtonUsed) {
				BButtonUsed = false;
			}

			if (Input.GetButtonDown ("X_Button") && !XButtonUsed) {
				XButtonUsed = true;
				Debug.Log ("X Button Pressed");
			}
			if (Input.GetButtonUp ("X_Button") && XButtonUsed) {
				XButtonUsed = false;
			}

			if (Input.GetButtonDown ("Y_Button") && !YButtonUsed) {
				YButtonUsed = true;
				Debug.Log ("Y Button Pressed");
			}
			if (Input.GetButtonUp ("Y_Button") && YButtonUsed) {
				YButtonUsed = false;
			}

			if (Input.GetButtonDown ("LB_Button") && !LBButtonUsed) {
				LBButtonUsed = true;
				Debug.Log ("LB Button Pressed");
			}
			if (Input.GetButtonUp ("LB_Button") && LBButtonUsed) {
				LBButtonUsed = false;
			}

			if (Input.GetButtonDown ("RB_Button") && !RBButtonUsed) {
				RBButtonUsed = true;
				Debug.Log ("RB Button Pressed");
			}
			if (Input.GetButtonUp ("RB_Button") && RBButtonUsed) {
				RBButtonUsed = false;
			}

			if (Input.GetAxis ("LTrigger") > TriggersOffset && !LTriggerUsed) {
				LTriggerUsed = true;
				Debug.Log ("Left Trigger Pressed");
			}
			if (Input.GetAxis ("LTrigger") < TriggersOffset && LTriggerUsed) {
				LTriggerUsed = false;
			}

			if (Input.GetAxis ("RTrigger") > TriggersOffset && !RTriggerUsed) {
				RTriggerUsed = true;
				Debug.Log ("Right Trigger Pressed");
			}
			if (Input.GetAxis ("RTrigger") < TriggersOffset && RTriggerUsed) {
				RTriggerUsed = false;
			}*/

            #endregion
        }
        #endregion

        
        
            #region Inputs Command
            if(((InputValue & boolInput.a_button) != 0))
            {
                if((InputValue & boolInput.down) != 0)
                    Coor.GoThroughPlatform(false);
                else
                    Coor.Jump();
            }

            if((InputValue & boolInput.b_button) != 0) Coor.dodge(LJH, LJV);
            if((InputValue & boolInput.y_button) != 0) Coor.charge(LJH, LJV);
            if((InputValue & boolInput.x_button) != 0) Coor.Hit();
            if((InputValue & boolInput.lb_button) != 0) Coor.SpellLaunch(1);
            if((InputValue & boolInput.rb_button) != 0) Coor.SpellLaunch(2);
            if((InputValue & boolInput.rt_axis) != 0) Coor.SpellLaunch(3);
            if((InputValue & boolInput.lt_axis) != 0) Coor.UltimateLaunch();
            Coor.abilityCtrl.getTargetObject(targetV2);

            Coor.FastFalling((InputValue & boolInput.down) != 0);
            Coor.Move(MoveZ);
            #endregion
        
		_lastInputValue = InputValue;
    }

	public float Movement ( float LJH){
		if (Mathf.Abs(LJH) > LeftJoystickHOffset)
		{
			return LJH;
		}
		else
		{
			return 0;
		}
	}
	
}
