using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AbilityElectricWall : _AbilityParentClass
{

    //Initialisation parameters----------------------------
    public string abName = "MurElectriqueP5";
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
    public float wallDuration = 0;
    public float ElectricWallSpawDist = 1.5f;

    public GameObject ElectricWallGO = null;

    //private IEnumerator _rDestroyWall_routineRef = null;

    [Header("Damages parameters", order = 3)]
    public int Damages = 0;
    public float StunDuration = 0;
    public HitInfos ElectricWallScript;

    public override void AbilityInit(int playerId)
    {
        _abilityInitialisation(abName, playerId, maxCooldown, nbActivation, CastingTime, WaitingTimes);
    }

    // Use this for initialization
    void Start()
    {
        if (!ElectricWallScript && ElectricWallGO)
        {
            ElectricWallScript = ElectricWallGO.GetComponent<HitInfos>();
        }
        if (ElectricWallScript)
        {
            ElectricWallScript.damages = Damages;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void abilityEffect(int actualsActivations, Vector3 JoystickInput)
    {
        base.abilityEffect(actualsActivations, JoystickInput);

        Debug.Log(NetIDSource);
        switch (actualsActivations)
        {
            case 0:
                #region
                Cmd_ElectricWall(JoystickInput, NetIDSource);
                #endregion
                break;
            default:
                break;
        }
    }

    [Command]
    void Cmd_ElectricWall(Vector3 JoystickInput, NetworkInstanceId _idsource)
    {
        GameObject wallInstance = Instantiate(ElectricWallGO, transform.position + JoystickInput * ElectricWallSpawDist, transform.rotation) as GameObject;
        wallInstance.transform.forward = JoystickInput.normalized;
        NetworkServer.SpawnWithClientAuthority(wallInstance, NetworkServer.FindLocalObject(_idsource));
        Destroy(wallInstance, wallDuration);
    }
}
