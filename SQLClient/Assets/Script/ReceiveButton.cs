using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveButton : MonoBehaviour
{
    [SerializeField] ClientManager cManager;

    public void OnClickReceive()
    {
        cManager.SendMessage();
    }
}
