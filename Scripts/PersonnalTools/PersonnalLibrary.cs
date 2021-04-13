using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;

#region header (BARRELET Alexandre - 2016 - C# & Unity 3D)
/*##############################################################
*                      BARRELET Alexandre                      *
*                             2016                             *
----------------------------------------------------------------
*                          Toolbox3D                           *
*                       ComponentGesture                       *
*                         ObjectGesture                        *
*                          PhysicsBox                          *
*                        StateMachineBox                       *
*                           DebugBox                           *
----------------------------------------------------------------
*                         C# - Unity 3D                        *
###############################################################*/
#endregion

//3D Operations 
public class ToolBox3D
{
    /// <summary>
    /// Get the Normalized Vector of the direction between two coordinates
    /// </summary>
    /// <param name="origin">"From" coordinates</param>
    /// <param name="endPos">"To" coordinates</param>
    /// <returns>a direction Vector between two point with a magnitude of 1</returns>
    public static Vector3 getPointToPointDir(Vector3 origin, Vector3 endPos)
    {
        return (endPos - origin).normalized;
    }

    public static Vector3 getPointToPointDir(Vector3 origin, Vector3 endPos, Vector3 unlockedAxis)
    {
        unlockedAxis = set1VectorV(unlockedAxis);
        origin = multiplyVector(origin, unlockedAxis);
        endPos = multiplyVector(endPos, unlockedAxis);
        return getPointToPointDir(origin, endPos);
    }

    public static float getGreaterDist(Vector3 comparePoint, Vector3 point1, Vector3 point2)
    {
        float _distC2p1 = Vector3.Distance(comparePoint,point1);
        float _distC2p2 = Vector3.Distance(comparePoint,point2);

        return MathBox.getGreater(_distC2p1, _distC2p2);
    }

    public static float getGreaterDist(Vector3 comparePoint, Vector3 point1, Vector3 point2, out Vector3 dir)
    {
        float _distC2p1 = Vector3.Distance(comparePoint,point1);
        float _distC2p2 = Vector3.Distance(comparePoint,point2);
        float retV = MathBox.getGreater(_distC2p1, _distC2p2);

        dir = _distC2p1 == retV ? getPointToPointDir(comparePoint, point1) : getPointToPointDir(comparePoint, point2);

        return retV;
    }

    public static float getAngle(Vector3 origin, Vector3 fromPoint, Vector3 toPoint)
    {
        Vector3 _tmpDir1 = getPointToPointDir(origin,fromPoint);
        Vector3 _tmpDir2 = getPointToPointDir(origin,toPoint);
        return Vector3.Angle(_tmpDir1, _tmpDir2);
    }

    public static float getAngle(Vector3 origin, Vector3 fromPoint, Vector3 toPoint, Vector3 unlockedAxis)
    {
        unlockedAxis = set1VectorV(unlockedAxis.normalized);
        origin = multiplyVector(origin, unlockedAxis);
        fromPoint = multiplyVector(fromPoint, unlockedAxis);
        toPoint = multiplyVector(toPoint, unlockedAxis);

        Vector3 _tmpDir1 = getPointToPointDir(origin,fromPoint);
        Vector3 _tmpDir2 = getPointToPointDir(origin,toPoint);
        return Vector3.Angle(_tmpDir1, _tmpDir2);
    }

    public static Vector3 set1VectorV(Vector3 v)
    {
        if(v.x != 0)
            v.x = Mathf.Sign(v.x);
        if(v.y != 0)
            v.y = Mathf.Sign(v.y);
        if(v.z != 0)
            v.z = Mathf.Sign(v.z);

        return v;
    }

    public static Vector3 multiplyVector(Vector3 v1, Vector3 v2)
    {
        Vector3 _tmpV = v1;
        _tmpV.x *= v2.x;
        _tmpV.y *= v2.y;
        _tmpV.z *= v2.z;
        return _tmpV;
    }

    /// <summary>
    /// Get Vector of Lerp Like Movement (usefull for for Character Controller)
    /// </summary>
    /// <param name="originPos">Start Position</param>
    /// <param name="endPos">End Position</param>
    /// <param name="startTime">Start Time</param>
    /// <param name="duration">Wanted Duration</param>
    /// <returns>a movement vector in the wanted direction with the corresponding speed</returns>
    public static Vector3 CharacterCtrl_LerpLikeMove(Vector3 originPos, Vector3 endPos, float startTime, float duration)
    {
        Vector3 _motionDir = (endPos - originPos).normalized;
        float _motionMove = Vector3.Distance(originPos,endPos) / duration;
        Vector3 _retV3 = _motionDir * _motionMove;

        return _retV3;
    }

    /// <summary>
    /// Simplified Lerp use
    /// </summary>
    /// <param name="originPos">Start Point</param>
    /// <param name="endPos">End Point</param>
    /// <param name="startTime">Start Time</param>
    /// <param name="duration">Movement Duration</param>
    /// <returns>The current Movement Vector</returns>
    public static Vector3 lerpPosDuration(Vector3 originPos, Vector3 endPos, float startTime, float duration)
    {
        Vector3 retV3 = Vector3.zero;

        if(Time.time > startTime + duration)
        {
            float _totDist = Vector3.Distance(originPos, endPos);
            float _speed = _totDist / duration;
            float _distCovered = (Time.time - startTime) * _speed;
            float _fracJourney = _distCovered / _totDist;
            retV3 = Vector3.Lerp(originPos, endPos, _fracJourney);
        }
        return retV3;
    }
    
    /// <summary>
    /// Simplified Raycast use
    /// </summary>
    /// <param name="origin">Start Point</param>
    /// <param name="endPoint">End Point</param>
    /// <param name="lm">Raycast LayerMask</param>
    /// <returns>The RaycastHit returned</returns>
    public static RaycastHit simpleRaycast(Vector3 origin, Vector3 endPoint, LayerMask lm)
    {

        RaycastHit _retRCH = new RaycastHit();
        Vector3 _dir = (endPoint - origin).normalized;
        float _dist = Vector3.Distance(origin, endPoint);
        Ray _r = new Ray(origin,_dir);
        Physics.Raycast(_r, out _retRCH, _dist, lm);
        return _retRCH;
    }

    /// <summary>
    /// Simplified Raycast use
    /// </summary>
    /// <param name="origin">Start Point</param>
    /// <param name="endPoint">End Point</param>
    /// <param name="lm">Raycast LayerMask</param>
    /// <param name="objectLayer">(out) the hot Object Layer</param>
    /// <returns>The RaycastHit returned</returns>
    public static RaycastHit simpleRaycast(Vector3 origin, Vector3 endPoint, LayerMask lm, out int objectLayer)
    {
        objectLayer = 0;
        RaycastHit _retRCH = simpleRaycast(origin,endPoint,lm);
        if(_retRCH.collider)
            objectLayer = _retRCH.collider.gameObject.layer;
        return _retRCH;
    }
}

public class ToolBox2D
{
    public static Vector2 Set1Vector(Vector2 v)
    {
        if(v.x != 0)
            v.x = Mathf.Sign(v.x);
        if(v.y != 0)
            v.y = Mathf.Sign(v.y);
        return v;
    }
}

public class MathBox
{
    public static float getGreater(float v1, float v2)
    {
        return v1 > v2 ? v1 : v2;
    }

    public static float getGreater(float[] values)
    {
        float _retF = float.MinValue;
        foreach(float _f in values)
        {
            _retF = _f > _retF ? _f : _retF;
        }
        return _retF;
    }
}

// Component & GameObject Referencing -------------------------
#region Component & GameObject Referencing
public class ComponentGesture
{
    DataTable _dTable;
    uint _currId = 1;

    public ComponentGesture()
    {
        _dTable = new DataTable("ComponentTable");
        _dTable.Columns.Add("id", typeof(uint));
        _dTable.Columns.Add("idGO", typeof(uint));
        _dTable.Columns.Add("gameObject", typeof(GameObject));
        _dTable.Columns.Add("componentType", typeof(System.Type));
        _dTable.Columns.Add("component", typeof(Component));
        _dTable.Columns.Add("addedTime", typeof(float));

        UniqueConstraint _const = new UniqueConstraint(new DataColumn[] {_dTable.Columns["id"]});
        _dTable.Constraints.Add(_const);
    }

    public void addComponent(uint idObj, GameObject go, Component co)
    {
        _dTable.Rows.Add(_currId,idObj, go, co.GetType(), co, Time.time);
        _currId++;
    }
}

public class ObjectGesture
{
    DataTable _dTable;

    uint _currId = 1;

    public ObjectGesture()
    {
        _dTable = new DataTable("ChildTable");
        _dTable.Columns.Add("id", typeof(uint));
        _dTable.Columns.Add("addedTime", typeof(float));
        _dTable.Columns.Add("childObject", typeof(GameObject));
        _dTable.Columns.Add("name", typeof(string));
        UniqueConstraint _uniqueConstraints =
            new UniqueConstraint(new DataColumn[]{_dTable.Columns["id"],_dTable.Columns["childObject"]});
        _dTable.Constraints.Add(_uniqueConstraints);
    }

    private GameObject[] _GetGameObjectsFromRows(DataRow[] dRows)
    {
        List<GameObject> _goList = new List<GameObject>();
        for(int i = 0; i < dRows.Length; i++)
        {
            _goList.Add(_GetGameObjectFromRow(dRows[i]));
        }

        return _goList.ToArray();
    }

    private GameObject _GetGameObjectFromRow(DataRow dRow)
    {
        return dRow[3] as GameObject;
    }
    
    private void _removeRows(DataRow[] dRows)
    {
        for(int i = 0; i < dRows.Length; i++)
        {
            _dTable.Rows.Remove(dRows[i]);
        }
    }
    
    public bool AddGameObject(GameObject go)
    {
        bool _retB = false;
        if(!GetGameObject(go))
        {
            _dTable.Rows.Add(_currId, Time.time, go, go.name);
            _retB = true;
            _currId++;
        }
        return _retB;
    }

    public bool AddGameObject(GameObject go, out uint id)
    {
        id = 0;
        bool _retB = false;
        if(!GetGameObject(go))
        {
            _dTable.Rows.Add(_currId, Time.time, go, go.name);
            _retB = true;
            id = _currId;
            _currId++;
        }
        return _retB;
    }

    #region GetGameObject (unique - 1st)
    public GameObject GetGameObject(GameObject go)
    {
        DataRow _dRow = _dTable.Select("childObject = " + go)[0];
        return _GetGameObjectFromRow(_dRow);
    }

    public GameObject GetGameObject(uint id)
    {
        DataRow _dRow = _dTable.Select("id = " + id)[0];
        return _GetGameObjectFromRow(_dRow);
    }

    public GameObject GetGameObject(string name)
    {
        DataRow _dRow = _dTable.Select("name = " + name)[0];
        return _GetGameObjectFromRow(_dRow);
    }

    public GameObject GetGameObject(float addedTime)
    {
        DataRow _dRow = _dTable.Select("addedTime " + addedTime)[0];
        return _GetGameObjectFromRow(_dRow);
    }
    #endregion

    #region GetGameObjects (multiple)
    public GameObject[] GetGameObjects(string name)
    {
        DataRow[] _dRows = _dTable.Select("name = " + name);
        return _GetGameObjectsFromRows(_dRows);
    }

    public GameObject[] GetGameObjects(float addedTime)
    {
        DataRow[] _dRows = _dTable.Select("addedTime = " + addedTime);
        return _GetGameObjectsFromRows(_dRows);
    }

    public GameObject[] GetGameObject(string name, float addedTime)
    {
        DataRow[] _dRows = _dTable.Select("addedTime = " + addedTime + "AND name = " + name);
        return _GetGameObjectsFromRows(_dRows);
    }
    #endregion

    #region RemoveGameObject (unique - 1st)
    public GameObject RemoveGameObject(GameObject go)
    {
        DataRow _dRow =  _dTable.Select("childObject = " + go)[0];
        GameObject _retGo = _GetGameObjectFromRow(_dRow);
        _dTable.Rows.Remove(_dRow);
        return _retGo;
    }

    public GameObject RemoveGameObject(uint id)
    {
        DataRow _dRow = _dTable.Select("id = " + id)[0];
        GameObject _retGo = _GetGameObjectFromRow(_dRow);
        _dTable.Rows.Remove(_dRow);
        return _retGo;
    }

    public GameObject RemoveGameObject(float addedTime)
    {
        DataRow _dRow = _dTable.Select("addedTime = " + addedTime)[0];
        GameObject _retGo = _GetGameObjectFromRow(_dRow);
        _dTable.Rows.Remove(_dRow);
        return _retGo;
    }

    public GameObject RemoveGameObject(string name)
    {
        DataRow _dRow = _dTable.Select("name = " + name)[0];
        GameObject _retGo = _GetGameObjectFromRow(_dRow);
        _dTable.Rows.Remove(_dRow);
        return _retGo;
    }
    #endregion

    #region RemoveGameObjects (multiple)
    public GameObject[] RemoveGameObjects(float addedTime)
    {
        DataRow[] _dRows = _dTable.Select("addedTime = " + addedTime);
        GameObject[] _retGos = _GetGameObjectsFromRows(_dRows);
        _removeRows(_dRows);
        return _retGos;
    }

    public GameObject[] RemoveGameObjects(string name)
    {
        DataRow[] _dRows = _dTable.Select("name = " + name);
        GameObject[] _retGos = _GetGameObjectsFromRows(_dRows);
        _removeRows(_dRows);
        return _retGos;
    }

    public GameObject[] RemoveGameObjects(string name, float addedTime)
    {
        DataRow[] _dRows = _dTable.Select("name = " + name + "  AND addedTime = " + addedTime);
        GameObject[] _retGos = _GetGameObjectsFromRows(_dRows);
        _removeRows(_dRows);
        return _retGos;
    }
    #endregion
}
#endregion

// Layer & Physics ---------------------------------------------
public class PhysicsBox
{
    /// <summary>
    /// Is the game object layer part of the LayerMask
    /// </summary>
    /// <param name="obj">tested obj</param>
    /// <param name="lm">LayerMask Test</param>
    /// <returns>if the given GameObject layer is contained in the given LayerMask</returns>
    public static bool isInlayerMask(GameObject obj, LayerMask lm)
    {
        int _objLayer = (1 << obj.layer);
        if((_objLayer & lm) > 1)
            return true;
        else
            return false;
    }
}

//State Machines Utilities (int, long, short)-------------------
public class StateMachineBox
{
    public class longState
    {
        long currState = 0;
        public long CurrStates {
            get { return currState; }
        }

        Dictionary<string,long> states = new Dictionary<string, long>();
        int _currDecalage = 0;


        public longState()
        {
            states = new Dictionary<string, long>();
            _currDecalage = 0;
            currState = 0;
        }

        public longState(string[] statesNames)
        {
            states = new Dictionary<string, long>();
            _currDecalage = 0;
            currState = 0;
            foreach(string _stateN in statesNames)
            {
                addState(_stateN);
            }
        }

        public bool stateIsSet(long testState)
        {
            return (currState & testState) != 0;
        }

        public bool stateIsSet(string testState)
        {
            return (currState & states[testState]) != 0;
        }

        public bool stateIsSet(int testState)
        {
            return (currState & (1 << testState)) != 0;
        }

        public long stateSet(long state)
        {
            return currState |= state;
        }

        public long stateSet(string state)
        {
            return currState |= states[state];
        }

        public long stateSet(int state)
        {
            return currState |= (long)(1 << state);
        }

        public long stateUnset(long state)
        {
            return currState &= ~state;
        }

        public long stateUnset(string state)
        {
            return currState &= ~states[state];
        }

        public long stateUnset(int state)
        {
            return currState &= ~(long)(1 << state);
        }

        public long getStateV(string stateName)
        {
            return states[stateName];
        }

        public bool addState(string stateName)
        {
            bool retV = false;
            if(!states.ContainsValue(1 << _currDecalage))
            {
                states.Add(stateName, _currDecalage);
                _currDecalage++;
                retV = true;
            }

            return retV;
        }

        public bool addState(string stateName, out long stateV)
        {
            stateV = -1;
            bool retV = false;
            if(!states.ContainsValue(1<< _currDecalage))
            {
                stateV = (1 << _currDecalage);
                states.Add(stateName, stateV);
                _currDecalage++;
                retV = true;
            }
            return retV;
        }

        public bool changeStateName(string oldName, string newName)
        {
            bool retV = false;

            if(retV = states.ContainsKey(oldName))
            {
                long _tempV = states[oldName];
                states.Remove(oldName);
                states.Add(newName, _tempV);
            }  

            return retV;
        }
    }

    // Int State Machine
    public static bool stateIsSet(int baseState, int testState)
    {
        return (baseState & testState) != 0;
    }

    public static int stateSet(int baseState, int setState)
    {
        return baseState | setState;
    }

    public static int stateUnset(int baseState, int unsetState)
    {
        return baseState & ~unsetState;
    }

    //Long State Machine
    public static bool stateIsSet(long baseState, long testState)
    {
        return (baseState & testState) != 0;
    }

    public static long stateSet(long baseState, long setState)
    {
        return baseState | setState;
    }

    public static long stateUnset(long baseState, long unsetState)
    {
        return baseState & ~unsetState;
    }

    //Short State Machine
    public static bool stateIsSet(short baseState, short testState)
    {
        return (baseState & testState) != 0;
    }

    public static short stateSet(short baseState, short setState)
    {
        return (short)(baseState | setState);
    }

    public static short stateUnset(short baseState, short unsetState)
    {
        return (short)(baseState & ~unsetState);
    }
}

// Debug Tools -------------------------------------------------
public class DebugBox
{    
    /// <summary>
    /// Create String chain to display array elements
    /// </summary>
    /// <param name="array">(int[]) Array to Display</param>
    /// <param name="separator">array Element separator</param>
    /// <param name="startId">start element id</param>
    /// <returns>string chain of all array elements</returns>
    public static string DebugArray(int[] array, string separator = "\n", string color = "blue" ,int startId = 0)
    {
        string retS = " ";
        for(int i = startId; i < array.Length; i++)
        {
            retS += "[id:" + i + " |  <color="+color+">" + array[i] + "</color>]";
            if(i < array.Length - 1)
                retS += " " + separator + " ";
        }
        return retS;
    }

    /// <summary>
    /// Create String chain to display array elements
    /// </summary>
    /// <param name="array">(string)Array to Display</param>
    /// <param name="separator">array Element separator</param>
    /// <param name="startId">start element id</param>
    /// <returns>string chain of all array elements</returns>
    public static string DebugArray(string[] array, string separator = "\n", string color = "blue", int startId = 0)
    {
        string retS = " ";
        for(int i = startId; i < array.Length; i++)
        {
            retS += "[id:" + i + " |  <color=" + color + ">" + array[i] + "</color>]";
            if(i < array.Length - 1)
                retS += " " + separator + " ";
        }
        return retS;
    }

    /// <summary>
    /// Create String chain to display array elements
    /// </summary>
    /// <param name="array">(float)Array to Display</param>
    /// <param name="separator">array Element separator</param>
    /// <param name="startId">start element id</param>
    /// <returns>string chain of all array elements</returns>
    public static string DebugArray(float[] array, string separator = "\n", string color = "blue", int startId = 0)
    {
        string retS = " ";
        for(int i = startId; i < array.Length; i++)
        {
            retS += "[id:" + i + " |  <color=" + color + ">" + array[i] + "</color>]";
            if(i < array.Length - 1)
                retS += " " + separator + " ";
        }
        return retS;
    }

}


