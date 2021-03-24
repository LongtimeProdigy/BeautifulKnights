using UnityEngine;
using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TCP
{
    private static int ToInt(byte by)
    {
        var n = by - '0';
        if (n < 0 || n >= 10)
            throw new Exception("data error");

        return n;
    }

    private static int FourByteNumberToInt(byte[] buf, int offset)
    {
        int res = 0;
        res += ToInt(buf[0]) * 1000;
        res += ToInt(buf[1]) * 100;
        res += ToInt(buf[2]) * 10;
        res += ToInt(buf[3]);

        return res;
    }

    private static void IntToFourByteNumber(int val, byte[] buf, int offset)
    {
        if (val > 9999) { }

        for (int i = 3; i >= 0; i--)
        {
            var digit = val % 10;
            buf[offset + i] = (byte)(digit + '0');

            val /= 10;
        }
    }

    byte[] recvBuffer = new byte[4096];
    int recvLen = 0;
    Socket sock;

    Queue<object> sendQueue = new Queue<object>();
    Dictionary<object, DateTime> sendQueueTimeSample = new Dictionary<object, DateTime>();
    volatile bool sending = false;
    
    public void SendAsync(object req)
    {
        if (req == null)
            return;

        lock (sendQueue)
        {
            sendQueue.Enqueue(req);
            sendQueueTimeSample.Add(req, DateTime.UtcNow);
        }

        if (!sending)
        {
            DoSend();
        }
    }

    private void DoSend()
    {
        sending = true;

        object packet = null;

        int sendQueueCount = 0;
        double millis = 0;
        lock (sendQueue)
        {
            if (sendQueue.Count > 0)
            {
                packet = sendQueue.Dequeue();
                sendQueueCount = sendQueue.Count;

                DateTime queueTime;
                DateTime time = DateTime.UtcNow;
                if (sendQueueTimeSample.TryGetValue(packet, out queueTime))
                {
                    sendQueueTimeSample.Remove(packet);
                    millis = (time - queueTime).Milliseconds;
                }
            }
        }

        if (packet == null)
        {
            // 일어나면 안 되는 상황. sendQueue.Count == 0이다.
            sending = false;
            return;
        }

        //Debug.Log("SEND ELAPSED: " + millis + " / REMAINING COUNT: " + sendQueueCount);

        byte[] sendBuffer = new byte[4096];
        var str = JsonUtility.ToJson(packet);
        int writtenBytes = System.Text.Encoding.ASCII.GetBytes(str, 0, str.Length, sendBuffer, 4);
        IntToFourByteNumber(9999 - writtenBytes, sendBuffer, 0);

        try
        {
            sock.BeginSend(sendBuffer, 0, writtenBytes + 4, SocketFlags.None, SendCallback, this);
        }
        catch (SocketException ex)
        {
            sending = false;
            Debug.LogWarning(ex.Message);
        }
    }

    public void Connect(string _ipAddress, int _port)
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sock.NoDelay = true;
        sock.Connect(new IPEndPoint(IPAddress.Parse(_ipAddress), _port));

        sock.BeginReceive(recvBuffer, recvLen, recvBuffer.Length - recvLen, SocketFlags.None, RecvCallback, this);
    }

    public bool IsConnected()
    {
        return sock.Connected;
    }

    public void Disconnect()
    {
        sock.Close();
        sock = null;
    }

    const int kHeaderSize = 4;

    void OnSendComplete(IAsyncResult ar)
    {
        try
        {
            sock.EndSend(ar);
        }
        catch (SocketException e)
        {
            Debug.LogWarning(e.Message);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        int count = 0;
        lock (sendQueue)
        {
            count = sendQueue.Count;
        }

        if (count > 0)
        {
            DoSend();
        }
        else
        {
            sending = false;
        }
    }

    private void ProcessPacket(byte[] buf)
    {
        var str = System.Text.Encoding.ASCII.GetString(buf);
        Console.WriteLine("Recv: " + str);

        UnityNetworkAdaptor.getInstance.PushPacket(str);
        // => packet queue로 보내자
    }

    private void OnReceive(IAsyncResult ar)
    {
        try
        {
            int recvBytes = sock.EndReceive(ar);

            recvLen += recvBytes;
            Console.WriteLine("RecvBytes: " + recvBytes);

            while (recvLen >= kHeaderSize)
            {
                int payloadLen = FourByteNumberToInt(recvBuffer, recvLen);
                payloadLen = 9999 - payloadLen;
                int packetLen = kHeaderSize + payloadLen;

                if (recvLen >= packetLen)
                {
                    // 패킷 페이로드를 뜯어낸다.
                    //Debug.Log("RecvPacket: " + payloadLen);

                    // TODO: 메모리 할당 최적화
                    var buffer = new byte[payloadLen];
                    //var payloadSpan = new Span<byte>(buffer, 4, payloadLen);
                    Array.Copy(recvBuffer, 4, buffer, 0, payloadLen);

                    //
                    ProcessPacket(buffer);

                    // 남은 부분 처리.
                    // TODO: 매 패킷마다 버퍼를 교체하기보다, 버퍼가 꽉 차면 교체하는 식으로 최적화 필요.
                    if (recvLen != packetLen)
                    {
                        Array.Copy(recvBuffer, packetLen, recvBuffer, 0, recvLen - packetLen);
                    }

                    recvLen -= packetLen;
                }
                else
                {
                    break;
                }
            }

            sock.BeginReceive(recvBuffer, recvLen, recvBuffer.Length - recvLen, SocketFlags.None, RecvCallback, this);
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.WouldBlock)
            {
                sock.BeginReceive(recvBuffer, recvLen, recvBuffer.Length - recvLen, SocketFlags.None, RecvCallback, this);
            }
            else
            {
                Debug.LogWarning(e.Message);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        var pro = (TCP)ar.AsyncState;
        pro.OnSendComplete(ar);
    }

    private static void RecvCallback(IAsyncResult ar)
    {
        var pro = (TCP)ar.AsyncState;
        pro.OnReceive(ar);
    }
}
