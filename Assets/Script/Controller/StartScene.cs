using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Scene;

public class StartScene : Scene
{
    public override void OnActivate()
    {
        //.. TODO :: 리소스 캐싱
        StartCoroutine(Initialize());
        //SceneHelper.getInstance.ChangeScene(typeof(InGameScene));
    }

    public override void OnUpdate()
    {

    }

    public override void OnDeactivate()
    {

    }

    private IEnumerator Initialize()
    {
        NetworkConnector.getInstance.ConnectToTcpServer();
        while (!NetworkConnector.getInstance.isConnected)
            yield return null;

        SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));
    }
}
