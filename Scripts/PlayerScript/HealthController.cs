using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HealthController : NetworkBehaviour {

    //External link
    public Coordinator coor;

    //Health Datas
    public int charMaxHealthPoints = 100;
    public int charMaxLifeStock = 3;

    private int currentHealthPoints=100;
    private int currentLifeStock=3;

    //Repawn behavior
    public float outTime = 3.5f;

    public HealthHUD playerHealtHud;

    GameObject HUDposPlayer;
    public GameObject Dead;

    [SyncVar]
    bool active = true;

    NetManager NetManager;
    CharacterController charctrl;

    #region hudLink(s)
    //
    //
    //
    //
    //
    #endregion

    // Use this for initialization
    void Start () {
        coor = coor ? coor : GetComponent<Coordinator>();
        currentHealthPoints = charMaxHealthPoints;
        currentLifeStock = charMaxLifeStock;
        charctrl = GetComponent<CharacterController>();
        NetManager = GameObject.Find("NetworkingManager").GetComponent<NetManager>();

        if (isLocalPlayer)
        {
            HUDposPlayer = GameObject.Find("PosHUDPlayer");
            NetManager.client.Send(Message.SpawnPrefab, new SpawnPrefabMessage(HUDposPlayer.transform.position));
            StartCoroutine(FindHUD());
        }
	}
	
    IEnumerator FindHUD()
    {
        GameObject[] PlayersHud = GameObject.FindGameObjectsWithTag("HUD");
        while (!ReplaceObj.AlreadyAuthority)
        {
            yield return new WaitForSeconds(0.3f);
        }

        playerHealtHud = GameObject.FindGameObjectWithTag("MyHud").GetComponent<HealthHUD>();
        playerHealtHud.InitHUDValue(charMaxHealthPoints, charMaxLifeStock, outTime);
        
        yield return null;
        StopCoroutine(FindHUD());
    }


	// Update is called once per frame
	void Update () {
        charctrl.enabled = active;
	}

    bool isHudSet()
    {
        return true;
    }


    public void TakeDamages(NetworkInstanceId idSource, int dmgAmount)
    {
        
        currentHealthPoints -= Mathf.Abs(dmgAmount);
        if(playerHealtHud)
            playerHealtHud.TakeDamage(dmgAmount);
        if (DoDie())
            StartCoroutine(_rRespawn());
            
    }

    bool DoDie()
    {
        return currentHealthPoints <= 0;
    }

    void Die(int _KillerId)
    {
        currentLifeStock--;
        currentHealthPoints = charMaxHealthPoints;
        if (isLocalPlayer)
        {
            
            NetManager.client.Send(Message.Die, new DieMessage(netId, 0, currentLifeStock));
            if (currentLifeStock <= 0)
                ClientScene.RemovePlayer(0);
            //Instantiate(Dead, Vector3.zero, Quaternion.identity);
        }



    }

    bool CanRespawn()
    {
        return false;
    }

    [Command]
    void Cmd_Respawn(bool _value)
    {
        active = _value;
    }

    IEnumerator _rRespawn()
    {
        if (isLocalPlayer)
            Cmd_Respawn(false);
        yield return new WaitForSeconds(outTime);
        if (isLocalPlayer)
            Cmd_Respawn(true);
        Die(0);
    }


}
