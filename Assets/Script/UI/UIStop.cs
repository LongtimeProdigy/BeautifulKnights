using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStop : UIShow {

    #region parameters
    GameModeData gamedata;

    PlayerData playerdata;
    PlayerData enermydata;

    private GameObject img_BG;
    private UILabel lbl_round;

    private UISprite img_playerBG;
    private UISprite img_playername;
    private UILabel lbl_playername;
    private UILabel lbl_playerchanged;
    private GameObject playerlanceBG;
    private UILabel lbl_playerlancename;
    private UISprite img_playerlancethumbnail;
    private UISprite img_playerlanceskillthumbnail;
    private UILabel lbl_playerlanceskilldesc;

    private UISprite img_enermyBG;
    private UISprite img_enermyname;
    private UILabel lbl_enermyname;
    private UILabel lbl_enermychanged;
    private GameObject enermylanceBG;
    private UILabel lbl_enermylancename;
    private UISprite img_enermylancethumbnail;
    private UISprite img_enermylanceskillthumbnail;
    private UILabel lbl_enermylanceskilldesc;

    enum stats
    {
        Power,
        Destructive,
        Penetration,
        Parrying,
        Weight
    }
    private UISprite[] img_playercurrentstats = new UISprite[5];
    private UILabel[] lbl_playercurrentstats = new UILabel[5];
    private UISprite[] img_playercomparestats = new UISprite[5];
    private UILabel[] lbl_playercomparestats = new UILabel[5];

    private UISprite[] img_enermycurrentstats = new UISprite[5];
    private UILabel[] lbl_enermycurrentstats = new UILabel[5];
    private UISprite[] img_enermycomparestats = new UISprite[5];
    private UILabel[] lbl_enermycomparestats = new UILabel[5];

    private bool isplayerchanged = false;
    public bool Isplayerchanged
    {
        get { return isplayerchanged; }
        set { isplayerchanged = value; }
    }
    private bool isenermychanged = false;
    public bool Isenermychanged
    {
        get { return isenermychanged; }
        set { isenermychanged = value; }
    }

    private int playerbeforelance = -1;
    public int Playereforelance
    {
        get { return playerbeforelance; }
        set { playerbeforelance = value; }
    }
    private int playercurrentlance = -1;
    public int Playercurrentlance
    {
        get { return playercurrentlance; }
        set { playercurrentlance = value; }
    }
    private int enermybeforelance = -1;
    public int Enermybeforelance
    {
        get { return enermybeforelance; }
        set { enermybeforelance = value; }
    }
    private int enermycurrentlance = -1;
    public int EnermyCurrentlance
    {
        get { return enermycurrentlance; }
        set { enermycurrentlance = value; }
    }
    #endregion

    public override void Initialize()
    {
        gamedata = GameModeData.getInstance;

        playerdata = gamedata.player.Playerdata;
        enermydata = gamedata.enermy.Playerdata;

        gameObject.SetActive(false);

        img_BG = transform.Find("Background").gameObject;

        lbl_round = transform.Find("Round_Label").GetComponent<UILabel>();

        img_playerBG = transform.Find("PlayerBGBG_Sprite").GetComponent<UISprite>();
        img_playername = transform.Find("PlayerBGBG_Sprite/Name_Sprite").GetComponent<UISprite>();
        lbl_playername = img_playername.transform.Find("Name_Label").GetComponent<UILabel>();
        lbl_playerchanged = transform.Find("PlayerBGBG_Sprite/Change_Label").GetComponent<UILabel>();
        playerlanceBG = transform.Find("PlayerBGBG_Sprite/LanceBG_Sprite").gameObject;
        lbl_playerlancename = playerlanceBG.transform.Find("LanceName_Label").GetComponent<UILabel>();
        img_playerlancethumbnail = playerlanceBG.transform.Find("Thumbnail_Sprite").GetComponent<UISprite>();
        img_playerlanceskillthumbnail = playerlanceBG.transform.Find("LanceSkillBG_Sprite/Thumbnail_Sprite").GetComponent<UISprite>();
        lbl_playerlanceskilldesc = playerlanceBG.transform.Find("LanceSkillBG_Sprite/Exp_Label").GetComponent<UILabel>();

        img_enermyBG = transform.Find("EnermyBGBG_Sprite").GetComponent<UISprite>();
        img_enermyname = transform.Find("EnermyBGBG_Sprite/Name_Sprite").GetComponent<UISprite>();
        lbl_enermyname = img_enermyname.transform.Find("Name_Label").GetComponent<UILabel>();
        lbl_enermychanged = transform.Find("EnermyBGBG_Sprite/Change_Label").GetComponent<UILabel>();
        enermylanceBG = transform.Find("EnermyBGBG_Sprite/LanceBG_Sprite").gameObject;
        lbl_enermylancename = enermylanceBG.transform.Find("LanceName_Label").GetComponent<UILabel>();
        img_enermylancethumbnail = enermylanceBG.transform.Find("Thumbnail_Sprite").GetComponent<UISprite>();
        img_enermylanceskillthumbnail = enermylanceBG.transform.Find("LanceSkillBG_Sprite/Thumbnail_Sprite").GetComponent<UISprite>();
        lbl_enermylanceskilldesc = enermylanceBG.transform.Find("LanceSkillBG_Sprite/Exp_Label").GetComponent<UILabel>();

        // 플레이어 랜스 스텟 창 초기화
        for (int i = 0; i < 5; i++)
        {
            stats stat = (stats)i;
            img_playercurrentstats[i] = playerlanceBG.transform.Find("Stat/" + stat + "/Current_Sprite").GetComponent<UISprite>();
            lbl_playercurrentstats[i] = playerlanceBG.transform.Find("Stat/" + stat + "/Current_Label").GetComponent<UILabel>();
            img_playercomparestats[i] = playerlanceBG.transform.Find("Stat/" + stat + "/Compare_Sprite").GetComponent<UISprite>();
            lbl_playercomparestats[i] = playerlanceBG.transform.Find("Stat/" + stat + "/Compare_Label").GetComponent<UILabel>();

            img_enermycurrentstats[i] = enermylanceBG.transform.Find("Stat/" + stat + "/Current_Sprite").GetComponent<UISprite>();
            lbl_enermycurrentstats[i] = enermylanceBG.transform.Find("Stat/" + stat + "/Current_Label").GetComponent<UILabel>();
            img_enermycomparestats[i] = enermylanceBG.transform.Find("Stat/" + stat + "/Compare_Sprite").GetComponent<UISprite>();
            lbl_enermycomparestats[i] = enermylanceBG.transform.Find("Stat/" + stat + "/Compare_Label").GetComponent<UILabel>();
        }

        SetFalse();
    }

    void SetFalse()
    {
        img_BG.SetActive(false);

        lbl_round.gameObject.SetActive(false);

        img_playerBG.gameObject.SetActive(false);
        lbl_playername.gameObject.SetActive(false);
        lbl_playerchanged.text = "";
        playerlanceBG.gameObject.SetActive(false);

        img_enermyBG.gameObject.SetActive(false);
        lbl_enermyname.gameObject.SetActive(false);
        lbl_enermychanged.text = "";
        enermylanceBG.gameObject.SetActive(false);
    }

    void LanceStatChange(bool isplayer, bool ischanged)
    {
        int beforelance = isplayer ? playerbeforelance : enermybeforelance;
        int currentlance = isplayer ? playercurrentlance : enermycurrentlance;

        if (!ischanged)
        {
            currentlance = beforelance;
        }

        Lance _beforelance = isplayer ? playerdata.Lances[beforelance] : enermydata.Lances[beforelance];
        Lance _lance = isplayer ? playerdata.Lances[currentlance] : enermydata.Lances[currentlance];

        UILabel lbl_lancename = isplayer ? lbl_playerlancename : lbl_enermylancename;
        UISprite img_lancethumbnail = isplayer ? img_playerlancethumbnail : img_enermylancethumbnail;

        // 선택된 창으로 창 thumbnail 업데이트
        lbl_lancename.text = _lance.Info.name;
        img_lancethumbnail.spriteName = string.Format("Icon_Lance_{0:0000}", _lance.Info.index + 1);

        // 창스킬 상태창 설정
        LanceSkill lanceskill = _lance.Skill;
        UILabel lbl_lanceskilldesc = isplayer ? lbl_playerlanceskilldesc : lbl_enermylanceskilldesc;

        lbl_lanceskilldesc.text = string.Format("[{0} Lv.{1}] {2} {3} {4}", lanceskill.Info.name, lanceskill.Stat.level, lanceskill.Info.description1, lanceskill.Stat.degree, lanceskill.Info.description2);
        // 창 스킬을 사용했는가?
        UISprite img_lanceskillthumbnail = isplayer ? img_playerlanceskillthumbnail : img_enermylanceskillthumbnail;
        if (_lance.Skill.Isused)
        {
            // 창 스킬을 사용한 상태라면 회색처리
            img_lanceskillthumbnail.spriteName = string.Format("Icon_AllSkill_{0:0000}_dis", lanceskill.Info.index + 1);
            lbl_lanceskilldesc.color = hexToColor("C1C1C1FF");
        }
        else
        {
            // 창 스킬을 사용하지 않은 상태라면 원색처리
            img_lanceskillthumbnail.spriteName = string.Format("Icon_AllSkill_{0:0000}", lanceskill.Info.index + 1);
            lbl_lanceskilldesc.color = hexToColor("33CDFFFF");
        }

        // 이전 창과 비교하여 stat창 업데이트
        for (int i = 0; i < 5; i++)
        {
            UILabel lbl_comparestats = isplayer ? lbl_playercomparestats[i] : lbl_enermycomparestats[i];
            UISprite img_comparestats = isplayer ? img_playercomparestats[i] : img_enermycomparestats[i];
            UILabel lbl_currentstats = isplayer ? lbl_playercurrentstats[i] : lbl_enermycurrentstats[i];
            UISprite img_currentstats = isplayer ? img_playercurrentstats[i] : img_enermycurrentstats[i];

            float currentstat = _lance.Stat.getstat(i);

            lbl_currentstats.text = string.Format("{0:f0}", _lance.Stat.getstat(i));
            img_currentstats.fillAmount = currentstat / _lance.Stat.getmaxstat(i);

            float diff = currentstat - _beforelance.Stat.getstat(i);
            img_comparestats.width = (int)(Mathf.Abs(diff) * (500 / 150));
            if (diff < 0.0f)
            {
                img_comparestats.color = hexToColor("FF0000FF");
                img_comparestats.transform.localPosition = new Vector3(50.0f + currentstat * (500 / 150), 0.0f, 0.0f);
                lbl_comparestats.text = string.Format("(-{0:f0})", diff);
                lbl_comparestats.color = hexToColor("FF0000FF");
            }
            else if (diff == 0.0)
            {
                img_comparestats.color = hexToColor("38CEFFFF");
                lbl_comparestats.text = "(+0)";
                lbl_comparestats.color = hexToColor("38CEFFFF");
            }
            else
            {
                img_comparestats.color = hexToColor("63FF3DFF");
                img_comparestats.transform.localPosition = new Vector3(50.0f + (currentstat - (diff)) * (500 / 150), 0.0f, 0.0f);
                lbl_comparestats.text = string.Format("({0:f0})", diff);
                lbl_comparestats.color = hexToColor("63FF3DFF");
            }
        }
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        gameObject.SetActive(true);

        StartCoroutine(Cor_Show());
    }

    IEnumerator Cor_Show()
    {
        // 라운드에 맞게 보여줄 라운드 텍스트 설정
        lbl_round.gameObject.SetActive(true);
        if (gamedata.Currentround < gamedata.Maxround)
        {
            lbl_round.text = string.Format("Round {0:f0}", gamedata.Currentround);
        }
        else if (gamedata.Currentround == gamedata.Maxround)
        {
            lbl_round.text = "Final Round";
        }
        else
        {
            lbl_round.text = "Extra Round";
        }

        yield return new WaitForSeconds(0.5f); // 0.5초대기
        lbl_round.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.75f); // 1.75초 대기 // 2.5초에 라운드 표시 제거
        lbl_round.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f); // 0.5초대기    // 플레이어와 적의 착용정보 표시 시작
        img_BG.SetActive(true);

        // 0.5초 대기
        yield return new WaitForSeconds(0.5f + 0.5f);                  //3.0-3.5
        // 플레이어 및 적 정비결과 보여주기
        img_playerBG.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);                  // 3.2 플레이어 이름 표시
        lbl_playername.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);                  // 3.4  3.4-3.9 플레이어 창 정보표시
        LanceStatChange(true, false);                           // 플레이어 창 정보업데이트
        playerlanceBG.SetActive(true);
        yield return new WaitForSeconds(0.2f);                  // 3.6 
        img_enermyBG.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);                  // 3.8 적 이름 표시
        lbl_enermyname.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);                  // 4.0  4.0-4.5 적 창 정보 표시
        LanceStatChange(false, false);
        enermylanceBG.SetActive(true);
        yield return new WaitForSeconds(0.2f);                  // 4.2  4.2-4.7 창정보변경 (4.25-4.75)
        if (isplayerchanged && gamedata.Currentround != 1)
        {
            //StartCoroutine(Cor_Changed(true));
            playerlanceBG.SetActive(false);
            LanceStatChange(true, true);
            yield return new WaitForSeconds(0.05f);
            lbl_playerchanged.text = "Changed";
            playerlanceBG.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.25f);                 // 4.45  4.45-4.95 창정보변경 (4.5-5.0)
        if (isenermychanged && gamedata.Currentround != 1)
        {
            //StartCoroutine(Cor_Changed(false));
            enermylanceBG.SetActive(false);
            LanceStatChange(false, true);
            yield return new WaitForSeconds(0.05f);
            lbl_enermychanged.text = "Changed";
            enermylanceBG.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(3.05f);                 // 7.5 까지 대기

        // 모두 비활성화
        yield return new WaitForSeconds(1.5f);                  // 9.0 UI 종료
    }

    public override void UnShow()
    {
        SetFalse();

        isplayerchanged = false;
        isenermychanged = false;
        gameObject.SetActive(false);
    }

    static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }

        return new Color32(r, g, b, a);
    }
}
