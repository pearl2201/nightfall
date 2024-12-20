using MasterServerToolkit.MasterServer;
using MasterServerToolkit.Networking;
using MasterServerToolkit.Utils;
using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;


public class NativeSocketPeer : BasePeer
{
    public Action onConnecting;

    
    private readonly NWebSocket socket;
    private Queue<byte[]> delayedMessages;
    private readonly float delay = 0.2f;

    public NativeSocketPeer(NWebSocket socket, Action onConnecting)
    {
        this.socket = socket;
        delayedMessages = new Queue<byte[]>();
        this.onConnecting = onConnecting;
    }

    public override bool IsConnected => socket.State == NativeWebSocket.WebSocketState.Open;

    public void SendDelayedMessages()
    {
        SafeCoroutine.PermanentRunner.StartCoroutine(SendDelayedMessagesCoroutine());
    }

    public IEnumerator SendDelayedMessagesCoroutine()
    {
        yield return new WaitForSecondsRealtime(delay);

        if (delayedMessages == null)
        {
            yield break;
        }

        lock (delayedMessages)
        {
            if (delayedMessages == null)
            {
                yield break;
            }

            var copy = delayedMessages;
            delayedMessages = null;

            foreach (var data in copy)
            {
                socket.Send(data);
            }
        }
    }

    public override void SendMessage(IOutgoingMessage message, DeliveryMethod deliveryMethod)
    {
        if (delayedMessages != null)
        {
            lock (delayedMessages)
            {
                if (delayedMessages != null)
                {
                    delayedMessages.Enqueue(message.ToBytes());
                    return;
                }
            }
        }

        Mst.TrafficStatistics.RegisterOpCodeTrafic(message.OpCode, message.Data.LongLength, TrafficType.Outgoing);
        socket.Send(message.ToBytes());
    }

    public override void Disconnect(string reason)
    {
        Disconnect((ushort)CloseStatusCode.Normal, reason);
    }

    public override void Disconnect(ushort code, string reason)
    {
        AsyncHelper.RunSync(() => socket.Close());
    }

    public void Connect()
    {
        onConnecting.Invoke();
        AsyncHelper.RunSync(() => socket.Connect());
    }
}


public static class AsyncHelper
{
    private static readonly TaskFactory _myTaskFactory = new
      TaskFactory(CancellationToken.None,
                  TaskCreationOptions.None,
                  TaskContinuationOptions.None,
                  TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        return AsyncHelper._myTaskFactory
          .StartNew<Task<TResult>>(func)
          .Unwrap<TResult>()
          .GetAwaiter()
          .GetResult();
    }

    public static void RunSync(Func<Task> func)
    {
        AsyncHelper._myTaskFactory
          .StartNew<Task>(func)
          .Unwrap()
          .GetAwaiter()
          .GetResult();
    }
}