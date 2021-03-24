using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Extension;

public class QuickBattleUI : MonoBehaviour
{
    private List<GameObject> statObjList = new List<GameObject>();

    private UITexture thumbTexture = null;

    private int curCharPage = 0;

    public void Init()
    {
        thumbTexture = gameObject.FindChild("Texture (Thumb)").GetComponent<UITexture>();

        gameObject.FindChild("RightButton").SetTriggerEventOnClick(this, "OnChangeCharButtonClick", 1);
        gameObject.FindChild("LeftButton").SetTriggerEventOnClick(this, "OnChangeCharButtonClick", -1);

        CreateStatElemList();
    }

    private GameObject CreateStatElement(GameObject _prefab, GameObject _root, string _statName, Color _color)
    {
        GameObject elem = Object.Instantiate(_prefab);
        _root.AttachChild(elem);

        UILabel namLabel = elem.FindChild("Label (StatName)").GetComponent<UILabel>();
        namLabel.color = _color;
        namLabel.text = _statName;

        UILabel statValLabel = elem.FindChild("Label (StatValue)").GetComponent<UILabel>();
        statValLabel.color = _color;

        UISprite progressBarSpr = elem.FindChild("Sprite (GaugeFG)").GetComponent<UISprite>();
        progressBarSpr.color = _color;

        return elem;
    }

    private void CreateStatElemList()
    {
        GameObject list = gameObject.FindChild("StatList");
        list.RemoveChilds();

        string[] statNames = new string[6] { "체력", "근력", "유연성", "순발력", "집중력", "균형감각" };

        GameObject prefab = Resources.Load("Prefabs/UI/StatInfoElement", typeof(GameObject)) as GameObject;
                
        GameObject elemObj = CreateStatElement(prefab, list, statNames[0], new Color(99.0f / 255.0f, 1.0f, 61.0f / 255.0f));
        statObjList.Add(elemObj);

        Color blueColor = new Color(56.0f / 255.0f, 206.0f / 255.0f, 1.0f);

        for (int i = 1; i < 6; i++)
        {
            elemObj = CreateStatElement(prefab, list, statNames[i], blueColor);
            statObjList.Add(elemObj);
        }

        var grid = list.GetComponent<UIGrid>();
        grid.Reposition();
    }

    public void OnUpdateCharInfo(CharacterDataInfo _info)
    {
        //.. 매번 로드하면 최적화 이슈 발생
        thumbTexture.mainTexture = Resources.Load(string.Format("UI/RawTexture/CharThumb/Image_UI_200{0}", _info.id + 3), typeof(Texture2D)) as Texture2D;

        UILabel charNameLabel = gameObject.FindChild("Label (CharName)").GetComponent<UILabel>();
        charNameLabel.text = _info.name;

        OnUpdateCharInfo(0, _info.maxHP, 300);
        OnUpdateCharInfo(1, _info.strength, 150);
        OnUpdateCharInfo(2, _info.flexibility, 150);
        OnUpdateCharInfo(3, _info.improvisation, 150);
        OnUpdateCharInfo(4, _info.concentration, 150);
        OnUpdateCharInfo(5, _info.balancesense, 150);

        GlobalData.getInstance.user.deckId = _info.id;
    }

    private void OnUpdateCharInfo(int _index, float _value, int _max)
    {
        GameObject statObj = statObjList[_index];
        UILabel statValLabel = statObj.FindChild("Label (StatValue)").GetComponent<UILabel>();
        statValLabel.text = string.Format("{0}", Mathf.RoundToInt(_value));

        Transform progressBar = statObj.FindChild("Sprite (GaugeFG)").transform;
        Vector3 size = progressBar.localScale;
        size.x = _value / _max;
        progressBar.localScale = size;
    }

    private void OnChangeCharButtonClick(int _changeId)
    {
        curCharPage += _changeId;
        if (curCharPage < 0)
            curCharPage = 1;

        if (curCharPage > 1)
            curCharPage = 0;

        RequestCharInfo req = new RequestCharInfo();
        req.packet = "charInfo";
        req.charId = curCharPage;

        NetworkConnector.getInstance.SendToServer(req);
    }
}
