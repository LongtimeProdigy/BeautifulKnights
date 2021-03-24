using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

    #region parameters
    public bool isplayer = true;

    private int userid;
    public int Userid
    {
        get { return userid; }
        set { userid = value; }
    }

    private int charid = 0;
    public int Charid
    {
        get { return charid; }
        set { charid = value; }
    }

    // UIManaver에서 사용하기 위해 프로퍼티 선언
    private float maxhp = 100.0f;           // 최대 체력
    public float Maxhp
    {
        get { return maxhp; }
    }
    private float currenthp = 0.0f;         // 현재 체력
    public float Currenthp
    {
        get { return currenthp; }
        set { currenthp = value; }
    }
    private float movespeed = 0.0f;         // 현재 스피드
    public float Movespeed
    {
        get { return movespeed; }
        set { movespeed = value; }
    }

    // 창 인벤토리 5칸 생성
    private Lance[] lances = new Lance[5];
    public Lance[] Lances
    {
        get { return lances; }
    }

    // 캐릭터 스킬 5칸 생성
    private Skill[] skills = new Skill[5];
    public Skill[] Skills
    {
        get { return skills; }
    }

    // 아이템 인벤토리 5칸 생성
    private Item[] items = new Item[5];
    public Item[] Items
    {
        get { return items; }
    }

    // 디버프 인벤토리 5칸 생성
    private Debuff[] debuffs = new Debuff[5];
    public Debuff[] Debuffs
    {
        get { return debuffs; }
    }

    // 현재 장착된 창/스킬/아이템 (-1=아무것도 장착하지 않음)
    private int currentlance = -1;
    public int Currentlance
    {
        get { return currentlance; }
        set { currentlance = value; }
    }
    private int currentskill = -1;
    public int Currentskill
    {
        get { return currentskill; }
        set { currentskill = value; }
    }
    private int currentitem = -1;
    public int Currentitem
    {
        get { return currentitem; }
        set { currentitem = value; }
    }
    private int currentdebuff = -1;
    public int Currentdebuff
    {
        get { return currentdebuff; }
    }

    // 캐릭터 능력치
    public struct stat
    {
        public float strength;
        public float flexibility;
        public float improvisation;
        public float concentration;
        public float balancesense;

        public void Initialize(float _strength, float _flexibility, float _improvisation, float _concentration, float _balancesense)
        {
            strength = _strength;
            flexibility = _flexibility;
            improvisation = _improvisation;
            concentration = _concentration;
            balancesense = _balancesense;
        }
    }
    private stat playerstat;
    public stat Playerstat
    {
        get { return playerstat; }
    }
    public struct tempstat
    {
        public float focusmovespeed;
        public float focusradius;
        public float focusbigrebound;
        public float focussmallrebound;

        public void Initialize()
        {
            focusmovespeed = 0;
            focusradius = 0;
            focusbigrebound = 0;
            focussmallrebound = 0;
        }
    }
    private tempstat playertempstat;
    public tempstat Playertempstat
    {
        get { return playertempstat; }
        set { playertempstat = value; }
    }

    private Dictionary<string, GameObject> destroyedArmor = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> DestroyedArmor
    {
        get { return destroyedArmor; }
    }

    private bool isavoid = false;
    public bool Isavoid
    {
        get { return isavoid; }
    }
    private int attackpoint = 0;
    public int Attackpoint
    {
        get { return attackpoint; }
        set { attackpoint = value; }
    }
    #endregion

    // 게임 시작 시 캐릭터의 여러가지 설정
    public void Initialize(bool _isplayer)
    {
        isplayer = _isplayer;

        currenthp = maxhp;

        playertempstat.Initialize();
    }

    public void MakeInventory(Deck deck)
    {
        XmlManager xmlmanager = new XmlManager();
        playerstat.Initialize(float.Parse(xmlmanager.LoadAttribute(XmlManager.eType.Character, deck.charId, "STRENGTH")),
            float.Parse(xmlmanager.LoadAttribute(XmlManager.eType.Character, deck.charId, "FLEXIBILITY")),
            float.Parse(xmlmanager.LoadAttribute(XmlManager.eType.Character, deck.charId, "IMPROVISATION")),
            float.Parse(xmlmanager.LoadAttribute(XmlManager.eType.Character, deck.charId, "CONCENTRARION")),
            float.Parse(xmlmanager.LoadAttribute(XmlManager.eType.Character, deck.charId, "BALANCESENSE")));

        for (int i = 0; i < 5; i++)
        {
            lances[i] = new Lance(deck.lanceIds[i]);
            skills[i] = new Skill(deck.skillIds[i], int.Parse(xmlmanager.LoadAttribute(XmlManager.eType.Character, deck.charId, string.Format("LV{0}", i + 1))));
            items[i] = new Item(i);
            debuffs[i] = new Debuff(i);
        }
    }
}
