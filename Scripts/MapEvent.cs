using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapEvent : MonoBehaviour {

    GameObject[] LightsObj;
    float InclinaisonValue;
    int RotateDirValue ;
    int SpeedRotPlanete;

    List<Light> Lights = new List<Light>();
    Dictionary<Light, float> MaxValueLight = new Dictionary<Light, float>();
    public GameObject PlanetEclipse;
    public GameObject[] Planets;
    public GameObject Background;
    public Transform DestinationBackground;
    public Transform[] RefNextAngle;

    float[] Angles;

    float lastAngle = 0;
    float DistAtoB;
    Vector3 CoordinateBackground;
    int Time;
    int index = 0;

    public int eventTime;
    public int signalTime;
    public int TimeEventSpawn;

    private Vector3 _nextSun;
    private Vector3 _nextPlanet;

    NetManager NetManager;
	// Use this for initialization
	void Start () {

        Planets = GameObject.FindGameObjectsWithTag("Planet");
        LightsObj = GameObject.FindGameObjectsWithTag("LightEvent");
        NetManager = GameObject.Find("NetworkingManager").GetComponent<NetManager>();
        Angles = new float[4];
     
        Time = NetManager.GameTime;
        foreach (GameObject obj in LightsObj)
        {
            Light _light = obj.GetComponent<Light>();
            Lights.Add(_light);
            MaxValueLight.Add(_light, _light.intensity);
        }

        iTween.MoveTo(Background, iTween.Hash("position",DestinationBackground,"time", NetManager.GameTime, "easetype","linear"));

        CoordinateBackground = Background.transform.position;
        DistAtoB = Vector3.Distance(CoordinateBackground, DestinationBackground.position);

        foreach (GameObject planet in Planets)
        {
            InclinaisonValue = Random.Range(-25, 25);
            SpeedRotPlanete = Random.Range(5, 15);
            planet.transform.parent.Rotate(new Vector3(0, 0, InclinaisonValue));
            RotateDirValue = Random.Range(-180, 180);

            iTween.RotateTo(planet.transform.parent.gameObject, iTween.Hash("y", RotateDirValue, "time", NetManager.GameTime, "easetype", "linear"));
        }

        iTween.RotateAdd(PlanetEclipse.transform.parent.gameObject, iTween.Hash("y", -GetEclipseRotvalue(RefNextAngle[0].position, TimeEventSpawn), "time", TimeEventSpawn, "easetype", "OutCirc", "oncompletetarget", this.gameObject, "oncomplete", "EclipseAwake"));
    }

    void EclipseAwake()
    {
        iTween.RotateAdd(PlanetEclipse.transform.parent.gameObject, iTween.Hash("y",- GetEclipseRotvalue(RefNextAngle[1].position, TimeEventSpawn + signalTime), "time", signalTime, "easetype", "linear","oncompletetarget", this.gameObject, "oncomplete", "Eclipse"));
        iTween.ValueTo(this.gameObject, iTween.Hash("from", 1, "to", 0, "time", signalTime, "onupdate", "LightUpdate"));
    }

    void LightUpdate(float _Value)
    {
        foreach(Light light in Lights)
        {
            light.intensity = _Value * MaxValueLight[light];
        }
    }
    void Eclipse()
    {
        iTween.RotateAdd(PlanetEclipse.transform.parent.gameObject, iTween.Hash("y", - GetEclipseRotvalue(RefNextAngle[2].position, TimeEventSpawn +signalTime+ eventTime), "time", eventTime, "easetype", "linear", "oncompletetarget", this.gameObject, "oncomplete", "EndEclipse"));
    }

    void EndEclipse()
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", 0, "to", 1, "time", signalTime, "onupdate", "LightUpdate"));
        iTween.RotateAdd(PlanetEclipse.transform.parent.gameObject, iTween.Hash("y", -GetEclipseRotvalue(RefNextAngle[3].position, TimeEventSpawn + signalTime + eventTime+signalTime), "time", signalTime, "easetype", "linear", "oncompletetarget", this.gameObject, "oncomplete", "GoFar"));
    }

    void GoFar()
    {

    }

    public float GetEclipseRotvalue(Vector3 _boatPos, float _time)
    {
        Debug.Log(DistAtoB);
        float DistFctTime = (_time * DistAtoB / NetManager.GameTime);
        Vector3 NextPosSun = new Vector3 (CoordinateBackground.x,CoordinateBackground.y,CoordinateBackground.z+DistFctTime);
        
        Debug.Log(NextPosSun);
        _nextSun = NextPosSun;
        Vector3 dir = _boatPos- NextPosSun ;
        dir.y = 0;
        dir.Normalize();

        Vector3 dirSunToPlanet = PlanetEclipse.transform.position -Background.transform.position;
        dirSunToPlanet.y = 0;
        dirSunToPlanet.Normalize();

        float DistSunToPlanet = Vector3.Distance(Background.transform.position, PlanetEclipse.transform.position);
        Vector3 NextPosPlanet = NextPosSun + (dirSunToPlanet * DistSunToPlanet);
        _nextPlanet = NextPosPlanet;

        Vector3 Newdir = NextPosPlanet - NextPosSun;
        Newdir.y = 0;
        Newdir.Normalize();

        float Angle = Vector3.Angle(Newdir,dir );
        Vector3 V =  Vector3.Cross(Newdir, dir);
        Angle *= -Mathf.Sign(V.y);
        Debug.Log(Angle + "(x=" + V + ")" );
        return Angle;


    }
}
