using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSMLibrary;

public class UIManager : MonoSingleton<UIManager> {

    private UILabel lbl_timer;              // (임시)게임중UI - 타이머
    private UILabel lbl_speed;              // (임시)게임중UI - 스피드
    private UILabel lbl_round;              // (임시)게임중UI - 현재 라운드
    private float passedTime = 0.0f;        // (임시) 지난 시간
    bool isactive = false;                  // (임시) update 오류 방지용

    Dictionary<string, UIShow> uis = new Dictionary<string, UIShow>();
    public UIShow GetUI(string name)
    {
        if (uis.ContainsKey(name))
        {
            return uis[name];
        }
        else
        {
            Debug.Log(name + " is not found");
            return null;
        }
    }

    public void Initialize()
    {
        // Playing 패널 설정
        lbl_timer = transform.Find("PlayingPanel/Time_Label").GetComponent<UILabel>();
        lbl_speed = transform.Find("PlayingPanel/Speed_Label").GetComponent<UILabel>();
        lbl_round = transform.Find("PlayingPanel/Round_Label").GetComponent<UILabel>();

        uis.Add("MaintenancePanel", transform.Find("MaintenancePanel").GetComponent<UIMaintenance>());
        uis.Add("StopPanel", transform.Find("StopPanel").GetComponent<UIStop>());
        uis.Add("PlayingPanel", transform.Find("PlayingPanel").GetComponent<UIPlaying>());
        uis.Add("AmingPanel", transform.Find("AmingPanel").GetComponent<UIAming>());
        uis.Add("GameoverPanel", transform.Find("GameoverPanel").GetComponent<UIGameover>());

        foreach (KeyValuePair<string, UIShow> ui in uis) 
        {
            ui.Value.Initialize();
        }

        isactive = true;
    }
	
	// Update is called once per frame
    // (임시) 지난 시간 설정
    // (임시) 현재 스피드 설정
    // (임시) 현재 라운드 설정
	void Update () {
        if (isactive)
        {
            passedTime += Time.deltaTime;
            lbl_timer.text = string.Format("Time: {0:f2}", passedTime);
            lbl_speed.text = string.Format("Speed: {0:f2}", GameModeData.getInstance.player.Playerdata.Movespeed);
            lbl_round.text = string.Format("Round: {0:f0}", GameModeData.getInstance.Currentround);
        }
    }

    // 상황에 따라 UI 보여주기
    public void ShowClose_Panel(string target, bool setactive)
    {
        if (setactive)
        {
            uis[target].Show();
        }
        else
        {
            uis[target].UnShow();
        }
    }
}