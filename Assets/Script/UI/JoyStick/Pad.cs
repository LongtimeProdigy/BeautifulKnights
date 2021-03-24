using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour {

    private UISprite focus;
    private int focussize1;
    private int focussize2;
    private bool ispressed = false;

    public void Initialize()
    {
        focus = transform.parent.transform.Find("Focus").GetComponent<UISprite>();
        focussize1 = focus.width;
        focussize2 = (int)(focus.width * 0.6f);
    }

    // 임시 ... 버튼 클릭으로 포커스 키 조절
    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            ispressed = true;
            OnPress(ispressed);
        }
        else
        {
            ispressed = false;
            OnPress(ispressed);
        }
    }

    void OnPress(bool _ispressed)
    {
        if (_ispressed)
        {
            focus.SetDimensions(focussize2, focussize2);
            this.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        }
        else
        {
            focus.SetDimensions(focussize1, focussize1);
            this.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void UnShow()
    {
        gameObject.SetActive(false);
        this.transform.localScale = new Vector3(1, 1, 1);
        ispressed = false;

        GameMode.getInstance.SendToServerBeforeAttack(ispressed);
    }
}
