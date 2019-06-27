using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// State object for receiving data from remote device.  
public class StateObject
{
    // Client socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 256;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new StringBuilder();
}

public class ClientManager : MonoBehaviour
{
    // The port number for the remote device.  
    private const int port = 2738;

    // ManualResetEvent instances signal completion.  
    private static ManualResetEvent connectDone = new ManualResetEvent(false);
    private static ManualResetEvent sendDone = new ManualResetEvent(false);
    private static ManualResetEvent receiveDone = new ManualResetEvent(false);

    // The response from the remote device.  
    private static String response = String.Empty;
    private static Socket sockClient;

    private UInt16[] text = new UInt16[32];
    private String query = "insert into myemployees(firstName, lastName, email, phone) values('말싸미', '달아', 'RCF', '010-0000-0010');";
    private String query2;

    private static StringBuilder textBuilder = new StringBuilder();
    [SerializeField] InputField inputField;
    [SerializeField] Text textArea;
    private static Text _textArea;
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        InitClient();
        _textArea = textArea;
    }

    void OnApplicationQuit()
    {
        CleanUp();
    }

    public static void SetTextArea(String message)
    {
        _textArea.text = message;
        textBuilder.Clear();
    }
    public void InitClient()
    {
        // Connect to a remote device.  
        try
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("106.242.203.69");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            sockClient = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            sockClient.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), sockClient);
            connectDone.WaitOne();

            // Send test data to the remote device.  
            //Send(sockClient, query);
            Debug.Log("send : " + query);
            // Receive the response from the remote device.  
            //Receive(sockClient);
            StartCoroutine(ReceiveRoutine(sockClient));
            // Write the response to the console.  
            Debug.Log("Response received : " + response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public void SendMessage()
    {
        Send(sockClient, inputField.text);
    }

    IEnumerator ReceiveRoutine(Socket sock)
    {
        while (true)
        {
            Receive(sock);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void CleanUp()
    {
        // Release the socket.  
        //sockClient.Shutdown(SocketShutdown.Both);
        sockClient.Close();
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.  
            client.EndConnect(ar);

            Console.WriteLine("Socket connected to {0}",
                client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.  
            connectDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void Receive(Socket client)
    {
        try
        {
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.  
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            //Debug.Log("리시브 콜백 콜");
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.  
                state.sb.Append(Encoding.GetEncoding("euc-kr").GetString(state.buffer, 0, bytesRead));
                SetTextArea(textBuilder.Append(state.sb).ToString());
                // Get the rest of the data.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                //Debug.Log("데이터 담는다");
            }
            else
            {
                // All the data has arrived; put it in response.  
                if (state.sb.Length > 1)
                {
                    response = state.sb.ToString();
                }
                
                // Signal that all bytes have been received.  
                receiveDone.Set();
                //Debug.Log("데이터 다 담았다");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void Send(Socket client, String data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.GetEncoding("euc-kr").GetBytes(data);

        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), client);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            // Signal that all bytes have been sent.  
            sendDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}