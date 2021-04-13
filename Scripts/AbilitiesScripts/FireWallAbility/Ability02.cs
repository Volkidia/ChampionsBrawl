using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Ability02 : _AbilityParentClass {

    //Initialisation parameters----------------------------
    public string abName = "MurDeFeuP10";
    public float maxCooldown = 10.0f;
    public int nbActivation = 1;
    public float CastingTime = 0.5f;
    [Space(5)]
    public float[] WaitingTimes;

    //Ability Effect utilities------------------------------

    [Space(15)]
    [Header("Abilities Utilities", order = 1)]
    [Space(10)]
    [Header("Wall parameters", order = 2)]
    public float WallSpeed = 0;
    public float wallDuration = 0;
    public float firewallSpawDist = 1.5f;

    public Transform wallTransform;
    public GameObject FireWall = null;
    private GameObject wallInstance = null;

    private IEnumerator _rDestroyWall_routineRef = null;

    [Header("Damages parameters", order = 3)]
    public int DotDamages = 0;
    public float DotDuration = 0;

    [Header("Explosion parameters", order = 4)]
    public GameObject explosionGO = null;
    public float explosionDuration = 0;
    public int explosionDamages = 0;
    public HitInfos exploScript;
    private GameObject exploInstance = null;
    private int increaseDamagesFromProjectile = 0;
    private Vector3 originalScale;
    private Vector3 destinationScale;

    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
    }

    // Use this for initialization
    void Start()
    {
        if(!exploScript && explosionGO)
        {
            exploScript = explosionGO.GetComponent<HitInfos>();
        }
        if(exploScript)
        {
            exploScript.damages = explosionDamages;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        switch (actualsActivations)
        {
            case 0:
                #region
                Cmd_FireWall(JoystickInput, NetIDSource);
                StartCoroutine(_rExplosionAppear(wallDuration - 0.05f));
                #endregion
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Dot();       
    }

    private void Dot()
    {
        
    }

    [Command]
    void Cmd_FireWall(Vector3 JoystickInput, NetworkInstanceId _idsource)
    {
        wallInstance = Instantiate(FireWall, transform.position + JoystickInput * firewallSpawDist, transform.rotation) as GameObject;
        wallInstance.transform.forward = JoystickInput.normalized;
        wallInstance.GetComponent<Rigidbody>().velocity = WallSpeed * JoystickInput.normalized;

        NetworkServer.SpawnWithClientAuthority(wallInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(wallInstance, wallDuration);
    }

    IEnumerator _rExplosionAppear(float duration)
    {
        yield return new WaitForSeconds(duration);
        Cmd_Explosion(NetIDSource);
    }

    [Command]
    void Cmd_Explosion(NetworkInstanceId _idsource)
    {
        exploInstance = Instantiate(explosionGO, wallInstance.transform.position, wallInstance.transform.rotation) as GameObject;
        NetworkServer.SpawnWithClientAuthority(exploInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(exploInstance, explosionDuration);
    }


    void OnDrawGizmos()
     {
         if (doDebug)
         {
             /*
             Vector3[] _directions =
             {
             new Vector3(0,1,0),
             new Vector3(0,-1,0),
             new Vector3(0,0,1),
             new Vector3(0,0,-1),
             new Vector3(0,.5f,.5f),
             new Vector3(0,.5f,-.5f),
             new Vector3(0,-.5f,.5f),
             new Vector3(0,-.5f,-.5f)
             };
             */

            if(FireWall)
            {
                Gizmos.color = new Color(1, 0, 0, .2f);

                Vector3 FireWallprefabScale = FireWall.transform.localScale;
                Gizmos.DrawCube(transform.position + transform.forward * firewallSpawDist, FireWallprefabScale);

                Gizmos.color = new Color(1, 0, 0, .5f);
                Gizmos.DrawCube(transform.position + (transform.forward * (WallSpeed * wallDuration)), FireWallprefabScale);
            }

            if(explosionGO)
            {
                Gizmos.color = Color.blue;

                float spherRad = explosionGO.GetComponent<SphereCollider>().radius;
                Vector3 exploScale = explosionGO.transform.localScale;
                float radMod = Mathf.Max(new float[] { exploScale.x, exploScale.y, exploScale.z });

                Gizmos.DrawWireSphere(transform.position + (transform.forward * (WallSpeed * wallDuration)), spherRad * radMod);
            }
          }
     }
}
