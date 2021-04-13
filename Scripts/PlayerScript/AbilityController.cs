using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AbilityController : NetworkBehaviour {

    private List<_AbilityParentClass> _currentAbilityKit = new List<_AbilityParentClass>();
    public string[] PresetKits;
    int idPlayer = 1;
    public Coordinator coor;
    public goKitUse _kitUse;
    public GameObject[] targetObjects;
    private int[,] _objectRef = { { 0, 1, 2 }, { 3,-1,4}, { 5,6,7} };
    private Vector2 _lastInput = Vector2.one;
    private GameObject _targetDir = null;

    [Command]
    void Cmd_SetAbilitiesKit(int idKit)
    {
        GameObject _go = Instantiate(net_aIndex.getGoKit(idKit),transform.position,Quaternion.identity) as GameObject;
        _kitUse = _go.GetComponent<goKitUse>();
        _go.transform.parent = transform;
        NetworkServer.SpawnWithClientAuthority(_go, base.connectionToClient);
    }



	// Use this for initialization
	void Start () {
        coor = coor ? coor : GetComponent<Coordinator>();
        if(isLocalPlayer)
            Cmd_SetAbilitiesKit(StaticData.Kit);

        foreach(GameObject _go in targetObjects)
        {
            _go.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        //if(Input.GetKeyDown(KeyCode.S))
            //UseAbility(0, new Vector3(0,0.3f,.5f));

        Vector2 _inV = Vector2.zero;
        if(Input.GetKey(KeyCode.UpArrow))
            _inV.y -= 1;
        if(Input.GetKey(KeyCode.DownArrow))
            _inV.y += 1;
        if(Input.GetKey(KeyCode.LeftArrow))
            _inV.x += 1;
        if(Input.GetKey(KeyCode.RightArrow))
            _inV.x -= 1;

        //Debug.Log(_inV);
        getTargetObject(_inV);
    }

    /// <summary>
    /// Add the ability to the gameObject, and set the ListElement corresponding as a reference to the created component
    /// </summary>
    private void addAbilities()
    {
        for(int i = 0; i <= _currentAbilityKit.Count - 1; i++)
        {
            System.Type type = _currentAbilityKit[i].GetType();
            Component Copy = gameObject.AddComponent(type);

            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach(System.Reflection.FieldInfo _field in fields)
            {
                _field.SetValue(Copy, _field.GetValue(_currentAbilityKit[i]));
            }
            _currentAbilityKit[i] = Copy as _AbilityParentClass;
            _currentAbilityKit[i].AbilityInit(idPlayer);
        }
    }

    // ############## CURRENT KIT SET (SetAbilitiesKit(int idKit) + 2 Overloads)
    /*
    #region current kit Set (SetAbilitiesKit(int idKit) + 2 Overloads)
    /// <summary>
    /// Use the abilities from a previously prepared set as the current Kit
    /// </summary>
    /// <param name="idKit">the id of the preset kit to use (PresetKits)</param>
    /// <returns>the new amount of abilities in the current kit</returns>
    public float SetAbilitiesKit(int idKit)
    {
        foreach (string _ability in PresetKits)
        {
            _currentAbilityKit.Add(AbilitiesIndex.getAbiliy(_ability));
        }
        addAbilities();
        return _currentAbilityKit.Count;
    }

    /// <summary>
    /// Use the given three abilities as the current Kit
    /// </summary>
    /// <param name="ability01">name of the first ability to use in the current kit</param>
    /// <param name="ability02">name of the second ability to use in the current kit</param>
    /// <param name="ability03">name of the third ability to use in the current kit</param>
    /// <returns>the new amount of abilities in the current kit</returns>
    public float SetAbilitiesKit(string ability01, string ability02, string ability03)
    {
        _currentAbilityKit.Add(AbilitiesIndex.getAbiliy(ability01));
        _currentAbilityKit.Add(AbilitiesIndex.getAbiliy(ability02));
        _currentAbilityKit.Add(AbilitiesIndex.getAbiliy(ability03));
        addAbilities();

        return _currentAbilityKit.Count;
    }

    /// <summary>
    /// Use all the given abilities form the array as the current Kit
    /// </summary>
    /// <param name="abilitiesKit">the kit to use as the new current kit</param>
    /// <returns>the new amount of abilities in the current kit</returns>
    public float SetAbilitiesKit(string[] abilitiesKit)
    {
        foreach (string _ability in abilitiesKit)
        {
            _currentAbilityKit.Add(AbilitiesIndex.getAbiliy(_ability));
        }
        addAbilities();
        return _currentAbilityKit.Count;
    }
    #endregion
    */
    public void SetKitUseGO(goKitUse kUse)
    {
        if(kUse)
            _kitUse = kUse;
    }
    
    public void SetAbilitiesKit(int idKit)
    {
        /*
        _kitUse = _go.GetComponent<goKitUse>();

        if(_kitUse)
            return _kitUse.abArray.Length;
        else
            return 0;
        */
    }

    //############## ABILITY USE (CanUse & Use)
    #region Ability Use (CanUse & Use & castingTime)    
    /// <summary>
    /// Test if the ability with the given abilityis currently usable
    /// </summary>
    /// <param name="id">ability id</param>
    /// <returns>usability of the ability with the given id</returns>
    public bool canUseAbility(int id)
    {
        bool retV = false;
        retV = _currentAbilityKit[id];
        return retV;
    }

    /// <summary>
    /// Give the cast Time of the ability
    /// </summary>
    /// <param name="id">ability id</param>
    /// <returns>castingTime of the ability with the given id</returns>
    public float getCastTime(int id)
    {
        if(_currentAbilityKit[id])
            return _currentAbilityKit[id].castTime;
        else
            return 0.0f;
    }

    /// <summary>
    /// Use the ability with the given id if currently usable
    /// </summary>
    /// <param name="id">ability id</param>
    /// <returns>usability of the ability with the given id</returns>
    public bool UseAbility(int id)
    {
        return _kitUse && _targetDir? _kitUse.useAbility(id, netId, ToolBox3D.getPointToPointDir(transform.position,_targetDir.transform.position)) : false;        
    }
    #endregion

    //################## Targetting Ability ########################
    public GameObject getTargetObject(Vector2 input)
    {
        input = ToolBox2D.Set1Vector(input) + Vector2.one;
        GameObject _tmpDir = _targetDir;
        if(input != _lastInput)
        {
            int _idx = _objectRef[(int)input.y, (int)input.x];
            _lastInput = input;
            if(_idx >= 0)
            {
                GameObject _tmpGo = targetObjects[_idx];
                if(_tmpGo != _targetDir)
                {
                    _targetDir = _tmpGo;
                    _targetDir.SetActive(true);
                }
            }
        }

        if(_tmpDir && _tmpDir != _targetDir)
            _tmpDir.SetActive(false);

        return _targetDir;
    }

    //############## ABILITY INFOS DISPLAY (GetCD)

    /// <summary>
    /// Return the maxcooldown of all Abilities in the current kit
    /// </summary>
    /// <returns>float array : CDs of abilities in order</returns>
    public float[] abiliesCD()
    {
        float[] retV = null;
        if(_currentAbilityKit.Count > 0)
        {
            for(int i = 0; i <= _currentAbilityKit.Count -1; i++)
            {
                retV[i] = _currentAbilityKit[i].maxCD;
            }
        }
        return retV;
    }

    IEnumerator _r_addAbility()
    {
        do
        {
            yield return new WaitForSeconds(.1f);
        } while(!AbilitiesIndex.initState);

        SetAbilitiesKit(0);

        yield return null;
    }

    void OnDrawGizmosSelected()
    {
        /*
        foreach(GameObject item in targetObjects)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
        */
    }
}
