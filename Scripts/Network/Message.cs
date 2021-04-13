using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Message
{
    // used for auto-increment
    private enum Type
    {
        Die = MsgType.Highest + 1,
        SpawnPrefab = MsgType.Highest + 2,
        SetScore = MsgType.Highest +3

    }

    public const short Die = (short)Type.Die;
    public const short SpawnPrefab = (short)Type.SpawnPrefab;
    public const short SetScore = (short)Type.SetScore;
}
