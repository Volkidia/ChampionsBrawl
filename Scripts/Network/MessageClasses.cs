using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DieMessage : MessageBase
{
    public NetworkInstanceId ConnId;
    public int KillerId;
    public bool LastLife = false;

    public DieMessage()
    {
    }
    public DieMessage(NetworkInstanceId _conn, int _Killer, int _Life)
    {
        ConnId = _conn;
        KillerId = _Killer;
        if (_Life == 0) LastLife = true;
    }

    private static NetworkInstanceId GetNetId(GameObject obj)
    {
        // NOTE: no error handling
        return obj.GetComponent<NetworkIdentity>().netId;
    }
}


public class SpawnPrefabMessage : MessageBase
{
    public Vector3 Position;
   public SpawnPrefabMessage()
    {
    }
    public SpawnPrefabMessage(Vector3 _SpawnPos)
    {
        Position = _SpawnPos;
    }
}

public class SetScoreMessage : MessageBase
{
    public int order;
    public int NbKill;
    public int NbDead;
    public int Nemesis;
    public SetScoreMessage()
    {

    }

    public SetScoreMessage (int _Order, int _Nbkill, int _NbDead, int _Nemesis)
    {
        order = _Order;
        NbKill = _Nbkill;
        NbDead = _NbDead;
        Nemesis = _Nemesis;

    }
}
