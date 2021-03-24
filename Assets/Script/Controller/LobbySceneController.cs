using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;
using HSMLibrary.Extension;
using HSMLibrary.Scene;
//.. FIXME :: 버튼 셋, 버튼 인풋 전부 분할해야할것

public class LobbySceneController : Controller
{
    private enum LOBBY_PHASE
    {
        LOGIN,
        LOBBY,
        QUICK_BATTLE,
        MATCHING
    }

    private LOBBY_PHASE curPhase = LOBBY_PHASE.LOGIN;

    private Dictionary<LOBBY_PHASE, GameObject> menuDic = new Dictionary<LOBBY_PHASE, GameObject>();

    private QuickBattleUI quickBattleUI;

    private void Awake()
    {
        menuDic.Clear();

        GameObject uiPanel = GameObject.Find("UIPanel");

        menuDic.Add(LOBBY_PHASE.LOGIN, uiPanel.FindChild("AccountMenu"));
        menuDic.Add(LOBBY_PHASE.LOBBY, uiPanel.FindChild("LobbyMenu"));
        menuDic.Add(LOBBY_PHASE.QUICK_BATTLE, uiPanel.FindChild("QuickBattleMenu"));
        menuDic.Add(LOBBY_PHASE.MATCHING, uiPanel.FindChild("MatchPopup"));

        StartCoroutine(RecievePacket());

        quickBattleUI = menuDic[LOBBY_PHASE.QUICK_BATTLE].GetComponent<QuickBattleUI>();
        quickBattleUI.Init();

        #region SET LOGIN
        string uuid = PlayerPrefs.GetString("UUID", string.Empty);

        Debug.Log("UUID = " + uuid);
        if (uuid == string.Empty)
        {
            curPhase = LOBBY_PHASE.LOGIN;
            menuDic[curPhase].FindChild("AccountCreateButton").SetTriggerEventOnClick(this, "OnAccountCreateButtonClick");
        }
        else
        {
            OnAccountCreateButtonClick();
        }
        #endregion

        #region SET LOBBY
        GameObject lobbyMnuObj = menuDic[LOBBY_PHASE.LOBBY];
        lobbyMnuObj.FindChild("QuickBattleButton").SetTriggerEventOnClick(this, "OnQuickBattleButtonClick");
        #endregion

        #region SET QUICK BATTLE
        GameObject quickBattleMnuObj = menuDic[LOBBY_PHASE.QUICK_BATTLE];
        quickBattleMnuObj.FindChild("BackButton").SetTriggerEventOnClick(this, "OnBackButtonClick", LOBBY_PHASE.LOBBY);
        quickBattleMnuObj.FindChild("SearchButton").SetTriggerEventOnClick(this, "OnSearchButtonClick");
        #endregion

        #region SET MATCH
        GameObject matchMnuObj = menuDic[LOBBY_PHASE.MATCHING];
        matchMnuObj.FindChild("CancelButton").SetTriggerEventOnClick(this, "OnMatchCancelButtonClick");
        #endregion

        ShowMenuType();
    }

    #region COMMON
    private void ShowMenuType()
    {
        foreach (var menu in menuDic)
        {
            GameObject menuObj = menu.Value;
            menuObj.SetActive(menu.Key == curPhase);
        }
    }
    #endregion

    #region LOGIN
    private void OnAccountCreateButtonClick()
    {
        string uuid = PlayerPrefs.GetString("UUID", string.Empty);
        RequestLogin req = new RequestLogin();
        req.packet = "login";
        req.uuid = uuid;

        NetworkConnector.getInstance.SendToServer(req);
        //StartCoroutine(CoLoginRequeste(string.Empty));
    }

    private IEnumerator RecievePacket()
    {
        while (true)
        {
            Queue<string> localPacketQueue = new Queue<string>();
            lock (UnityNetworkAdaptor.getInstance.packetQueue)
            {
                int packetCnt = UnityNetworkAdaptor.getInstance.packetQueue.Count;
                if (packetCnt > 0)
                {
                    for (int i = 0; i < packetCnt; i++)
                    {
                        localPacketQueue.Enqueue(UnityNetworkAdaptor.getInstance.packetQueue.Dequeue());
                    }
                }
            }

            int localPacketCnt = localPacketQueue.Count;
            for (int i = 0; i < localPacketCnt; i++)
            {
                string packet = localPacketQueue.Dequeue();
                PacketBase packetType = JsonUtility.FromJson<PacketBase>(packet);
                switch (packetType.packet)
                {
                    case "login":
                        {
                            GlobalData instance = GlobalData.getInstance;

                            ResultLogin res = JsonUtility.FromJson<ResultLogin>(packet);
                            instance.user.id = res.id;
                            instance.charList = res.charList;
                            instance.armorList = res.armorList;
                            instance.lanceList = res.lanceList;
                            instance.curGold = res.curGold;
                            instance.curCash = res.curCash;
                            instance.deckId = res.deckId;
                            instance.deckList = res.deckList;

                            quickBattleUI.OnUpdateCharInfo(res.charInfo);

                            PlayerPrefs.SetString("UUID", res.uuid);

                            Debug.Log("RES " + packet);

                            curPhase = LOBBY_PHASE.LOBBY;
                            ShowMenuType();
                        }
                        break;
                    case "charInfo":
                        {
                            ResultCharInfo res = JsonUtility.FromJson<ResultCharInfo>(packet);

                            Debug.Log("RES " + packet);

                            quickBattleUI.OnUpdateCharInfo(res.info);
                        }
                        break;
                    case "match":
                        {
                            ResultMatch res = JsonUtility.FromJson<ResultMatch>(packet);
                            GlobalData.getInstance.roomId = res.roomId;

                            Debug.Log("RES " + packet);

                            SceneHelper.getInstance.ChangeScene(typeof(InGameScene));
                        }
                        break;
                    case "match_exit":
                        {
                            curPhase = LOBBY_PHASE.QUICK_BATTLE;
                            ShowMenuType();
                        }
                        break;
                }
            }

            yield return null;
        }
    }
    #endregion

    #region LOBBY
    private void OnQuickBattleButtonClick()
    {
        curPhase = LOBBY_PHASE.QUICK_BATTLE;
        ShowMenuType();
    }
    #endregion

    #region QUICK BATTLE
    private void OnBackButtonClick(LOBBY_PHASE _phase)
    {
        curPhase = _phase;
        ShowMenuType();
    }

    private void OnSearchButtonClick()
    {
        RequestMatch req = new RequestMatch();
        req.packet = "match";
        req.id = GlobalData.getInstance.user.id;
        req.roomId = GlobalData.getInstance.user.roomId;
        req.deckId = GlobalData.getInstance.user.deckId;

        NetworkConnector.getInstance.SendToServer(req);

        curPhase = LOBBY_PHASE.MATCHING;
        ShowMenuType();

        //SceneHelper.getInstance.ChangeScene(typeof(InGameScene));
    }
    #endregion

    #region MATCH
    private void OnMatchCancelButtonClick()
    {
        RequestMatchCancel req = new RequestMatchCancel();
        req.packet = "match_exit";
        req.id = GlobalData.getInstance.user.id;

        NetworkConnector.getInstance.SendToServer(req);
    }
    #endregion
}
