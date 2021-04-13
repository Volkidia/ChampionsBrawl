using UnityEngine;
using System.Collections;

public class CameraTestScript : MonoBehaviour {

    public Transform playerTransform;
    public string playerTransformName = "";
    public Vector3 startPos = new Vector3(15, 0, 0);
    public float rotSpeed = 3;
    public float zoomSpeed = 3;


    private bool isUsed = false;
    private enum controlsState
    {
        upArrow = (1 << 0),
        downArrow = (1 << 1),
        rightArrow = (1 << 2),
        leftArrow = (1 << 3),
        leftShiftKey = (1 << 4),
        leftCtrl = (1 << 5),
    }

    private controlsState _cState = 0;

	// Use this for initialization
	void Start () {
        if(playerTransform)
        {
            transform.position = playerTransform.position + startPos;
            transform.LookAt(playerTransform);
        }
        else
            StartCoroutine(_rFindObjectInScene(playerTransformName, 3.0f));
    }
	
	// Update is called once per frame
	void Update () {
	    if(playerTransform)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
                setState(controlsState.upArrow);
            if(Input.GetKeyUp(KeyCode.UpArrow))
                unsetState(controlsState.upArrow);

            if(Input.GetKeyDown(KeyCode.DownArrow))
                setState(controlsState.downArrow);
            if(Input.GetKeyUp(KeyCode.DownArrow))
                unsetState(controlsState.downArrow);

            if(Input.GetKeyDown(KeyCode.LeftArrow))
                setState(controlsState.leftArrow);
            if(Input.GetKeyUp(KeyCode.LeftArrow))
                unsetState(controlsState.leftArrow);

            if(Input.GetKeyDown(KeyCode.RightArrow))
                setState(controlsState.rightArrow);
            if(Input.GetKeyUp(KeyCode.RightArrow))
                unsetState(controlsState.rightArrow);

            if(Input.GetKeyDown(KeyCode.LeftShift))
                setState(controlsState.leftShiftKey);
            if(Input.GetKeyUp(KeyCode.LeftShift))
                unsetState(controlsState.leftShiftKey);

            if(Input.GetKeyDown(KeyCode.LeftControl))
                setState(controlsState.leftCtrl);
            if(Input.GetKeyUp(KeyCode.LeftControl))
                unsetState(controlsState.leftCtrl);

            if(Input.GetKeyDown(KeyCode.Space))
            {
                transform.position = playerTransform.position + startPos;
                transform.LookAt(playerTransform);
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                StartCoroutine(_rFindObjectInScene(playerTransformName, 0));
            }
           applyMotion();
        }
	}
    
    private void applyMotion()
    {
        int _Zside = 0;
        int _Hside = 0;
        int _Vside = 0;


        if(testState(controlsState.leftShiftKey))
        {
            if(testState(controlsState.downArrow))
                _Zside -= 1;
            if(testState(controlsState.upArrow))
                _Zside += 1;
        }else
        {
            if(testState(controlsState.leftArrow))
                _Hside -= 1;
            if(testState(controlsState.rightArrow))
                _Hside += 1;
            if(testState(controlsState.downArrow))
                _Vside -= 1;
            if(testState(controlsState.upArrow))
                _Vside += 1;
        }

        if(testState(controlsState.leftCtrl))
            gameObject.transform.LookAt(playerTransform);

        transform.Translate((Vector3.right * _Hside * zoomSpeed) * Time.deltaTime);
        transform.Translate((Vector3.up * _Vside * zoomSpeed) * Time.deltaTime);
        transform.Translate((Vector3.forward * _Zside * zoomSpeed) * Time.deltaTime);
    }
        

    private void setState(controlsState s)
    {
        _cState |= s;
    }

    private void unsetState(controlsState s)
    {
        _cState &= ~s;
    }

    private bool testState(controlsState s)
    {
        return (_cState & s) != 0;
    }

    IEnumerator _rFindObjectInScene(string tag, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerTransform = GameObject.FindWithTag(tag).transform;
        if(playerTransform)
        {
            transform.position = playerTransform.position + startPos;
            transform.LookAt(playerTransform);
        }            
    }

}
