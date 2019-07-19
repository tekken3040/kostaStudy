using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] private Text txtMessage;

    public void SetTextMessage(string strMessage)
    {
        txtMessage.text = strMessage;
    }
}
