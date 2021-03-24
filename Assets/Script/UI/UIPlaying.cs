using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlaying : UIShow {

    private UISprite player_currenthp;          // 게임중UI - 플레이어 HP
    private GameObject player_maxhp;
    private GameObject playername;
    private UILabel lbl_playerexp;

    private UISprite enermy_currenthp;          // 게임중UI - 적 HP
    private GameObject enermy_maxhp;
    private GameObject enermyname;
    private UILabel lbl_enermyexp;

    private GameObject lbl_game;
    private GameObject lbl_set;
    private UILabel lbl_result;

    private bool isfirstshow = true;

    public override void Initialize()
    {
        player_currenthp = transform.Find("Player/PlayerBG_Sprite/PlayerHP_Sprite").GetComponent<UISprite>();
        player_maxhp = transform.Find("Player/PlayerBG_Sprite").gameObject;
        playername = transform.Find("Player/PlayerName_Sprite").gameObject;
        lbl_playerexp = transform.Find("Player/Playerexp_Label").GetComponent<UILabel>();

        enermy_currenthp = transform.Find("Enermy/EnermyBG_Sprite/EnermyHP_Sprite").GetComponent<UISprite>();
        enermy_maxhp = transform.Find("Enermy/EnermyBG_Sprite").gameObject;
        enermyname = transform.Find("Enermy/EnermyName_Sprite").gameObject;
        lbl_enermyexp = transform.Find("Enermy/Enermyexp_Label").GetComponent<UILabel>();

        lbl_game = transform.Find("Game_Label").gameObject;
        lbl_set = transform.Find("Set_Label").gameObject;
        lbl_result = transform.Find("Result_Label").GetComponent<UILabel>();

        playername.SetActive(false);
        player_maxhp.SetActive(false);
        enermyname.SetActive(false);
        enermy_maxhp.SetActive(false);
        lbl_playerexp.text = "";
        lbl_enermyexp.text = "";
        lbl_game.SetActive(false);
        lbl_set.SetActive(false);
        lbl_result.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
    }

    public void SetUI_HPBar(PlayerData playerdata, PlayerData enermydata)
    {
        player_currenthp.fillAmount = Mathf.Clamp(playerdata.Currenthp / playerdata.Maxhp, 0.0f, 1.0f);
        enermy_currenthp.fillAmount = Mathf.Clamp(enermydata.Currenthp / enermydata.Maxhp, 0.0f, 1.0f);
    }

    public override void Show()
    {
        gameObject.SetActive(true);

        if (isfirstshow)
        {
            StartCoroutine(Cor_Show());
        }
    }   

    IEnumerator Cor_Show()
    {
        yield return new WaitForSeconds(1.0f);
        playername.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        player_maxhp.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        enermyname.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        enermy_maxhp.SetActive(true);

        isfirstshow = false;
    }

    public override void UnShow()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator Cor_Player_Walk(PlayerData playerdata, bool iswalk)
    {
        // 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        if (!playerdata.isplayer)
        {
            yield return new WaitForSeconds(1.15f);
        }

        UILabel lbl_exp = playerdata.isplayer ? lbl_playerexp : lbl_enermyexp;

        if (iswalk)
        {
            lbl_exp.color = hexToColor("FFFF00");

            if (playerdata.Currentitem != -1)
            {
                Item item = playerdata.Items[playerdata.Currentitem];

                // 플레이어 장착 아이템(이름) 표시
                lbl_exp.text = item.Info.name;
                yield return new WaitForSeconds(0.75f);

                // 플레이어 장착 아이템(설명) 표시
                lbl_exp.text = string.Format("{0}", item.Info.description2);             
                yield return new WaitForSeconds(2.75f);
            }
            else
            {
                lbl_exp.text = "아이템 사용 없음";
                yield return new WaitForSeconds(0.75f + 2.75f);
            }
        }
        else
        {
            lbl_exp.color = hexToColor("5AFFD8");

            if (playerdata.Currentitem != -1)
            {
                Skill skill = playerdata.Skills[playerdata.Currentskill];

                // 플레이어 장착 스킬(이름) 표시
                lbl_exp.text = string.Format("{0} Lv.{1}",  skill.Info.name, skill.Stat.level);
                yield return new WaitForSeconds(0.75f);

                // 플레이어 장착 스킬(설명) 표시
                lbl_exp.text = string.Format("{0} {1} {2}", skill.Info.description1, skill.Stat.degree, skill.Info.description2);
                yield return new WaitForSeconds(2.75f);
            }
            else
            {
                lbl_exp.text = "스킬 사용 없음";
                yield return new WaitForSeconds(0.75f + 2.75f);
            }
        }

        if (playerdata.isplayer)
        {
            yield return new WaitForSeconds(1.15f);
        }
        lbl_exp.text = "";
    }

    public IEnumerator Cor_Result(GameModeData gamedata)
    {
        if (gamedata.player.CurrentHP > gamedata.enermy.CurrentHP)
        {
            lbl_result.text = "YOU WIN";
            lbl_result.color = hexToColor("38CEFF");
        }
        else if (gamedata.player.CurrentHP == gamedata.enermy.CurrentHP)
        {
            lbl_result.text = "DRAW";
            lbl_result.color = hexToColor("C1C1C1");
        }
        else
        {
            lbl_result.text = "YOU LOSE";
            lbl_result.color = hexToColor("FF3737");
        }

        yield return new WaitForSeconds(0.5f);
        lbl_game.SetActive(true);
        lbl_playerexp.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        lbl_set.SetActive(true);

        yield return new WaitForSeconds(0.8f);
        lbl_result.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        lbl_game.SetActive(false);
        lbl_set.SetActive(false);
        lbl_result.gameObject.SetActive(false);
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