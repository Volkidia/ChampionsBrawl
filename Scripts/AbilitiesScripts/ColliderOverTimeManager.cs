using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ColliderOverTimeManager : NetworkBehaviour {

    public Vector3 ModifScale;
    public float ScaleDuration;
	// Use this for initialization
	void Start () {

        StartCoroutine(_rScaleCollider(ScaleDuration));
    }

    IEnumerator _rScaleCollider(float duration)
    {
        float currentTime = 0.0f;
        do
        {
            transform.localScale = Vector3.Lerp(transform.localScale, ModifScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);
    }
}
