using UnityEngine;
using System.Collections;

public class Respawner : MonoBehaviour {

    public float moveSpeed = 3.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MoveToLocation(Vector3 destination)
    {
        StartCoroutine(_rMoveToLocation(destination));
    }

    public Respawner EndRespawner()
    {
        Destroy(this.gameObject);
        return this;
    }
    
    IEnumerator _rMoveToLocation(Vector3 destination)
    {
        float _travelLength = Vector3.Distance(transform.position, destination);
        float startTime = Time.time;
        if(_travelLength > 0)
        {
            Vector3 _startPos = transform.localPosition;

            do
            {
                float _distCovered = (Time.time - startTime) * moveSpeed;
                float _fracJourney = _distCovered / _travelLength;
                transform.localPosition = Vector3.Lerp(_startPos, destination, _fracJourney);

                yield return new WaitForEndOfFrame();
            } while(transform.localPosition != destination);
        }
        yield return null;
    }
}
