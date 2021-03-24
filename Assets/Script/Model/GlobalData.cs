using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;

public class GlobalData : Singleton<GlobalData>
{
    public UserInfo user;

    public List<int> charList;
    public List<int> lanceList;
    public List<int> armorList;

    public List<Deck> deckList;

    public int deckId;
    public int curGold;
    public int curCash;

    public int roomId; //.. 바꿔야함

    public GlobalData()
    {
        user = new UserInfo();
    }
}
