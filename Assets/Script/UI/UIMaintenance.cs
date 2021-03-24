using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class UIMaintenance : UIShow {

    #region parameters
    private GameModeData gamedata;
    private PlayerData playerdata;

    // 버튼 클릭시 사운드 재생할 매니저
    SoundManager soundmanager;

    // 라운드 및 카운드 위젯
    private UILabel lbl_nextround;
    private UILabel lbl_round;
    private UILabel lbl_count;

    // 창 스텟 위젯
    private UISprite img_lancethumbnail;
    private UILabel lbl_lancename;
    private UISprite[] img_currentstats = new UISprite[5];
    private UILabel[] lbl_currentstats = new UILabel[5];
    private UISprite[] img_comparestats = new UISprite[5];
    private UILabel[] lbl_comparestats = new UILabel[5];

    // 창 슬롯 / 상태이상 슬롯 / 캐릭터 스킬 슬롯 / 아이템 슬롯
    private UISprite[] lanceslots = new UISprite[5];
    private UISprite[] img_lanceslots = new UISprite[5];
    private UICustomButton[] btn_lanceslots = new UICustomButton[5];
    private BoxCollider[] lanceslotcolliders = new BoxCollider[5];

    // 창 스킬 위젯
    private UISprite img_lanceskill;
    private UILabel lbl_lanceskilldesc;

    private UILabel lbl_debuff;
    private UISprite[] debuffslots = new UISprite[5];
    private UISprite[] img_debuffslots = new UISprite[5];
    private UILabel[] lbl_debuffslots = new UILabel[5];
    private UICustomButton[] btn_debuffslots = new UICustomButton[5];
    private BoxCollider[] debuffslotcolliders = new BoxCollider[5];

    private UILabel lbl_skill;
    private UISprite[] skillslots = new UISprite[5];
    private UISprite[] img_skillslots = new UISprite[5];
    private UILabel[] lbl_skillslots = new UILabel[5];
    private UICustomButton[] btn_skillslots = new UICustomButton[5];
    private BoxCollider[] skillslotcolliders = new BoxCollider[5];

    private UILabel lbl_item;
    private UISprite[] itemslots = new UISprite[5];
    private UISprite[] img_itemslots = new UISprite[5];
    private UICustomButton[] btn_itemslots = new UICustomButton[5];
    private BoxCollider[] itemslotcolliders = new BoxCollider[5];

    // 창 스텟 위젯
    enum stats
    {
        Power,
        Destructive,
        Penetration,
        Parrying,
        Weight
    }

    // 이전 창 정보(-1 = 이전에 낀 창이 없음 = 1라운드)
    private int beforelance;
    // 이번에 교체할 창/스킬/아이템/상태이상 정보
    private int currentlance = -1;
    private int currentskill = -1;
    private int currentitem = -1;
    private int currentdebuff = -1;

    // 적의 이전창 정보
    private int enermybeforelance = -1;
    #endregion 

    public override void Initialize()
    {
        gamedata = GameModeData.getInstance;
        playerdata = gamedata.player.Playerdata;
        soundmanager = SoundManager.getInstance;

        // 라운드 창 위젯 초기화
        lbl_nextround = transform.Find("InfoWindow_Round/NextRound_Label").GetComponent<UILabel>();
        lbl_round = transform.Find("InfoWindow_Round/Currentround_Label").GetComponent<UILabel>();
        lbl_count = transform.Find("InfoWindow_Round/Countdown_Label").GetComponent<UILabel>();

        // 창 위젯 초기화
        img_lancethumbnail = transform.Find("InfoWindow_Stat/LanceProfile_Sprite/Thumbnail_Sprite").GetComponent<UISprite>();
        lbl_lancename = transform.Find("InfoWindow_Stat/LanceProfile_Sprite/LanceName_Label").GetComponent<UILabel>();
        img_lanceskill = transform.Find("InfoWindow_Stat/LanceSkill_Sprite/Thumbnail_Sprite").GetComponent<UISprite>();
        lbl_lanceskilldesc = transform.Find("InfoWindow_Stat/LanceSkill_Sprite/LanceSkill_Label").GetComponent<UILabel>();

        // 랜스 스텟 창 초기화
        for (int i = 0; i < 5; i++)
        {
            stats stat = (stats)i;
            img_currentstats[i] = transform.Find("InfoWindow_Stat/Stat/" + stat + "/Current_Sprite").GetComponent<UISprite>();
            lbl_currentstats[i] = transform.Find("InfoWindow_Stat/Stat/" + stat + "/Current_Label").GetComponent<UILabel>();
            img_comparestats[i] = transform.Find("InfoWindow_Stat/Stat/" + stat + "/Compare_Sprite").GetComponent<UISprite>();
            lbl_comparestats[i] = transform.Find("InfoWindow_Stat/Stat/" + stat + "/Compare_Label").GetComponent<UILabel>();
        }

        // 각 버튼 스크립트와 델리게이트 동적 생성 및 할당
        // 버튼 생성 / 스크립트 부착 / 스크립트 설정
        // 창 슬롯 초기화
        Vector2 buttonsize = transform.Find("InfoWindow_Lance/SlotA_Sprite").GetComponent<UISprite>().localSize;
        for (int i = 0; i < 5; i++)
        {
            lanceslots[i] = transform.Find("InfoWindow_Lance/Slot" + (char)((i + 65)) + "_Sprite").GetComponent<UISprite>();
            lanceslotcolliders[i] = lanceslots[i].gameObject.AddComponent<BoxCollider>();
            lanceslotcolliders[i].size = buttonsize;
            btn_lanceslots[i] = lanceslots[i].gameObject.AddComponent<UICustomButton>();
            lanceslots[i].GetComponent<UICustomButton>().index = i;
            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = lanceslots[i].GetComponent<UICustomButton>();
            param.expectedType = typeof(int);
            param.field = "index";
            EventDelegate onclick = new EventDelegate(this, "InfoWindow_Lance_Slot_clicked");
            onclick.parameters[0] = param;
            btn_lanceslots[i].onClick.Add(onclick);
            btn_lanceslots[i].tweenTarget = lanceslots[i].gameObject;
            //lanceslot[i].GetComponent<UIButton_custom>().pressed = Color.red;

            img_lanceslots[i] = lanceslots[i].transform.Find("Thumbnail_Sprite").GetComponent<UISprite>();
        }

        // 상태이상 슬롯 초기화
        lbl_debuff = transform.Find("InfoWindow_Debuff/Exp_Label").GetComponent<UILabel>();
        for (int i = 0; i < 5; i++)
        {
            debuffslots[i] = transform.Find("InfoWindow_Debuff/Slot" + (char)((i + 65)) + "_Sprite").GetComponent<UISprite>();
            debuffslotcolliders[i] = debuffslots[i].gameObject.AddComponent<BoxCollider>();
            debuffslotcolliders[i].size = buttonsize;
            debuffslotcolliders[i].enabled = false;
            btn_debuffslots[i] = debuffslots[i].gameObject.AddComponent<UICustomButton>();
            debuffslots[i].GetComponent<UICustomButton>().index = i;
            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = debuffslots[i].GetComponent<UICustomButton>();
            param.expectedType = typeof(int);
            param.field = "index";
            EventDelegate onclick = new EventDelegate(this, "InfoWindow_Debuff_Slot_clicked");
            onclick.parameters[0] = param;
            btn_debuffslots[i].onClick.Add(onclick);
            btn_debuffslots[i].tweenTarget = debuffslots[i].gameObject;
            //characterskillslot[i].GetComponent<UIButton_custom>().pressed = Color.red;

            img_debuffslots[i] = debuffslots[i].transform.Find("Thumbnail_Sprite").GetComponent<UISprite>();
            lbl_debuffslots[i] = debuffslots[i].transform.Find("Rank_Label").GetComponent<UILabel>();
        }

        // 캐릭터 스킬 슬롯 초기화
        lbl_skill = transform.Find("InfoWindow_Skill/Exp_Label").GetComponent<UILabel>();
        for (int i = 0; i < 5; i++)
        {
            skillslots[i] = transform.Find("InfoWindow_Skill/Slot" + (char)((i + 65)) + "_Sprite").GetComponent<UISprite>();
            skillslotcolliders[i] = skillslots[i].gameObject.AddComponent<BoxCollider>();
            skillslotcolliders[i].size = buttonsize;
            btn_skillslots[i] = skillslots[i].gameObject.AddComponent<UICustomButton>();
            skillslots[i].GetComponent<UICustomButton>().index = i;
            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = skillslots[i].GetComponent<UICustomButton>();
            param.expectedType = typeof(int);
            param.field = "index";
            EventDelegate onclick = new EventDelegate(this, "InfoWindow_Skill_Slot_clicked");
            onclick.parameters[0] = param;
            btn_skillslots[i].onClick.Add(onclick);
            btn_skillslots[i].tweenTarget = skillslots[i].gameObject;
            //characterskillslot[i].GetComponent<UIButton_custom>().pressed = Color.red;

            img_skillslots[i] = skillslots[i].transform.Find("Thumbnail_Sprite").GetComponent<UISprite>();
            lbl_skillslots[i] = skillslots[i].transform.Find("Rank_Label").GetComponent<UILabel>();
        }

        // 아이템 슬롯 초기화
        lbl_item = transform.Find("InfoWindow_Item/Exp_Label").gameObject.GetComponent<UILabel>();
        for (int i = 0; i < 5; i++)
        {
            itemslots[i] = transform.Find("InfoWindow_Item/Slot" + (char)((i + 65)) + "_Sprite").GetComponent<UISprite>();
            itemslotcolliders[i] = itemslots[i].gameObject.AddComponent<BoxCollider>();
            itemslotcolliders[i].size = buttonsize;
            btn_itemslots[i] = itemslots[i].gameObject.AddComponent<UICustomButton>();
            itemslots[i].GetComponent<UICustomButton>().index = i;
            EventDelegate.Parameter param = new EventDelegate.Parameter();
            param.obj = itemslots[i].GetComponent<UICustomButton>();
            param.expectedType = typeof(int);
            param.field = "index";
            EventDelegate onclick = new EventDelegate(this, "InfoWindow_Item_Slot_clicked");
            onclick.parameters[0] = param;
            btn_itemslots[i].onClick.Add(onclick);
            btn_itemslots[i].tweenTarget = itemslots[i].gameObject;
            //itemslot[i].GetComponent<UIButton_custom>().pressed = Color.red;

            img_itemslots[i] = itemslots[i].transform.Find("Thumbnail_Sprite").GetComponent<UISprite>();
        }

        // 창슬롯/디버프슬롯/스킬슬롯/아이템슬롯 초기화
        lbl_lanceskilldesc.text = "";
        lbl_debuff.text = "";
        lbl_skill.text = "";
        lbl_item.text = "";
        for (int i = 0; i < 5; i++)
        {
            img_lanceslots[i].spriteName = string.Format("Icon_Lance_{0:0000}", playerdata.Lances[i].Info.index + 1);
            img_debuffslots[i].spriteName = string.Format("Icon_Wound_{0:0000}_dis", playerdata.Debuffs[i].Info.index + 1);
            lbl_debuffslots[i].text = "";
            img_skillslots[i].spriteName = string.Format("Icon_AllSkill_{0:0000}", playerdata.Skills[i].Info.index + 1);
            lbl_skillslots[i].text = playerdata.Skills[i].Stat.level.ToString();
            img_itemslots[i].spriteName = string.Format("Icon_ItemAS_{0:0000}", playerdata.Items[i].Info.index + 1);
        }

        // 창 스킬 창 최초 업데이트 ( 창 메인 및 스텍 창도 같이 업데이트 됨 )
        // 현재 창을 가장 첫번째로 설정
        currentlance = 0;
        beforelance = currentlance;

        // 0번 창을 자동 클릭(창 메인/창 스텟/ 창 스킬 슬롯 업데이트)
        InfoWindow_Lance_Slot_clicked(currentlance);

        this.gameObject.SetActive(false);
    }

    // 창 스킬 창 클릭 시 호출 (창 메인/창 스텍/ 창 스킬 슬롯 업데이트)
    void InfoWindow_Lance_Slot_clicked(int index)
    {
        if (currentlance == index && gamedata.Currentround > 1)
        {
            Debug.Log("지금 클릭한 창과 같은 창을 클릭했습니다.");
            // 버튼 클릭 사운드 재생 ... 같은 버튼 클릭 다른 사운드 사용예정
            soundmanager.PlayOneShotSFX("Effect_SFX_0001");
            return;
        }

        Lance lance = playerdata.Lances[index];

        // 창 정보창 업데이트
        if (!lance.Isused) 
        {
            // 버튼 클릭 사운드 재생
            soundmanager.PlayOneShotSFX("Effect_SFX_0001");

            // 이전 선택된 슬롯 원상복귀
            btn_lanceslots[currentlance].defaultColor = hexToColor("000000FF");
            lanceslots[currentlance].width = 134;
            lanceslots[currentlance].height = 134;

            // 창 사용 전 클릭 시 슬롯 업데이트(스프라이트/랭크/스킬설명업데이트)
            // 현재 슬롯 선택 설정
            // 현재 1라운드라면 이전창을 현재창으로 동기화
            currentlance = index;
            if (gamedata.Currentround == 1)
            {
                beforelance = currentlance;
            }
            
            btn_lanceslots[index].defaultColor = hexToColor("33CDFFFF");
            lanceslots[currentlance].width = 141;
            lanceslots[currentlance].height = 141;
            // 선택된 창으로 창 프로필창 업데이트
            img_lancethumbnail.spriteName = string.Format("Icon_Lance_{0:0000}", lance.Info.index + 1);
            lbl_lancename.text = lance.Info.name;

            // 창 스킬 정보창 업데이트
            LanceSkill lanceskill = lance.Skill;
            // 창 스킬을 사용했는가?
            if (lance.Skill.Isused)
            {
                // 창 스킬을 사용한 상태라면 회색처리
                img_lanceskill.spriteName = string.Format("Icon_AllSkill_{0:0000}_dis", lanceskill.Info.index + 1);
                lbl_lanceskilldesc.color = hexToColor("C1C1C1FF");
                Debug.Log("Lanceskill is used");
            }
            else
            {
                // 창 스킬을 사용하지 않은 상태라면 원색처리
                img_lanceskill.spriteName = string.Format("Icon_AllSkill_{0:0000}", lanceskill.Info.index + 1);
                lbl_lanceskilldesc.color = hexToColor("33CDFFFF");
                Debug.Log("Lanceskill is not used");
            }
            lbl_lanceskilldesc.text = string.Format("[{0} Lv.{1}] {2} {3} {4}", lanceskill.Info.name, lanceskill.Stat.level, lanceskill.Info.description1, lanceskill.Stat.degree, lanceskill.Info.description2);

            // 선택된 창으로 stat창 업데이트
            for (int i=0; i<5; i++)
            {
                UILabel label = lbl_comparestats[i];
                UISprite sprite = img_comparestats[i];

                float currentstat = lance.Stat.getstat(i);
                float beforestat = playerdata.Lances[beforelance].Stat.getstat(i);
                float diff = currentstat - beforestat;

                lbl_currentstats[i].text = string.Format("{0:f0}", currentstat);
                img_currentstats[i].fillAmount = currentstat / lance.Stat.getmaxstat(i);

                sprite.width = (int)(Mathf.Abs(diff) * (500 / 150));
                if(diff < 0.0f)
                {
                    sprite.color = hexToColor("FF0000FF");
                    sprite.transform.localPosition = new Vector3(119 + currentstat * (500f / 150f), 0.0f, 0.0f);
                    label.text = string.Format("(-{0:f0})", Mathf.Abs(diff));
                    label.color = hexToColor("FF0000FF");
                }
                else if(diff == 0.0)
                {
                    sprite.color = hexToColor("38CEFFFF");
                    label.text = "(+0)";
                    label.color = hexToColor("FFFFFFFF");
                }
                else
                {
                    sprite.color = hexToColor("63FF3DFF");
                    sprite.transform.localPosition = new Vector3(119 + (currentstat - diff) * (500f / 150f), 0.0f, 0.0f);
                    label.text = string.Format("(+{0:f0})", Mathf.Abs(diff));
                    label.color = hexToColor("63FF3DFF");
                }
            }

            Debug.Log(currentlance + "번째 창 슬롯 버튼이 클릭되었습니다.");
        }
        else
        {
            Debug.Log("이미 사용된 창을 선택하셨습니다. 다른 창을 선택해주세요.");
        }
    }

    // 상태이상 슬롯 창 클릭 시 호출
    void InfoWindow_Debuff_Slot_clicked(int index)
    {
        // 버튼 클릭 사운드 재생
        soundmanager.PlayOneShotSFX("Effect_SFX_0001");

        Debug.Log(index + "번째 상태이상 창이 클릭되었습니다.");
    }

    // 캐릭터 스킬 창 클릭 시 호출 (캐릭터 스킬 슬롯 업데이트)
    void InfoWindow_Skill_Slot_clicked(int index)
    {
        Skill skill = playerdata.Skills[index];

        // 같은 슬롯을 누르면 사용 취소
        if(index == currentskill)
        {
            // 버튼 클릭 사운드 재생 ... 취소 사운드 다른 것 사용예정
            soundmanager.PlayOneShotSFX("Effect_SFX_0001");

            // 이전 슬롯 원상복귀
            btn_skillslots[index].defaultColor = hexToColor("000000FF");
            skillslots[index].width = 134;
            skillslots[index].height = 134;

            currentskill = -1;
            lbl_skill.text = "";

            Debug.Log("캐릭터 스킬 사용을 취소합니다.");

            return;
        }

        // 이전 과 다른 슬롯 클릭 시 및 스킬을 아직 사용하지 않았다면 실행
        if (!skill.Isused)
        {
            // 버튼 클릭 사운드 재생
            soundmanager.PlayOneShotSFX("Effect_SFX_0001");

            if (currentskill >= 0)
            {
                // 이전 슬롯 원상복귀
                btn_skillslots[currentskill].defaultColor = hexToColor("000000FF");
                skillslots[index].width = 134;
                skillslots[index].height = 134;
            }
            // 현재 슬롯 선택 설정
            btn_skillslots[index].defaultColor = hexToColor("33CDFFFF");
            skillslots[index].width = 141;
            skillslots[index].height = 141;
            //캐릭터 스킬 사용 전 클릭 시(캐릭터 스킬 설명 업데이트)
            currentskill = index;
            lbl_skill.text = string.Format("[{0} Lv.{1}] {2} {3} {4}", skill.Info.name, skill.Stat.level, skill.Info.description1, skill.Stat.degree, skill.Info.description2);

            Debug.Log(currentskill + "번째 캐릭터 스킬 슬롯 버튼이 클릭되었습니다.");
        }
        else
        {
            //캐릭터 스킬 사용 후 클릭 시 아무것도 못하게 막기 또는 안내메세지 출력
            Debug.Log("이미 사용된 스킬을 선택하셨습니다. 다른 스킬을 선택해주세요.");
        }
    }

    // 아이템 창 클릭 시 호출 (아이템 슬롯 업데이트)
    void InfoWindow_Item_Slot_clicked(int index)
    {
        // 같은 슬롯을 누르면 사용 취소
        if (index == currentitem)
        {
            // 버튼 클릭 사운드 재생 ... 취소 사운드 다른 것 사용 예정
            soundmanager.PlayOneShotSFX("Effect_SFX_0001");

            // 선택했던 슬롯 원상복귀
            btn_itemslots[currentitem].defaultColor = hexToColor("000000FF");
            itemslots[currentitem].width = 134;
            itemslots[currentitem].height = 134;

            currentitem = -1;
            lbl_item.text = "";

            Debug.Log("아이템 사용을 취소합니다.");

            return;
        }

        Item item = playerdata.Items[index];

        if (!item.Isused)
        {
            // 버튼 클릭 사운드 재생
            soundmanager.PlayOneShotSFX("Effect_SFX_0001");

            if (currentitem >= 0)
            {
                // 이전 슬롯 원상복귀
                btn_itemslots[currentitem].defaultColor = hexToColor("000000FF");
                itemslots[currentitem].width = 134;
                itemslots[currentitem].height = 134;
            }
            // 현재 슬롯 선택 설정
            btn_itemslots[index].defaultColor = hexToColor("33CDFFFF");
            itemslots[index].width = 141;
            itemslots[index].height = 141;

            // 아이템 사용 전 클릭 시 (아이템 스킬 설명 업데이트)
            currentitem = index;
            lbl_item.text = string.Format("[{0}] {1}", item.Info.name, item.Info.description1);

            Debug.Log(currentitem + "번째 아이템 슬롯 버튼이 클릭되었습니다.");
        }
        else
        {
            //아이템 사용 후 클릭 시 아무것도 못하게 막기 또는 안내메세지 출력
            Debug.Log("이미 사용된 아이템을 클릭하셨습니다. 다른 아이템을 클릭해주세요.");
        }
    }

    // 정비 창 보이기
    public override void Show()
    {
        // UI 활성화(보이기)
        gameObject.SetActive(true);

        // 라운드창 설정
        if (gamedata.Currentround < gamedata.Maxround)
        {
            lbl_nextround.text = "Next Round";
            lbl_round.text = "Round " + string.Format("{0:f0}", gamedata.Currentround);
        }
        else if (gamedata.Currentround == gamedata.Maxround)
        {
            lbl_nextround.text = "Final";
            lbl_round.text = "Round";
        }
        else
        {
            lbl_nextround.text = "Extra";
            lbl_round.text = "Round";
        }

        // 이전 무기를 현재 무기와 동기화
        beforelance = currentlance;
        // 이전 캐릭터 스킬을 초기화
        currentskill = -1;
        playerdata.Currentskill = -1;
        lbl_skill.text = "";
        // 이전 아이템을 초기화
        currentitem = -1;
        playerdata.Currentitem = -1;
        lbl_item.text = "";

        // 라운드 창 카운트 다운 시작
        StartCoroutine(Cor_CountDown());
    }

    // 카운트다운 시작 코루틴
    IEnumerator Cor_CountDown()
    {
        for(int i= (int)gamedata.Timemaintenance-1; i >= 0; i--)
        {
            lbl_count.text = string.Format("{0:f0}", i);
            yield return new WaitForSeconds(1.0f);
        }
        // 정비UI 종료
    }

    public override void UnShow()
    {
        ExitMaintenanceUI();
    }

    // 정비창 종료 함수
    void ExitMaintenanceUI()
    {
        // 선택된 창/스킬/아이템 사용 (각 슬롯 비활성화 및 캐릭터 적용)
        // 플레이어에게 사용한 창/스킬/아이템/상태이상을 정지UI에게 전달
        UIStop stop = (UIStop)UIManager.getInstance.GetUI("StopPanel");
        if (beforelance != currentlance)
        {
            stop.Isplayerchanged = true;
        }
        stop.Playereforelance = beforelance;
        stop.Playercurrentlance = currentlance;
        playerdata.Currentlance = currentlance;
        if(beforelance != currentlance)
        {
            //img_lanceslots[beforelance].color = color;
            img_lanceslots[beforelance].spriteName = string.Format("Icon_Lance_{0:0000}_dis", playerdata.Lances[beforelance].Info.index + 1);
            lanceslotcolliders[beforelance].enabled = false;
            playerdata.Lances[beforelance].Isused = true;
        }
        img_lanceskill.spriteName = string.Format("Icon_AllSkill_{0:0000}_dis", playerdata.Lances[currentlance].Skill.Info.index + 1);
        lbl_lanceskilldesc.color = hexToColor("C1C1C1FF");

        // use characterskill
        playerdata.Currentskill = currentskill;
        if(currentskill != -1)
        {
            //img_skillslots[currentskill].color = color;
            img_skillslots[currentskill].spriteName = string.Format("Icon_AllSkill_{0:0000}_dis", playerdata.Skills[currentskill].Info.index + 1);
            btn_skillslots[currentskill].defaultColor = hexToColor("000000FF");
            skillslots[currentskill].width = 134;
            skillslots[currentskill].height = 134;
            lbl_skillslots[currentskill].color = hexToColor("C1C1C1FF");
            skillslotcolliders[currentskill].enabled = false;
        }

        // use item
        playerdata.Currentitem = currentitem;
        if (currentitem != -1) {
            //img_itemslots[currentitem].color = color;
            img_itemslots[currentitem].spriteName = string.Format("Icon_ItemAS_{0:0000}_dis", playerdata.Items[currentitem].Info.index + 1);
            btn_itemslots[currentitem].defaultColor = hexToColor("000000FF");
            itemslots[currentitem].width = 134;
            itemslots[currentitem].height = 134;
            itemslotcolliders[currentitem].enabled = false;
        }

        // 내정보 건네주기
        // StartCoroutine.....

        // 적 창의 정보 가져오기
        PlayerData enermydata = gamedata.enermy.Playerdata;
        enermybeforelance = enermydata.Currentlance;
        stop.Enermybeforelance = enermybeforelance;
        enermydata.Currentlance = Random.Range(0, enermydata.Lances.Length - 1);        //StartCoroutine(NetworkManager.getInstance.Cor_GetEnermy(gamedata.enermy));
        if(gamedata.Currentround == 1)
        {
            enermybeforelance = enermydata.Currentlance;
            stop.Enermybeforelance = enermybeforelance;
        }
        stop.EnermyCurrentlance = enermydata.Currentlance;
        if(enermybeforelance != enermydata.Currentlance)
        {
            stop.Isenermychanged = true;
        }

        enermydata.Currentskill = Random.Range(0, enermydata.Skills.Length - 1);
        enermydata.Currentitem = Random.Range(0, enermydata.Items.Length - 1);

        // 정비창 비활성화
        this.gameObject.SetActive(false);
    }

    static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");
        hex = hex.Replace("#", "");
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        if(hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }

        return new Color32(r, g, b, a);
    }
}
