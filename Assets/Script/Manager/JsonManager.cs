using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;

public class JsonManager : Singleton<JsonManager> {

    public void ApplyJsonToPlayer(string json, Player player)
    {
        Debug.Log(json);
    }
}
