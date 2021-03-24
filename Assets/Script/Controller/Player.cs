using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class Player : MonoBehaviour {

    public bool isplayer = true;

    PlayerData playerdata;
    public PlayerData Playerdata
    {
        get { return playerdata; }
    }
    public float CurrentHP
    {
        get { return playerdata.Currenthp; }
    }

    Animator playeranimator;
    Animator horseanimator;

    // (임시) 플레이어 위치 조정을 위한 변수
    private float character_position_z;

    // 게임 시작 시 캐릭터의 여러가지 설정
    public void Initialize()
    {
        // 플레이어 데이터 세팅
        playerdata = new PlayerData();
        playerdata.Initialize(isplayer);
        
        // 애니메이터 세팅
        playeranimator = transform.Find("Model_Char_0001").GetComponent<Animator>();
        horseanimator = transform.Find("Horse").GetComponent<Animator>();

        // 플레이어 이름 클라이언트DB에서 추출하여 설정 (지금은 랜덤임 서버에서 받아오시길....)
        

        // (임시)
        // 캐릭터 초기 위치거리 계산(z값)
        // 서행 서행5초
        // 질주 질주3초 + 질주조준 2초+ 조준전에이밍2.2초 + 조준전0.8초
        // 공격조준 5초
        // 타격 1초
        GameModeData gamedata = GameModeData.getInstance;
        character_position_z = (gamedata.Speedwalk * (gamedata.Timewalk)) +
            (gamedata.Speedsprint * (gamedata.Timesprint + gamedata.Timesprintaim + gamedata.Timebeforeaming + gamedata.Timebeforeamingaim + 0.5f)) +
            (gamedata.Speedaming * gamedata.Timeaming);
        Debug.Log(string.Format("Character Position{0}: {1}", isplayer ? "player" : "enermy", character_position_z));
    }

    public void SetttingName(Deck deck)
    {
        XmlManager xmlmanager = new XmlManager();
        name = xmlmanager.LoadAttribute(XmlManager.eType.Character, deck.charId, "name");
    }

    // 라운드 시작시 호출되는 함수(from. GameMode)
    public void RoundStart()
    {        
        // 라운드 시작 시 위치 초기화
        if (playerdata.isplayer)
        {
            transform.position = new Vector3(1.0f, 0.0f, -character_position_z);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else
        {
            transform.position = new Vector3(-1.0f, 0.0f, character_position_z);
            transform.rotation = Quaternion.Euler(0.0f, -180.0f, 0.0f);
        }
    }

    // Update is called once per frame
    void Update () {
        //캐릭터 이동
        if (playerdata != null)
            transform.Translate(Vector3.forward * playerdata.Movespeed * Time.deltaTime, Space.Self);
    }

    // 입장
    public void Enter(float speed, float timeenter)
    {
        // 입장시 위치 재조정
        if (playerdata.isplayer)
        {
            this.transform.position = new Vector3(1.0f, 0.0f, -(character_position_z + speed * timeenter));
            this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else
        {
            this.transform.position = new Vector3(-1.0f, 0.0f, character_position_z + speed * timeenter);
            this.transform.rotation = Quaternion.Euler(0.0f, -180.0f, 0.0f);
        }

        // 입장 속도 설정 및 애니메이션 재설정
        playerdata.Movespeed = speed;
        playeranimator.SetFloat("Speed", speed);
        horseanimator.SetFloat("Speed", speed);
        playeranimator.SetBool("isEnter", true);
    }

    // 입장정지(갑옷을 착용하는 모션)
    public void EnterStop(float speed)
    {
        // 정지
        playerdata.Movespeed = speed;
        playeranimator.SetFloat("Speed", speed);
        horseanimator.SetFloat("Speed", speed);
        playeranimator.SetBool("isEnter", false);
        playeranimator.SetBool("isEnterStop", true);
    }

    // 정비
    public void Maintenance()
    {
        playeranimator.SetBool("isEnterStop", false);
    }

    //정지
    public void Stop(float speed)
    {
        // 이동속도 설정 (정지)
        playerdata.Movespeed = speed;
        playeranimator.SetFloat("Speed", speed);
        horseanimator.SetFloat("Speed", speed);
    }

    // 캐릭터 스피드 및 애니메이션 설정(서행, 질주)
    public void SetSpeedAndAnim(float speed)
    {
        // 캐릭터 애니메이션 초기화
        // playeranimator.SetBool("isAming", false);
        playeranimator.SetBool("isAttack", false);

        // 속도 설정
        playerdata.Movespeed = speed;
        playeranimator.SetFloat("Speed", speed);
        horseanimator.SetFloat("Speed", speed);
    }

    // 질주단계-조준준비
    public void SprintReadyAming()
    {
        playeranimator.SetBool("isEnterAming", true);
    }

    // 조준단계-공격준비
    public void AmingReadyHit()
    {
        playeranimator.SetBool("isReadyAming", true);
    }
    // 조준
    public void Aiming(float speed)
    {
        // 캐릭터 및 말 애니메이션 설정(슬로우 모션)
        playeranimator.SetBool("isReadyAming", false);
        horseanimator.SetBool("isSlow", true);

        // 이동속도 설정 (슬로우 모션)
        playerdata.Movespeed = speed;
        playeranimator.SetFloat("Speed", speed);
        horseanimator.SetFloat("Speed", speed);
    }

    // 공격-회피
    public void AttackAvoid()
    {
        playeranimator.SetBool("isEnterAming", false);
        playeranimator.SetBool("isEnterAttack", true);
        if (playerdata.Isavoid)
        {
            playeranimator.SetInteger("AvoidDirection", Random.Range(0, 6));
        }
        else
        {
            playeranimator.SetInteger("AvoidDirection", -1);
        }
    }

    // 공격
    public void Attack(float speed, bool ishit, bool ishitted)
    {
        // 캐릭터 및 말 애니메이션 설정
        playeranimator.SetBool("isEnterAttack", false);
        playeranimator.SetBool("isAttack", true);
        playeranimator.SetInteger("AttackPoint", playerdata.Attackpoint);
        playeranimator.SetBool("isHit", ishit);
        if (!ishit && ishitted)
        {
            playeranimator.SetInteger("AttackPoint", -1);
        }
        horseanimator.SetBool("isSlow", false);

        // 이동속도 설정 (다시 되돌리기)
        playerdata.Movespeed = speed;
        playeranimator.SetFloat("Speed", speed);
        horseanimator.SetFloat("Speed", speed);
    }

    public void AttackCount(int count)
    {
        playeranimator.SetInteger("AttackCount", count);
    }

    public void SetAnimationSpeed(float multiple)
    {
        playeranimator.speed = multiple;
        horseanimator.speed = multiple;
    }

    public void UseThings()
    {
        // 창, 스킬, 아이템, 디버프 사용처리
        // lances[currentlance].Isused = true;      // 창 사용처리는 uimanintenance에서
        LanceSkill lanceskill = playerdata.Lances[playerdata.Currentlance].Skill;
        if (!lanceskill.Isused)
        {
            playerdata.Lances[playerdata.Currentlance].Skill.Isused = true;
            //playerdata.Lances[playerdata.Currentlance].Skill.Use(this);
        }
        if (playerdata.Currentskill != -1 && !playerdata.Skills[playerdata.Currentskill].Isused)
        {
            playerdata.Skills[playerdata.Currentskill].Isused = true;
            //playerdata.Skills[playerdata.Currentskill].Use(this);
        }
        if (playerdata.Currentitem != -1 && !playerdata.Items[playerdata.Currentitem].Isused)
        {
            playerdata.Items[playerdata.Currentitem].Isused = true;
            //playerdata.Items[playerdata.Currentitem].Use(this);
        }
        if(playerdata.Currentdebuff != -1 && playerdata.Debuffs[playerdata.Currentdebuff].Isused)
        {
            //playerdata.Debuffs[playerdata.Currentdebuff].Use(this);
        }
        // 디버프 사용처리 추가할 예정

        //GameModeData gamedata = GameModeData.getInstance;
        //Lance lance = playerdata.Lances[playerdata.Currentlance];
        //// 능력치 적용 (포커스 크기, 포커스 이동속도, ...)
        //// 포커스 이동 속도
        //gamedata.focusmovespeed = gamedata.focusmovespeed * 
        //    ((75 + playerdata.Playerstat.improvisation) / (75 + lance.Stat.penetration)) * 
        //    (100 + playerdata.Playertempstat.focusmovespeed) / 100;

        //// 포커스 반지름
        //gamedata.focusradius = gamedata.focusradius * 
        //    ((75 + lance.Stat.parring) / (75 + playerdata.Playerstat.concentration)) * 
        //    (100 - playerdata.Playertempstat.focusradius) / 100;

        //// 포커스 큰 반동 최소폭
        //gamedata.bigreboundmin = gamedata.bigreboundmin * 
        //    ((75 + lance.Stat.destructive) / (75 + playerdata.Playerstat.improvisation)) * 
        //    (100 - playerdata.Playertempstat.focusbigrebound) / 100;
        //// 포커스 큰 반동 최대폭
        //gamedata.bigreboundmax = gamedata.bigreboundmin * 2.666f;

        //// 포커스 작은 반동 최소폭
        //gamedata.smallreboundmin = gamedata.smallreboundmin * 
        //    ((75 + lance.Stat.weight) / (75 + playerdata.Playerstat.balancesense)) * 
        //    (100 - playerdata.Playertempstat.focussmallrebound) / 100;
        //// 포커스 작은 반동 최대폭
        //gamedata.smallreboundmax = gamedata.smallreboundmin * 2;
    }
}