using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;

public class GameModeData : MonoSingleton<GameModeData>
{
    public float testtiemscale = 1;

    public Player player;
    public Player enermy;

    private int roomid = 0;
    public int Roomid
    {
        get { return roomid; }
        set { roomid = value; }
    }

    // 게임모드 변수
    private int maxround = 5;               // 최대 라운드 수
    public int Maxround
    {
        get { return maxround; }
    }
    private int currentround = 1;           // 현재 라운드
    public int Currentround
    {
        get { return currentround; }
        set { currentround = value; }
    }
    private int extraround = 1;             // 여분 라운드
    public int Extraround
    {
        get { return extraround; }
    }

    // 각 페이즈별 시간 및 시네머신 카운트
    private float timeenter = 8;                // 입장 움직임 시간
    public float Timeenter
    {
        get { return timeenter; }
    }
    private float timeenterstop = 7;            // 입장 투구 착용 시간  // 입장 총 15초
    public float Timeenterstop
    {
        get { return timeenterstop; }
    }

    private float timemaintenance = 16;          // 정비 시간
    public float Timemaintenance
    {
        get { return timemaintenance; }         // UISHow_maintanance 코루틴 카운트다운에 사용하기 위해서
    }

    private float timestop = 9;                 // 정지 시간
    public float Timestop
    {
        get { return timestop; }
    }
    private int stopplaycount = 3;              // 정지 시간 동안 실행될 시네머신 개수
    public int Stopplaycount
    {
        get { return stopplaycount; }
    }

    private float timewalk = 5;                 // 서행 시간
    public float Timewalk
    {
        get { return timewalk; }
    }

    private float timesprint = 3;               // 질주 시간
    public float Timesprint
    {
        get { return timesprint; }
    }
    private float timesprintaim = 2;            //질주 도중 에임준비 시간 (조준준비 + 조중중)
    public float Timesprintaim
    {
        get { return timesprintaim; }
    }

    private float timebeforeamingaim = 2.2f;    // 조준 전 에이밍 시간 (조준 에이밍 2.2초 + 조중중 공격준비시간 0.8초)
    public float Timebeforeamingaim
    {
        get { return timebeforeamingaim; }
    }
    private float timebeforeaming = 0.8f;       // 조준 전 시간
    public float Timebeforeaming
    {
        get { return timebeforeaming; }
    }

    private float timeaming = 5;                // 조준 시간
    public float Timeaming
    {
        get { return timeaming; }
    }

    private float timeattackaming = 0.25f;      // 공격 조준 시간
    public float Timeattackaming
    {
        get { return timeattackaming; }
    }
    private float timeattackavoid = 2.75f;      // 공격 회피 시간
    public float Timeattackavoid
    {
        get { return timeattackavoid; }
    }
    private float timeattack = 1;               // 공격 시간
    public float Timeattack
    {
        get { return timeattack; }
    }
    private int attackplaycount = 3;            // 공격을 보여주는 반복 횟수
    public int Attackplaycount
    {
        get { return attackplaycount; }
    }
    private float timeattacksprint = 2.2f;      // 공격 질주 시간
    public float Timeattacksprint
    {
        get { return timeattacksprint; }
    }
    private float timeafterstop = 5;            // 공격 정지 시간
    public float Timeafterstop
    {
        get { return timeafterstop; }
    }

    // 각 페이즈별 이동속도
    float enterspeed = 2;
    public float Enterspeed                     // 입장 스피드
    {
        get { return enterspeed; }
    }
    float speedwalk = 3;                        // 서행 스피드
    public float Speedwalk
    {
        get { return speedwalk; }
    }
    float speedsprint = 12;                     // 질주1 스피드
    public float Speedsprint
    {
        get { return speedsprint; }
    }
    float speedaming = 0.1f;                    // 조준 스피드 (질주 스피드 0.1배속)
    public float Speedaming
    {
        get { return speedaming; }
    }
    float speedattack = 1.2f;
    public float Speedattack
    {
        get { return speedattack; }
    }

    public float focusradius = 75;

    public float focusmovespeed = 100;

    public float bigreboundtime = 0.38f;
    public float bigreboundmin = 15;
    public float bigreboundmax = 40;

    public float smallreboundtime = 0.04f;
    public float smallreboundmin = 15;
    public float smallreboundmax = 40;

    bool isgameover = false;
    public bool Isgameover
    {
        get { return isgameover; }
        set { isgameover = value; }
    }

    void Awake()
    {
        //speedaming = speedsprint * 0.1f;
        //speedattack = speedsprint * 0.6f;
    }
}