using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;

public class UnityNetworkAdaptor : Singleton<UnityNetworkAdaptor>
{
    public Queue<string> packetQueue = new Queue<string>();

    public void PushPacket(string _packet)
    {
        packetQueue.Enqueue(_packet);
    }
}
