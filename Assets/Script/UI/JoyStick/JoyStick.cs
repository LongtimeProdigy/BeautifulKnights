using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{
    GameModeData gamemodedata;
    //private Camera maincamera;

    private UISprite focus;
    private GameObject controller;
    private Vector2 controllerposition;

    private float focusmovespeed = 100;

    private float limitradius;

    bool ispressed = false;
    int index;

    float Interpolation;

    public void Initialize()
    {
        focus = transform.parent.transform.Find("Focus").GetComponent<UISprite>();
        controller = transform.Find("Controller_Sprite").gameObject;
        limitradius = (GetComponent<UISprite>().localSize.x / 2);
        Interpolation = (float)Screen.width / (float)transform.root.GetComponent<UIRoot>().manualWidth;
        controllerposition = (Vector2)this.transform.localPosition * Interpolation - new Vector2(-Screen.width / 2, -Screen.height / 2);

        GetComponent<SphereCollider>().enabled = false;

        // (임시) 게임모드데이터
        gamemodedata = GameModeData.getInstance;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // 조준점 속도 변경
        focusmovespeed = gamemodedata.focusmovespeed;
        focusmovespeed = Mathf.Clamp(focusmovespeed, 10, 300);
    }

    public void UnShow()
    {
        ispressed = false;
        UnMoved();
        gameObject.SetActive(false);
    }

    void OnPress(bool _ispressed)
    {
        if (_ispressed)
        {
            ispressed = true;
        }
        else
        {
            ispressed = false;
        }
    }

    void Update()
    {
        if (ispressed)
        {
            Moved();
        }
        else
        {
            UnMoved();
        }

        // 조준 한계선을 넘어가면 조준한계선으로 clamp하고 플레이어에게 조준지점을 1~9사이로 알려줌
        if (focus)
        {
            focus.transform.localPosition = Vector2.ClampMagnitude(focus.transform.localPosition, limitradius);

            float verticallimit = limitradius / 3;
            if (focus.transform.localPosition.x <= verticallimit)
            {
                if (focus.transform.localPosition.y <= verticallimit)
                {

                }
            }
            gamemodedata.player.Playerdata.Attackpoint = 1;
        }
    }

    void Moved()
    {
#if UNITY_EDITOR
        Vector2 mouseposition = (Vector2)Input.mousePosition;
        Vector2 normaldirection = (controllerposition - mouseposition).normalized;
        float distance = Vector2.Distance(controllerposition, mouseposition) / Interpolation;
#else
        Vector2 dist = colar - Input.GetTouch(index).position;
        Vector2 distn = dist.normalized;
        float dis = Vector2.Distance(colar, Input.GetTouch(index).position) / Interpolation;
#endif

        if (distance < limitradius)
        {
            controller.transform.localPosition = -normaldirection * distance;
        }
        else
        {
            controller.transform.localPosition = -normaldirection * limitradius;
        }

        // 포커스키를 움직인 방향으로 이동
        focus.transform.localPosition += (Vector3)(-normaldirection) * focusmovespeed * Time.deltaTime;
    }

    void UnMoved()
    {
        if (controller)
        {
            controller.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    public Vector2 returnscreenpoint()
    {
        return focus.transform.localPosition * Interpolation - new Vector3(-Screen.width / 2, -Screen.height / 2, 0);
    }
}