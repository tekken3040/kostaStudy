using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class SimplePhotonChat : MonoBehaviour, IPhotonPeerListener
{
    private bool connected;
    PhotonPeer peer;
    SimplePhotonChat client;
    private void Awake()
    {
        client = new SimplePhotonChat();
        client.peer = new PhotonPeer(client, ConnectionProtocol.Tcp);
    }

    private void Start()
    {
        // connect
        client.DebugReturn(DebugLevel.INFO, "Connecting to server at 127.0.0.1:4530 using TCP");
        client.peer.Connect("106.242.203.69:2749", "ChatServer");

        // client needs a background thread to dispatch incoming messages and send outgoing messages
        StartCoroutine(UpdateLoop());
        //StartCoroutine(ChatInput());
    }

    private IEnumerator ChatInput()
    {
        while (true)
        {
            if (!client.connected)
                continue;
            // read input
            //string buffer = Console.ReadLine();
            // send to server
            //var parameters = new Dictionary<byte, object> { { 1, buffer } };
            //client.peer.OpCustom(1, parameters, true);
        }
    }

    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            client.peer.Service();
            yield return new WaitForEndOfFrame();
        }
    }

    #region IPhotonPeerListener

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(string.Format("{0}: {1}", level, message));
    }

    public void OnEvent(EventData eventData)
    {
        DebugReturn(DebugLevel.INFO, eventData.ToStringFull());
        if (eventData.Code == 1)
        {
            DebugReturn(DebugLevel.INFO, string.Format("Chat Message: {0}", eventData.Parameters[1]));
        }
    }

    public void OnMessage(object messages)
    {
        //throw new NotImplementedException();
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull());
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        if (statusCode == StatusCode.Connect)
        {
            connected = true;
        }
        switch (statusCode)
        {
            case StatusCode.Connect:
                DebugReturn(DebugLevel.INFO, "Connected");
                connected = true;
                break;
            default:
                DebugReturn(DebugLevel.ERROR, statusCode.ToString());
                break;
        }
    }

    #endregion
}
