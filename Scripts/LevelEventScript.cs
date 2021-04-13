using UnityEngine;
using System.Collections;

public class LevelEventScript : MonoBehaviour {

    public float[] EventsTimingTable;
    public EventObjects[] EventObjectsTable;

    private int noEvent = 0;
    private float timingEvent = -1f;

	// Use this for initialization
	void Start ()
    {
        timingEvent = Time.time + EventsTimingTable[0];
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(timingEvent <= Time.time && timingEvent > 0)
        {
            GlobalEvents();
            NextEvent();
        }
	}

    public void GlobalEvents()
    {
        foreach (EventObjects objects in EventObjectsTable)
        {
            objects.Event(noEvent);
        }
    }

    private void NextEvent()
    {
        noEvent++;
        if (noEvent <= EventsTimingTable.Length - 1)
        {
            timingEvent = Time.time + EventsTimingTable[noEvent];

        }
        else timingEvent = -1f;

    }
}
