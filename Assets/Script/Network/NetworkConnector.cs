using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;
using System.Net.Sockets;
using System.Text;
using HSMLibrary.Scene;
using System;
using System.Threading;


public class NetworkConnector : DontDestroyMonoSingleton<NetworkConnector>
{
    public delegate void Callback(string _responseString);
    public Callback responseCallback;

    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private TCP inner = null;

    public bool isConnected { get { return inner != null && inner.IsConnected(); } }

    private readonly string IPAddr = "52.78.67.151";

    public void ConnectToTcpServer()
    {
        inner = new TCP();
        inner.Connect(IPAddr, 9000);
    }

    public void SendToServer(object _req)
    {
        if (inner == null)
            return;

        inner.SendAsync(_req);
    }

    public void DirectDisconnectServer()
    {
        DisconnectComplete();
    }

    public void DisconnectServer()
    {
        Invoke("DisconnectComplete", 2.0f);
    }

    private void DisconnectComplete()
    {
        inner.Disconnect();
        inner = null;
    }
}