using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSMLibrary;

public class GameMode : MonoSingleton<GameMode>
{
    GameModeData gamedata;

    UIManager uimanager;
    CameraManager cameramanager;
    AttackManager attackmanager;
    SoundManager soundmanager;

    Player player;
    Player enermy;

    UIPlaying uiplaying;
    UIAming uiaming;

    void Awake()
    {
        gamedata = GameModeData.getInstance;
    }

    // (임시) 타임 스케일 조정
    public void Initialize()
    {
        uimanager = UIManager.getInstance;
        cameramanager = CameraManager.getInstance;
        attackmanager = AttackManager.getInstance;
        soundmanager = SoundManager.getInstance;
        Time.timeScale = gamedata.testtiemscale;

        // UI초기화
        uimanager.Initialize();
        uiplaying = (UIPlaying)uimanager.GetUI("PlayingPanel");
        uiaming = (UIAming)uimanager.GetUI("AmingPanel");

        // 시네머신들 초기화
        cameramanager.Initialize();
        // 어택 매니저를 초기화
        attackmanager.Initialize(player.Playerdata, enermy.Playerdata);
        // 사운드 매니저를 초기화
        soundmanager.Initialize();

        gamedata.Isgameover = false;

        // 준비가 완료되면 게임 시작
        RoundStart();
    }

    public void InitializePlayer()
    {
        player = gamedata.player;
        gamedata.player.Initialize();
    }

    public void InitializeEnermy()
    {
        enermy = gamedata.enermy;
        gamedata.enermy.Initialize();
    }

    // 라운드 시작
    void RoundStart()
    {
        // 재귀함수로 인한 무한 코루틴 방지
        StopAllCoroutines();
        
        StartCoroutine(Cor_RoundStart(gamedata.Currentround));
    }

    IEnumerator Cor_RoundStart(int currentround)
    {
        float enterspeed = gamedata.Enterspeed;
        float speedwalk = gamedata.Speedwalk;
        float speedsprint = gamedata.Speedsprint;
        float speedaming = gamedata.Speedaming;

        // 첫 라운드라면 입장단계부터 실행 ...
        #region Round Start (Enter or Maintenance)
        if (currentround == 1)
        {
            // 라운드 시작 시 모두 초기화
            player.RoundStart();
            enermy.RoundStart();
            StartCoroutine(cameramanager.Enter());
            uimanager.ShowClose_Panel("PlayingPanel", true);

            // 입장 사운드 재생
            soundmanager.PlaySFX("Effect_SFX_0005");
            soundmanager.PlaySFX("Effect_SFX_0002");

            // 입장
            player.Enter(enterspeed, gamedata.Timeenter);
            enermy.Enter(enterspeed, gamedata.Timeenter);

            // 입장 착용 (속도는 정지)
            yield return new WaitForSeconds(gamedata.Timeenter);
            soundmanager.StopSFX("Effect_SFX_0002");
            player.EnterStop(0);
            enermy.EnterStop(0);

            yield return new WaitForSeconds(gamedata.Timeenterstop);
        }
        // 아니라면 정비부터 실행
        else
        {
            player.RoundStart();
            enermy.RoundStart();
        }
        #endregion

        #region Maintenance
        // 정비
        //soundmanager.StopSFX("Effect_SFX_0005");
        uimanager.ShowClose_Panel("PlayingPanel", false);
        player.Maintenance();
        enermy.Maintenance();
        cameramanager.Maintenance();
        uimanager.ShowClose_Panel("MaintenancePanel", true);
        #endregion

        #region Stop1
        // 정지
        yield return new WaitForSeconds(gamedata.Timemaintenance);
        uimanager.ShowClose_Panel("MaintenancePanel", false);
        uimanager.ShowClose_Panel("PlayingPanel", true);
        player.Stop(0);
        enermy.Stop(0);
        StartCoroutine(cameramanager.Cor_BeforeStop());
        uimanager.ShowClose_Panel("StopPanel", true);
        #endregion

        #region Walk1
        // 걷기
        yield return new WaitForSeconds(gamedata.Timestop);
        soundmanager.PlaySFX("Effect_SFX_0002");
        uimanager.ShowClose_Panel("StopPanel", false);
        player.UseThings();
        player.SetSpeedAndAnim(speedwalk);
        enermy.SetSpeedAndAnim(speedwalk);
        cameramanager.BeforeWalk();
        StartCoroutine(uiplaying.Cor_Player_Walk(player.Playerdata, true));
        StartCoroutine(uiplaying.Cor_Player_Walk(enermy.Playerdata, true));
        #endregion

        #region Sprint1
        // 질주 (0~3질주, 3~x 조준준비, x~5 조준중)
        yield return new WaitForSeconds(gamedata.Timewalk);
        soundmanager.StopSFX("Effect_SFX_0002");
        soundmanager.PlaySFX("Effect_SFX_0003");
        player.SetSpeedAndAnim(speedsprint);
        enermy.SetSpeedAndAnim(speedsprint);
        cameramanager.BeforeSprint();
        StartCoroutine(uiplaying.Cor_Player_Walk(player.Playerdata, false));
        StartCoroutine(uiplaying.Cor_Player_Walk(enermy.Playerdata, false));
        yield return new WaitForSeconds(gamedata.Timesprint);
        player.SprintReadyAming();
        enermy.SprintReadyAming();
        #endregion

        #region BeforeAming
        // 조준 전 (조준중 0~2.2초 + 공격준비2.2~x + 조준x~3)
        yield return new WaitForSeconds(gamedata.Timesprintaim);
        uimanager.ShowClose_Panel("AmingPanel", true);
        StartCoroutine(cameramanager.Cor_BeforeAming());
        yield return new WaitForSeconds(gamedata.Timebeforeamingaim);
        player.AmingReadyHit();
        enermy.AmingReadyHit();
        // 조준전 공격준비~
        #endregion

        #region Aming
        // 조준
        yield return new WaitForSeconds(gamedata.Timebeforeaming);
        soundmanager.StopSFX("Effect_SFX_0003");
        soundmanager.PlaySFX("Effect_SFX_0006");
        player.Aiming(speedaming);
        enermy.Aiming(speedaming);
        #endregion

        #region Attack
        // 공격 (조준 0.25초 + 회피 2.75초 + 타격 0.6초 3번 + 공격질주 2.2초)
        // 조준
        yield return new WaitForSeconds(gamedata.Timeaming);
        player.SetAnimationSpeed(0.1f);
        enermy.SetAnimationSpeed(0.1f);
        uimanager.GetUI("AmingPanel").UnShow();
        soundmanager.StopSFX("Effect_SFX_0006");
        // 회피
        yield return new WaitForSeconds(gamedata.Timeattackaming);
        player.AttackAvoid();
        enermy.AttackAvoid();

        #region 공격판정지점
        // 공격 판정
        yield return new WaitForSeconds(0.5f - gamedata.Timeattackaming);
        // ... 랜덤 좌표 받아와야함
        bool ishit = attackmanager.Attack();
        bool ishitted = false;  // 적이 나를 떄렸는가
        #endregion

        // 타격 0.6 * 3
        yield return new WaitForSeconds(gamedata.Timeattackavoid - (0.5f - gamedata.Timeattackaming));
        cameramanager.BeforeAttack();
        player.SetAnimationSpeed(0.6f);
        enermy.SetAnimationSpeed(0.6f);
        Vector3 playerattackposition = player.transform.position;
        Vector3 enermyattackposition = enermy.transform.position;
        player.Attack(gamedata.Speedattack, ishit, ishitted);
        enermy.Attack(gamedata.Speedattack, ishit, ishitted);
        uiplaying.SetUI_HPBar(player.Playerdata, enermy.Playerdata);
        for (int i=0; i<gamedata.Attackplaycount; i++)
        {
            player.AttackCount(i);
            enermy.AttackCount(i);
            yield return new WaitForSeconds(0.3f);
            if (ishit)
            {
                soundmanager.PlayOneShotSFX("Effect_SFX_0007");
            }
            else
            {
                soundmanager.PlayOneShotSFX("Effect_SFX_0008");
            }
            player.transform.position = playerattackposition;
            enermy.transform.position = enermyattackposition;
            cameramanager.Attack(i);
            yield return new WaitForSeconds(gamedata.Timeattack - 0.3f);
        }
        // 공격질주 2.2초
        soundmanager.PlaySFX("Effect_SFX_0003");
        if (ishit)
        {
            soundmanager.PlaySFX("Effect_SFX_0004");
        }
        else
        {
            soundmanager.PlaySFX("Effect_SFX_0009");
        }
        player.SetAnimationSpeed(1);
        enermy.SetAnimationSpeed(1);
        cameramanager.AttackSprint();
        // 세레모니는 알아서 재생됨
        player.SetSpeedAndAnim(speedsprint);
        enermy.SetSpeedAndAnim(speedsprint);
        #endregion

        #region sprint2
        // 질주2
        yield return new WaitForSeconds(gamedata.Timeattacksprint);
        StartCoroutine(uiaming.Cor_ShowSprint());
        player.SetSpeedAndAnim(speedsprint);
        enermy.SetSpeedAndAnim(speedsprint);
        cameramanager.AfterSprint();
        #endregion

        #region walk2
        // 걷기2
        yield return new WaitForSeconds(gamedata.Timesprint);
        soundmanager.StopSFX("Effect_SFX_0003");
        soundmanager.PlaySFX("Effect_SFX_0002");
        player.SetSpeedAndAnim(speedwalk);
        enermy.SetSpeedAndAnim(speedwalk);
        cameramanager.AfterWalk();
        gamedata.Currentround++;
        //종료조건이라면 엑스트라라운드와 게임오버를 판별
        if (player.CurrentHP == 0.0f || enermy.CurrentHP == 0.0f || currentround > gamedata.Maxround)
        {
            // 엑스트라 라운드가 남았다면 엑스트라 라운드를 알림
            if (player.CurrentHP == 0.0f && enermy.CurrentHP == 0.0f && currentround <= gamedata.Extraround + gamedata.Maxround)
            {
                //gamedata.Isextraround = true;
                Debug.Log("extraround on");
            }
            else
            {
                gamedata.Isgameover = true;
                StartCoroutine(uiplaying.Cor_Result(gamedata));
                Debug.Log("gameover on");
            }
        }
        // ... GameSet 표시기능 추가
        #endregion

        #region stop2
        // 정지2
        yield return new WaitForSeconds(gamedata.Timewalk);
        soundmanager.StopSFX("Effect_SFX_0004");
        soundmanager.StopSFX("Effect_SFX_0009");
        soundmanager.StopSFX("Effect_SFX_0002");
        player.Stop(0);
        enermy.Stop(0);
        cameramanager.AfterStop();
        uimanager.ShowClose_Panel("GameoverPanel", true);
        #endregion

        #region round_restart
        // 정지 5초 후 라운드 재시작
        yield return new WaitForSeconds(gamedata.Timeafterstop);
        // 익스트라 라운드 및 일반 라운드 실행 & 게임오버라면 아무것도 하지 않음
        if (!gamedata.Isgameover)
        {
            uimanager.ShowClose_Panel("GameoverPanel", false);
            RoundStart();
        }
        #endregion
    }

    public void SendToServerBeforeAttack(bool _ispressed)
    {
        RequestBeforeAttack res = new RequestBeforeAttack();
        res.packet = "focus_check";
        res.Id = gamedata.player.Playerdata.Userid;
        res.roomId = gamedata.Roomid;
        res.charId = gamedata.player.Playerdata.Charid;
        res.lanceId = gamedata.player.Playerdata.Lances[player.Playerdata.Currentlance].Info.index;
        res.ispadpressed = _ispressed;
        NetworkConnector.getInstance.SendToServer(res);

        Debug.Log("SendToServer : " + res);
    }
}
