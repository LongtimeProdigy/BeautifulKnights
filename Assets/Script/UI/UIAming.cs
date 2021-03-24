using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAming : UIShow {

    private UISprite img_joystick;
    private UISprite img_controller;
    private UILabel lbl_joystickcount;
    private SphereCollider joystickcollider;
    private JoyStick joystick;
    public JoyStick Joystick
    {
        get { return joystick; }
    }

    private UISprite img_pad;
    private UILabel lbl_padcount;
    private SphereCollider padcollider;
    private Pad pad;

    private Focus focus;

    private UILabel lbl_playerparring;
    private UILabel lbl_playerhitname;
    private UILabel lbl_playerdamage;
    private UILabel lbl_playerdrain;

    private UILabel lbl_enermyparring;
    private UILabel lbl_enermyhitname;
    private UILabel lbl_enermydamage;
    private UILabel lbl_enermydrain;

    private int beforeamingcount;

    public override void Initialize()
    {
        img_joystick = transform.Find("Joystick_Sprite").GetComponent<UISprite>();
        img_controller = transform.Find("Joystick_Sprite/Controller_Sprite").GetComponent<UISprite>();
        lbl_joystickcount = transform.Find("Joystick_Sprite/Count_Label").GetComponent<UILabel>();
        joystickcollider = transform.Find("Joystick_Sprite").GetComponent<SphereCollider>();
        joystick = transform.Find("Joystick_Sprite").GetComponent<JoyStick>();

        img_pad = transform.Find("Pad_Sprite").GetComponent<UISprite>();
        lbl_padcount = transform.Find("Pad_Sprite/Count_Label").GetComponent<UILabel>();
        padcollider = transform.Find("Pad_Sprite").GetComponent<SphereCollider>();
        pad = transform.Find("Pad_Sprite").GetComponent<Pad>();

        focus = transform.Find("Focus").GetComponent<Focus>();

        lbl_playerparring = transform.Find("Player/Parring_Label").GetComponent<UILabel>();
        lbl_playerhitname = transform.Find("Player/HitName_Label").GetComponent<UILabel>();
        lbl_playerdamage = transform.Find("Player/Damage_Label").GetComponent<UILabel>();
        lbl_playerdrain = transform.Find("Player/Drain_Label").GetComponent<UILabel>();

        lbl_enermyparring = transform.Find("Enermy/Parring_Label").GetComponent<UILabel>();
        lbl_enermyhitname = transform.Find("Enermy/HitName_Label").GetComponent<UILabel>();
        lbl_enermydamage = transform.Find("Enermy/Damage_Label").GetComponent<UILabel>();
        lbl_enermydrain = transform.Find("Enermy/Drain_Label").GetComponent<UILabel>();

        beforeamingcount = (int)GameModeData.getInstance.Timebeforeaming;

        // 조이스틱 초기화
        joystick.Initialize();
        pad.Initialize();
        focus.Initialize();

        lbl_playerparring.gameObject.SetActive(false);
        lbl_playerhitname.gameObject.SetActive(false);
        lbl_playerdamage.gameObject.SetActive(false);
        lbl_playerdrain.gameObject.SetActive(false);

        lbl_enermyparring.gameObject.SetActive(false);
        lbl_enermyhitname.gameObject.SetActive(false);
        lbl_enermydamage.gameObject.SetActive(false);
        lbl_enermydrain.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);

        lbl_joystickcount.gameObject.SetActive(true);
        lbl_padcount.gameObject.SetActive(true);

        img_joystick.color = Color.black;
        img_controller.color = Color.black;
        img_pad.color = Color.black;

        lbl_joystickcount.text = string.Format("{0}", beforeamingcount);
        lbl_padcount.text = string.Format("{0}", beforeamingcount);

        joystick.Show();
        pad.Show();

        StartCoroutine(CountDown(beforeamingcount));
    }

    IEnumerator CountDown(int countamount)
    {
        while (countamount >0)
        {
            yield return new WaitForSeconds(1.0f);

            countamount--;

            lbl_joystickcount.text = string.Format("{0}", countamount);
            lbl_padcount.text = string.Format("{0}", countamount);
        }

        EnterAming();
    }

    public void EnterAming()
    {
        focus.Show();

        joystickcollider.enabled = true;
        padcollider.enabled = true;

        img_joystick.color = Color.white;
        img_controller.color = Color.white;
        img_pad.color = Color.white;

        lbl_joystickcount.text = string.Format("{0}", "");
        lbl_padcount.text = string.Format("{0}", "");
    }

    public override void UnShow()
    {
        joystickcollider.enabled = false;
        padcollider.enabled = false;

        joystick.UnShow();
        pad.UnShow();
        focus.UnShow();
    }

    public IEnumerator Cor_ShowSprint()
    {
        yield return new WaitForSeconds(0.5f);                      //0.5f

        // 히트 부위 생성
        lbl_playerhitname.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);                      //0.7f
        lbl_playerparring.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);                      //0.9f
        lbl_playerdamage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);                      //1.1f
        //isdrained = true;
        //drainamount = damage * player.drainamount
        //isdrained = false;

        yield return new WaitForSeconds(0.9f);                      //2.0f
        lbl_enermyhitname.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);                      //2.2f
        lbl_enermyparring.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);                      //2.4f
        lbl_enermydamage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);                      //2.6f
        lbl_enermydrain.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.9f);                      //4.5f
        lbl_playerdamage.gameObject.SetActive(false);
        lbl_playerhitname.gameObject.SetActive(false);
        lbl_playerparring.gameObject.SetActive(false);
        lbl_playerdrain.gameObject.SetActive(false);
        lbl_enermydamage.gameObject.SetActive(false);
        lbl_enermyhitname.gameObject.SetActive(false);
        lbl_enermyparring.gameObject.SetActive(false);
        lbl_enermydrain.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    public void Attacked(string hitpointname, string isparring)
    {
        lbl_playerhitname.text = hitpointname;
        lbl_playerparring.text = isparring;
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
