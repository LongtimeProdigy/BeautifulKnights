using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketBase
{
    public string packet;
}

// 로비
[System.Serializable]
public class RequestLogin : PacketBase
{
    public string uuid;
}

[System.Serializable]
public class RequestCharInfo : PacketBase
{
    public int charId;
}

[System.Serializable]
public class RequestMatch : PacketBase
{
    public int id;
    public int deckId;
    public int roomId;
}

[System.Serializable]
public class RequestMatchCancel : PacketBase
{
    public int id;
}

// 인게임
[System.Serializable]
public class RequestEnterRoom : PacketBase
{
    public int id;
    public int roomId;
    public int deckId;
    public string uuid;
}

[System.Serializable] 
public class RequestMaintainQuit : PacketBase
{
    public int lanceid;
    public int lanceskillid;
    public int skillid;
    public int itemid;
    public int debuffid;
}

[System.Serializable] 
public class RequestAming : PacketBase
{
    public Vector3 aimdirection;
}

[System.Serializable]
public class RequestBeforeAttack : PacketBase
{
    public int Id;
    public int roomId;
    public int charId;
    public int lanceId;

    public bool ispadpressed;
}

[System.Serializable] 
public class RequestAttack : PacketBase
{
    public int armorid;
    public int armorlevel;
}

[System.Serializable] 
public class RequestPhaseChange : PacketBase
{
    public int currentphase;
}

[System.Serializable]
public class PingTestRequest : PacketBase
{
    
}

/* Request Battle Packet - by Daru */
[System.Serializable]
public class RequestBattle : PacketBase
{
    /* DESC :> Packet name : battle
     * User unique ID
     * Attack damage
     * Attack armor part
     * User use attacking skill
     * User use item
     * User use lance skill
     */
    public int userID;

    public float damage;
    public string attackedArmorPart;

    public int useSkill;
    public int useItem;
    public int useLanceSkill;
}