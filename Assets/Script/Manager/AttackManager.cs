using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;

public class AttackManager : MonoSingleton<AttackManager> {

    Camera maincamera;

    PlayerData playerdata;
    PlayerData enermydata;

    UIAming uiaming;
    UISprite hitpoint;

    JoyStick joystick;

    private Vector2 randPos;
    public Vector2 RandPos
    {
        set { randPos = value; }
    }

	public void Initialize(PlayerData _playerdata, PlayerData _enermydata)
    {
        maincamera = GameObject.Find("Camera").GetComponent<Camera>();
        playerdata = _playerdata;
        enermydata = _enermydata;
        uiaming = (UIAming)UIManager.getInstance.GetUI("AmingPanel");
        hitpoint = uiaming.transform.Find("HitPoint").GetComponent<UISprite>();

        joystick = uiaming.Joystick;
        hitpoint.gameObject.SetActive(false);
    }

    public bool Attack()
    {
        Ray ray = maincamera.ScreenPointToRay(joystick.returnscreenpoint() + randPos);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 5f);
        Debug.Log(joystick.returnscreenpoint());

        hitpoint.transform.localPosition = joystick.returnscreenpoint() + randPos - new Vector2(Screen.width / 2, Screen.height / 2);
        Debug.Log((Vector2)hitpoint.transform.localPosition);
        StartCoroutine(Cor_BlinkHitPoint());

        if (Physics.Raycast(ray, out hit, 1000))
        {
            Transform objectHit = hit.transform;

            if (objectHit.name == "spear")
            {
                hitpoint.color = new Vector4(56, 206, 255, 255);

                uiaming.Attacked(objectHit.name, "Parring");
            }
            else
            {
                hitpoint.color = new Vector4(99, 255, 61, 255);

                uiaming.Attacked(objectHit.name, "");

                objectHit.gameObject.SetActive(false);
                playerdata.DestroyedArmor.Add(objectHit.name, objectHit.gameObject);
            }

            return true;
        }
        else
        {
            hitpoint.color = new Vector4(255, 0, 0, 255);

            uiaming.Attacked("Miss", "");

            return false;
        }
    }

    IEnumerator Cor_BlinkHitPoint()
    {
        hitpoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitpoint.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        hitpoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitpoint.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        hitpoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitpoint.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        hitpoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        hitpoint.gameObject.SetActive(false);
    }
}
