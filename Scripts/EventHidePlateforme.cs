using UnityEngine;
using System.Collections;

public class EventHidePlateforme : EventObjects
{
    public GameObject EventPlatform;
    public float PlatformScaleDuration;

    // Use this for initialization
    void Start()
    {
        EventPlatform = EventPlatform ? EventPlatform : GetComponentInChildren<Transform>().gameObject;
    }

    override public void Event(int noEvent)
    {
        switch (noEvent)
        {
            case 0:
                EventPlatform.SetActive(false);
                Debug.Log(noEvent);
                break;
            case 1:
                EventPlatform.SetActive(true);
                Debug.Log(noEvent);
                break;
            case 2:
                StartCoroutine(ScalePlatformOverTime(PlatformScaleDuration));
                Debug.Log(noEvent);
                break;
            default:
                break;
        }
    }
        
    IEnumerator ScalePlatformOverTime(float time)
    {
        Vector3 startScale = EventPlatform.transform.localScale;
        Vector3 endScale = new Vector3(1f, 1f, 3f);

        float currentTime = 0.0f;
        do
        {
            EventPlatform.transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        while (currentTime <= time);
    }
}

