using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;
using System;

public class InGameSceneController : Controller
{
    private DateTime sendTime = DateTime.UtcNow;

    private bool isplayerconnected = false;
    private bool isenermyconnected = false;

    private void Awake()
    {
        StartCoroutine(StartPingTest());
        StartCoroutine(RecievePacket());

        RequestEnterRoom req = new RequestEnterRoom();
        req.packet = "enter";
        req.roomId = GlobalData.getInstance.roomId;
        req.deckId = GlobalData.getInstance.deckId;
        req.id = GlobalData.getInstance.user.id;
        req.uuid = PlayerPrefs.GetString("UUID");

        NetworkConnector.getInstance.SendToServer(req);
    }

    private void OnApplicationQuit()
    {
        NetworkConnector.getInstance.DirectDisconnectServer();
    }

    private IEnumerator StartPingTest()
    {
        while(true)
        {
            PingTestRequest ping = new PingTestRequest();
            ping.packet = "ping";

            NetworkConnector.getInstance.SendToServer(ping);

            sendTime = DateTime.UtcNow;

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator RecievePacket()
    {
        while(true)
        {
            Queue<string> localPacketQueue = new Queue<string>();
            lock(UnityNetworkAdaptor.getInstance.packetQueue)
            {
                int packetCnt = UnityNetworkAdaptor.getInstance.packetQueue.Count;
                if (packetCnt > 0)
                {
                    for(int i = 0; i < packetCnt; i++)
                    {
                        localPacketQueue.Enqueue(UnityNetworkAdaptor.getInstance.packetQueue.Dequeue());
                    }
                }
            }

            int localPacketCnt = localPacketQueue.Count;
            for(int i = 0; i < localPacketCnt; i++)
            {
                string packet = localPacketQueue.Dequeue();
                PacketBase packetType = JsonUtility.FromJson<PacketBase>(packet);
                switch(packetType.packet)
                {
                    case "ping":
                        {
                            double ping = (DateTime.UtcNow - sendTime).Milliseconds;
                            //Debug.Log("PING : " + ping);
                        }
                        break;
                    case "enter":
                        {
                            ResultEnterRoom res = JsonUtility.FromJson<ResultEnterRoom>(packet);
                            Debug.Log("RES : " + packet);

                            GameMode.getInstance.InitializePlayer();

                            isplayerconnected = true;
                            GameModeData gamedata = GameModeData.getInstance;
                            gamedata.player.Playerdata.Charid = res.user.deck.charId;
                            gamedata.player.Playerdata.Userid = res.user.id;
                            gamedata.player.Playerdata.MakeInventory(res.user.deck);
                            gamedata.player.SetttingName(res.user.deck);

                            if (isplayerconnected && isenermyconnected)
                            {
                                gamedata.Roomid = res.user.roomId;
                                GameMode.getInstance.Initialize();
                            }
                            break;
                        }
                    case "enter_other":
                        {
                            ResultEnterRoomOther res = JsonUtility.FromJson<ResultEnterRoomOther>(packet);
                            Debug.Log("RES : " + packet);

                            GameMode.getInstance.InitializeEnermy();

                            isenermyconnected = true;
                            GameModeData gamedata = GameModeData.getInstance;
                            gamedata.enermy.Playerdata.Charid = res.user.deck.charId;
                            gamedata.enermy.Playerdata.Userid = res.user.id;
                            gamedata.enermy.Playerdata.MakeInventory(res.user.deck);
                            gamedata.enermy.SetttingName(res.user.deck);

                            if (isplayerconnected && isenermyconnected)
                            {
                                gamedata.Roomid = res.user.roomId;
                                GameMode.getInstance.Initialize();
                            }
                            break;
                        }

                    case "focus_check":
                        {
                            ResultBeforeAttack res = JsonUtility.FromJson<ResultBeforeAttack>(packet);
                            Debug.Log("RES : " + packet);

                            AttackManager.getInstance.RandPos = res.randPos;
                            break;
                        }
                    /* Work Battle Packet - by Daru */
                    case "battle":
                        {
                            ResultBattle battlePacket = packetType as ResultBattle;

                            /* TODO :> Battle 데이터 받은 이후
                             * Update user hp
                             * Update user condition
                             * Update user's all armor durability(= armor hp)
                             * Update user available skill
                             * Update user available item
                             * Update user available lance skill
                             */

                            break;
                        }
                }
            }

            yield return null;
        }
    }
}
