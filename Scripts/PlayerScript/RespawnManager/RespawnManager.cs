using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RespawnManager : MonoBehaviour {
    public bool doDebug = true;
    public int debugnbTest = 3;

    public GameObject respawnPrefab;
    public Vector3 respawnPos;
    public float minSpaceRespawner;
    public int maxSimultaneousRespawner = 4;
    public float respawnerDuration = 5.0f;

    private Dictionary<int,Respawner> _respawnerDictionary = new Dictionary<int, Respawner>();
    private Dictionary<int,IEnumerator> _rDestroyRef = new Dictionary<int, IEnumerator>();
    private Vector3 _spawnerColliderSize;
    private int _initCount = 0;

	// Use this for initialization
	void Start () {
	    if(respawnPrefab)
        {
            _spawnerColliderSize = Vector3.Scale(respawnPrefab.transform.localScale, respawnPrefab.GetComponent<BoxCollider>().size);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(respawnPrefab)
            if(Input.GetKeyDown(KeyCode.R))
                instRespawner();
	}

    void instRespawner()
    {
        if(respawnPrefab)
        {
            Vector3 _spawnPos = respawnPos;

            if(_respawnerDictionary.Count >= maxSimultaneousRespawner)
            {
                Vector3 _tempDest = _respawnerDictionary[_initCount - maxSimultaneousRespawner].EndRespawner().transform.localPosition;
                _respawnerDictionary.Remove(_initCount - maxSimultaneousRespawner);
                StopCoroutine(_rDestroyRef[_initCount - maxSimultaneousRespawner]);
                _rDestroyRef.Remove(_initCount - maxSimultaneousRespawner);

                _spawnPos = _respawnerDictionary[_initCount - maxSimultaneousRespawner + 2].transform.localPosition;
                _respawnerDictionary[_initCount - maxSimultaneousRespawner + 2].MoveToLocation(_tempDest);

            }else
            {

                _spawnPos = isEvenNumber(_respawnerDictionary.Count) ? respawnPos : newPos(_initCount, respawnPos, _spawnerColliderSize, minSpaceRespawner);

                string debugString = "";

                foreach(KeyValuePair<int, Respawner> item in _respawnerDictionary)
                {
                    debugString += "spawnig pos Refresh | Key : " + item.Key + "  ||  ";
                    item.Value.MoveToLocation(newPos(item.Key, item.Value.transform.localPosition, _spawnerColliderSize, minSpaceRespawner));
                }
                Debug.Log(debugString);
            }

            GameObject _tempRespawner = Instantiate(respawnPrefab,_spawnPos, Quaternion.identity) as GameObject;
            _tempRespawner.transform.parent = transform;
            _respawnerDictionary.Add(_initCount, _tempRespawner.GetComponent<Respawner>());
            IEnumerator _tempR = _rTimingDestroy(respawnerDuration, _initCount);
            StartCoroutine(_tempR);
            _rDestroyRef.Add(_initCount, _tempR);
            _initCount++;
        }
    }

    Vector3 newPos(int posId, Vector3 initPos, Vector3 objSize, float spacing)
    {
        Vector3 _retV = initPos;
        int sideMove = isEvenNumber(posId) ? -1 : 1;
        _retV.z += sideMove * ((objSize.z+ spacing) / 2);
        return _retV;

    }

    bool isEvenNumber(int nb)
    {
        return (nb % 2) == 0;
    }

    IEnumerator _rTimingDestroy(float duration, int key)
    {
        yield return new WaitForSeconds(duration);
        _respawnerDictionary[key].EndRespawner();
        _respawnerDictionary.Remove(key);
        foreach(KeyValuePair<int,Respawner> item in _respawnerDictionary)
        {
            Debug.Log("key : " + item.Key);
            item.Value.MoveToLocation(newPos(key, item.Value.transform.localPosition, _spawnerColliderSize, minSpaceRespawner));
        }
    }

    
    void OnDrawGizmos()
    {
        if(respawnPrefab && doDebug)
        {
            Gizmos.color = new Color(0,1f,.3f,.7f);
            Vector3 _gizmosEndSize = Vector3.Scale(respawnPrefab.transform.localScale, respawnPrefab.GetComponent<BoxCollider>().size);
            _gizmosEndSize.z = debugnbTest * _gizmosEndSize.z + (debugnbTest - 1) * minSpaceRespawner;
            //_tempZInitLocation -= (debugnbTest * _gizmosEndSize.z + (debugnbTest - 1) * minSpaceRespawner) / 2;
            Gizmos.DrawWireCube(respawnPos, _gizmosEndSize);



            //for(int i = 0; i <= debugnbTest-1; i++)
            //{
            //    float _finalZValue = _tempZInitLocation + i * (_gizmosEndSize.z + minSpaceRespawner) ;
            //    Vector3 _tempSpawnPos = respawnPos;
            //    _tempSpawnPos.z = _finalZValue;
            //    Gizmos.DrawCube(_tempSpawnPos,_gizmosEndSize);
            //}
        }
        
    }
}
