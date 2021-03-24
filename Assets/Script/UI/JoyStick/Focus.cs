using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour {

    GameModeData gamedata;

    UISprite img_focus;

    private float focusradius;

    private float bigreboundtime;
    private float bigreboundmin;
    private float bigreboundmax;

    private float smallreboundtime;
    private float smallreboundmin;
    private float smallreboundmax;

    Coroutine bigrebound;
    Coroutine smallrebound;

    public void Initialize()
    {
        gamedata = GameModeData.getInstance;

        img_focus = GetComponent<UISprite>();

        gameObject.SetActive(false);
    }

    public void Show()
    {
        PlayerData playerdata = gamedata.player.Playerdata;

        // 조준점 크기 변경
        focusradius = gamedata.focusradius;
        img_focus.width = (int)focusradius;
        img_focus.height = img_focus.width;

        // 큰 반동 최대/최소값 변경
        bigreboundtime = gamedata.bigreboundtime;
        bigreboundmin = gamedata.bigreboundmin;
        bigreboundmax = gamedata.bigreboundmax;

        // 작은 반동 최대/최소값 변경
        smallreboundtime = gamedata.smallreboundtime;
        smallreboundmin = gamedata.smallreboundmin;
        smallreboundmax = gamedata.smallreboundmax;

        gameObject.SetActive(true);

        // 큰반동/작은반동 실행
        bigrebound = StartCoroutine(BigReboudFocus());
        smallrebound = StartCoroutine(SmallReboudFocus());
    }

    public void UnShow()
    {
        StopCoroutine(bigrebound);
        StopCoroutine(smallrebound);

        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);

        gameObject.SetActive(false);
    }

    IEnumerator BigReboudFocus()
    {
        while (true)
        {
            yield return new WaitForSeconds(bigreboundtime);

            Vector3 randomposition = Random.insideUnitCircle * Mathf.Clamp(Random.Range(bigreboundmin, bigreboundmax), 5, 100);
            transform.localPosition += randomposition;
        }
    }

    IEnumerator SmallReboudFocus()
    {
        while (true)
        {
            yield return new WaitForSeconds(smallreboundtime);

            Vector3 randomposition = Random.insideUnitCircle * Mathf.Clamp(Random.Range(smallreboundmin, smallreboundmax), 1.5f, 20);
            transform.localPosition += randomposition;
        }
    }
}
