using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HSMLibrary.Scene;


public class UIGameover : UIShow {

    GameObject nextround;
    GameObject gameover;

    UILabel lbl_round;

    public override void Initialize()
    {
        nextround = transform.Find("NextRound").gameObject;
        gameover = transform.Find("GameOver").gameObject;

        lbl_round = nextround.transform.Find("CurrentRound_Label").GetComponent<UILabel>();
        
        EventDelegate onclick = new EventDelegate(this, "Restart");
        transform.Find("GameOver/ButtonBG_Sprite/Restart_Button").GetComponent<UIButton>().onClick.Add(onclick);

        onclick = new EventDelegate(this, "Quit");
        transform.Find("GameOver/ButtonBG_Sprite/Quit_Button").GetComponent<UIButton>().onClick.Add(onclick);

        nextround.SetActive(false);
        gameover.SetActive(false);
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);

        GameModeData gamedata = GameModeData.getInstance;

        if (gamedata.Isgameover)
        {
            gameover.SetActive(true);
        }
        else
        {
            if (gamedata.Currentround < gamedata.Maxround)
            {
                lbl_round.text = string.Format("Round {0}", gamedata.Currentround);
            }
            else if (gamedata.Currentround == gamedata.Maxround)
            {
                lbl_round.text = "Final Round";
            }
            else
            {
                lbl_round.text = "Extra Round";
            }

            nextround.SetActive(true);
        }
    }

    public override void UnShow()
    {
        gameover.SetActive(false);
        nextround.SetActive(false);
        gameObject.SetActive(false);
    }

    void Restart()
    {
        Debug.Log("게임이 재시작됩니다.");
        // 서버에게 재시작을 알림.

        SceneHelper.getInstance.ChangeScene(typeof(InGameScene));

    }

    void Quit()
    {
        Debug.Log("게임이 종료됩니다.");
        // 서버에게 종료를 알림

        Application.Quit();
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
