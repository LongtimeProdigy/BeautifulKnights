using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Network;

// 로비
[System.Serializable]
public class ResultLogin : PacketBase
{
    public int id;
    public string uuid;
    public List<int> charList;
    public List<int> lanceList;
    public List<int> armorList;

    public List<Deck> deckList;

    public CharacterDataInfo charInfo;

    public int deckId;
    public int curGold;
    public int curCash;
}

[System.Serializable]
public class ResultCharInfo : PacketBase
{
    public CharacterDataInfo info;
}

[System.Serializable]
public class ResultMatch : PacketBase
{
    public int roomId;
    public List<UserInfo> users;
}

// 인게임
[System.Serializable]
public class ResultEnterRoom : PacketBase
{
    public InGameUserData user;
}

[System.Serializable]
public class ResultEnterRoomOther : PacketBase
{
    public InGameUserData user;
}

[System.Serializable] 
public class ResultMaintainQuit : PacketBase
{
    // 플레이어 능력치
    public float strength;
    public float flexibility;
    public float improvisation;
    public float concentration;
    public float balancesense;

    // 아이템 사용 효과 옵션들
    // 회피 여부
    public bool isevasion;
    public int evasionlevel;
    // 회복량
    public float recovery;
    // 조준점 관련 (조준점 큰 반동 증감폭, 조준점 작은 반동 증감폭, 조준점 큰&작은 증감폭, 조준점 이동속도 증감폭, 조준점 크기 증감폭
    public float focusbigrebound;
    public float focussmallrebound;
    public float focusallrebound;
    public float focusspeed;
    public float focussize;
    // 패링관련 (패링 크기 증감포, 패링 거리 증감폭)
    public float parringsize;
    public float parringdistance;
    // 상태이상 제거
    public int debuffamount;
    // 방어구 회복
    public int armorid;
    public int armorlevel;
}

[System.Serializable] 
public class ResultAming : PacketBase
{
    public Vector3 aimdirection;
}

[System.Serializable]
public class ResultBeforeAttack : PacketBase
{
    public Vector2 randPos;
}

[System.Serializable] 
public class ResultAttack : PacketBase
{
    public int armorid;
    public int armorlevel;
    public int armorpart;
    public int armordurability;
    public float damage;
}

[System.Serializable] 
public class ResultPhaseChange : PacketBase
{
    public int nextphase;
}

[System.Serializable]
public class PingTestResult : PacketBase
{
    
}



[System.Serializable]
public class WebPacketResult
{
    public string result;
}

[System.Serializable]
public class WebLoginResult : WebPacketResult
{
    public string uuid;
}

/* Result Battle Packet - by Daru */
[System.Serializable]
public class ResultBattle : PacketBase
{
    /* DESC :> Packet name : battle
     * User unique ID
     * User hp
     * User condition
     * User armor list and armor durability(= armor hp)
     * User remain skill
     * User remain item
     * User remain lance skill
     */
    public int userId;

    public float currentHP;
    public string condition;

    public Dictionary<int, int> armorDurability;
    public List<int> availableSkill;
    public List<int> availableItem;
    public List<int> availableLanceSkill;
}
