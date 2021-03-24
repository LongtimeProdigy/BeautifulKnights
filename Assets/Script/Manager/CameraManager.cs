using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;
using Cinemachine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CameraManager : MonoSingleton<CameraManager> {

    GameModeData gamedata;
    public Camera maincamera;

    public GameObject looktarget;

    public GameObject[] playerenter = new GameObject[4];
    public GameObject[] enermyenter = new GameObject[4];

    public GameObject enter_equip_helmet;

    public GameObject maintenance1;
    private CinemachineDollyCart maintenancecart;

    public GameObject[] playerstop = new GameObject[2];
    private CinemachineDollyCart[] playerstopcart = new CinemachineDollyCart[2];

    public GameObject[] beforewalk = new GameObject[2];
    private CinemachineDollyCart[] beforewalkcart = new CinemachineDollyCart[2];

    public GameObject[] beforesprint = new GameObject[1];
    private CinemachineDollyCart[] beforesprintcart = new CinemachineDollyCart[1];

    public GameObject[] beforeaming = new GameObject[1];
    private CinemachineDollyCart[] beforeamingcart = new CinemachineDollyCart[1];

    public GameObject[] aming = new GameObject[1];

    public GameObject[] attack = new GameObject[3];

    public GameObject attacksprint;
    private CinemachineDollyCart attacksprintcart;

    public GameObject[] aftersprint = new GameObject[2];
    private CinemachineDollyCart[] aftersprintcart = new CinemachineDollyCart[2];
    private CinemachineVirtualCamera[] aftersprintcamera = new CinemachineVirtualCamera[2];

    public GameObject[] afterwalk = new GameObject[2];
    private CinemachineDollyCart[] afterwalkcart = new CinemachineDollyCart[2];

    public GameObject[] afterstop = new GameObject[2];
    private CinemachineDollyCart[] afterstopcart = new CinemachineDollyCart[2];
    private CinemachineVirtualCamera[] afterstopcamera = new CinemachineVirtualCamera[2];

    Queue<Coroutine> changeFOVqueue = new Queue<Coroutine>();

    // 랜덤으로 뽑힌 숫자를 저장할 곳
    int rn1, rn2;

    public void Initialize()
    {
        gamedata = GameModeData.getInstance;

        looktarget.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
        looktarget.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

        maintenancecart = maintenance1.transform.Find("CDC").GetComponent<CinemachineDollyCart>();

        enter_equip_helmet.SetActive(false);
        maintenance1.SetActive(false);

        for (int i = 0; i < playerenter.Length; i++)
        {
            playerenter[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>().m_Speed = playerenter[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / (gamedata.Timeenter / 2);
            enermyenter[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>().m_Speed = enermyenter[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / (gamedata.Timeenter / 2);
            playerenter[i].SetActive(false);
            enermyenter[i].SetActive(false);
        }

        enter_equip_helmet.transform.Find("CDC").GetComponent<CinemachineDollyCart>().m_Speed = enter_equip_helmet.transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / gamedata.Timeenterstop;

        maintenance1.transform.Find("CDC").GetComponent<CinemachineDollyCart>().m_Speed = maintenance1.transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / gamedata.Timemaintenance;

        for (int i = 0; i < playerstop.Length; i++)
        {
            playerstopcart[i] = playerstop[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            playerstopcart[i].m_Speed = playerstop[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / (gamedata.Timestop);
            //enermystopcart[i] = enermystop[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            //enermystopcart[i].m_Speed = enermystop[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / (gamedata.Timestop);
            playerstop[i].SetActive(false);
            //enermystop[i].SetActive(false);
        }

        for (int i = 0; i < beforewalk.Length; i++)
        {
            beforewalkcart[i] = beforewalk[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            beforewalkcart[i].m_Speed = beforewalk[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / gamedata.Timewalk;
            beforewalk[i].SetActive(false);
        }

        for (int i = 0; i < beforesprint.Length; i++)
        {
            beforesprintcart[i] = beforesprint[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            beforesprintcart[i].m_Speed = beforesprint[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / (gamedata.Timesprint + gamedata.Timesprintaim);
            beforesprint[i].SetActive(false);
        }

        for (int i = 0; i < beforeaming.Length; i++)
        {
            beforeamingcart[i] = beforeaming[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            beforeamingcart[i].m_Speed = beforeaming[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / (gamedata.Timebeforeaming + gamedata.Timebeforeamingaim);
            beforeaming[i].SetActive(false);
        }

        for (int i = 0; i < attack.Length; i++)
        {
            attack[i].SetActive(false);
        }

        attacksprintcart = attacksprint.transform.Find("CDC").GetComponent<CinemachineDollyCart>();
        attacksprintcart.m_Speed = attacksprint.transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / gamedata.Timeattacksprint;
        attacksprint.SetActive(false);

        for (int i = 0; i < aftersprint.Length; i++)
        {
            aftersprintcart[i] = aftersprint[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            aftersprintcart[i].m_Speed = aftersprint[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / gamedata.Timesprint;
            aftersprintcamera[i] = aftersprint[i].transform.Find("CDC").GetComponent<CinemachineVirtualCamera>();
            aftersprint[i].SetActive(false);
        }

        for (int i = 0; i < afterwalk.Length; i++)
        {
            afterwalkcart[i] = afterwalk[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            afterwalkcart[i].m_Speed = afterwalk[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / gamedata.Timewalk;
            afterwalk[i].SetActive(false);
        }

        for (int i = 0; i < afterstop.Length; i++)
        {
            afterstopcart[i] = afterstop[i].transform.Find("CDC").GetComponent<CinemachineDollyCart>();
            afterstopcart[i].m_Speed = afterstop[i].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().PathLength / gamedata.Timestop;
            afterstopcamera[i] = afterstop[i].transform.Find("CDC").GetComponent<CinemachineVirtualCamera>();
            afterstop[i].SetActive(false);
        }
    }

    IEnumerator MoveTarget(Vector3 toPosition, float time)
    {
        //yield return new WaitForSeconds(0.5f);

        Vector3 startvector = looktarget.transform.localPosition;

        for (float i = 0; i <= 1; i += 1 / (time / Time.deltaTime))
        {
            //looktarget.transform.localPosition += toPosition / (time / Time.deltaTime);
            looktarget.transform.localPosition = Vector3.Lerp(startvector, toPosition , i);
            yield return null;
        }
    }

    IEnumerator ChangeCameraFOV(CinemachineVirtualCamera target, float startfov, float endfov, float camerazoomtime)
    {
        if (changeFOVqueue.Count != 0)
        {
            StopCoroutine(changeFOVqueue.Dequeue());
        }
        
        target.m_Lens.FieldOfView = startfov;

        float inclination = 0.05f / camerazoomtime;

        for (float i = 0; i <= 1; i = i + inclination)
        {
            target.m_Lens.FieldOfView = Mathf.Lerp(startfov, endfov, i);

            yield return new WaitForSeconds(0.05f);
        }

        target.m_Lens.FieldOfView = endfov;
    }

    // 입장 카메라 시테마틱 실행
    public IEnumerator Enter()
    {
        rn1 = Random.Range(0, playerenter.Length);
        rn2 = Random.Range(0, playerenter.Length);

        // 캐릭터 시네마틱
        playerenter[rn1].SetActive(true);

        // 3초 뒤 적 시네마틱
        yield return new WaitForSeconds(gamedata.Timeenter / 2);
        looktarget.transform.parent = gamedata.enermy.transform;
        looktarget.transform.localPosition = new Vector3(0, 2, 0);
        playerenter[rn1].SetActive(false);
        enermyenter[rn2].SetActive(true);

        // 3초 뒤 투구 착용 시작
        yield return new WaitForSeconds(gamedata.Timeenter / 2);
        looktarget.transform.parent = gamedata.player.transform;
        looktarget.transform.localPosition = new Vector3(0, 2, 0);
        enermyenter[rn2].SetActive(false);
        enter_equip_helmet.SetActive(true);
    }

    public void Maintenance()
    {
        if(gamedata.Currentround == 1)
        {
            // 이전 시네머신 초기화
            enter_equip_helmet.SetActive(false);
        }
        else
        {
            // 정지 애니메이션 초기화(1라운드에는 필요없지만 다음 라운드부터 필요)
            afterstopcart[rn1].m_Position = 0;
            afterstop[rn1].SetActive(false);
        }

        // (임시) 룩타겟 부모 다시 설정
        looktarget.transform.parent = gamedata.player.transform;
        looktarget.transform.localPosition = new Vector3(0, 2, 0);

        // 정비 시네머신 실행
        maintenance1.SetActive(true);
    }

    public IEnumerator Cor_BeforeStop()
    {
        // 이전 시네머신 초기화
        maintenancecart.m_Position = 0;
        maintenance1.SetActive(false);

        // target 위치 초기화
        looktarget.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);

        rn1 = Random.Range(0, playerstop.Length);

        playerstop[rn1].SetActive(true);

        switch (rn1)
        {
            case 0:
                // 아무것도 할 게 없음
                break;
            case 1:
                // 4.5초뒤 시점의 변환
                yield return new WaitForSeconds((gamedata.Timestop / 2) + 1);
                //Vector3 destposition = gamedata.enermy.transform.position;
                StartCoroutine(MoveTarget(new Vector3(0, 8.5f, 0), (gamedata.Timestop / 2) - 1f));
                break;
            default:
                Debug.Log("BeforeStop 시네머신 오류");
                break;
        }
    }

    public void BeforeWalk()
    {
        // 이전 시네머신 초기화
        playerstop[rn1].SetActive(false);
        playerstopcart[rn1].m_Position = 0;

        // target 위치 초기화
        looktarget.transform.parent = gamedata.player.transform;
        looktarget.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);

        rn1 = Random.Range(0, beforewalk.Length);

        beforewalk[rn1].SetActive(true);
    }

    public void BeforeSprint()
    {
        // 이전 시네머신 초기화
        beforewalkcart[rn1].m_Position = 0;
        beforewalk[rn1].SetActive(false);

        // target 위치 초기화
        looktarget.transform.localPosition = new Vector3(0, 2, 0);
        //looktarget.transform.parent = gamedata.enermy.transform;

        // 현재 시네머신 실행
        rn1 = Random.Range(0, beforesprint.Length);
        beforesprint[rn1].SetActive(true);
    }

    // 조준 전과 조준 둘 다 포함되어 있음
    public IEnumerator Cor_BeforeAming()
    {
        // 이전 시네머신 초기화
        beforesprintcart[rn1].m_Position = 0;
        beforesprint[rn1].SetActive(false);

        // looktarget 초기화
        looktarget.transform.parent = gamedata.enermy.transform;
        looktarget.transform.localPosition = new Vector3(0, 2, 0);

        // 현재 시네머신 실행
        rn1 = Random.Range(0, beforeaming.Length);
        beforeaming[rn1].SetActive(true);

        //float targetpositionz = (gamedata.enermy.transform.position.z - gamedata.Timebeforeaming * gamedata.Speedsprint) - (gamedata.player.transform.position.z + gamedata.Timebeforeaming * gamedata.Speedsprint);
        //StartCoroutine(MoveTarget(new Vector3(-2, 2, targetpositionz), gamedata.Timebeforeaming));

        // aming 단계 돌입
        yield return new WaitForSeconds(gamedata.Timebeforeaming);

        //Vector3 amingposition = (gamedata.player.transform.position - gamedata.enermy.transform.position) - beforeaming[rn1].

        looktarget.transform.parent = gamedata.enermy.transform;
        looktarget.transform.localPosition = new Vector3(0, 2, 0);

        //beforeaming[rn1].SetActive(false);
        //beforeamingcart[rn1].m_Position = 0;
        //Vector3 attackposition = gamedata.player.transform.position + beforeaming[rn1].transform.Find("CSP").GetComponent<CinemachineSmoothPath>().m_Waypoints[1].position;
        //beforeattack.transform.position = attackposition;
        //beforeattack.SetActive(true);

        // 5초 뒤에 종료
    }

    public void BeforeAttack()
    {
        // 이전 시네머신 초기화
        beforeaming[rn1].SetActive(false);
        beforeamingcart[rn1].m_Position = 0;

        looktarget.transform.parent = null;
        looktarget.transform.position = new Vector3(0, 0, 0);
    }

    public void Attack(int stack)
    {
        if(stack != 0)
        {
            attack[stack - 1].SetActive(false);
        }

        attack[stack].SetActive(true);
    }

    public void AttackSprint()
    {
        attack[attack.Length - 1].SetActive(false);

        looktarget.transform.parent = gamedata.player.transform;
        looktarget.transform.localPosition = new Vector3(0, 2, 0);

        // 현재 시네머신 실행 및 카메라 줌조절
        attacksprint.SetActive(true);
    }
    
    public void AfterSprint()
    {
        // 이전 시네머신 초기화
        attacksprintcart.m_Position = 0;
        attacksprint.SetActive(false);

        rn1 = Random.Range(0, aftersprint.Length);

        // 현재 시네머신 실행 및 카메라 줌조절
        aftersprint[rn1].SetActive(true);

        changeFOVqueue.Enqueue(StartCoroutine(ChangeCameraFOV(aftersprintcamera[rn1], 20, 40, gamedata.Timeattacksprint)));
    }

    public void AfterWalk()
    {
        // 이전 시네머신 초기화
        aftersprintcart[rn1].m_Position = 0;
        aftersprint[rn1].SetActive(false);

        rn1 = Random.Range(0, afterwalk.Length);

        afterwalk[rn1].SetActive(true);
    }

    public void AfterStop()
    {
        // 이전 시네머신 초기화
        afterwalkcart[rn1].m_Position = 0;
        afterwalk[rn1].SetActive(false);

        rn1 = Random.Range(0, afterstop.Length);

        afterstop[rn1].SetActive(true);
    }
}
