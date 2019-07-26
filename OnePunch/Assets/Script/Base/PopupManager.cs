using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] private Text txtMessage;
    [SerializeField] private GameObject objPanel;

    public void ShowPopup(string strMessage)
    {
        txtMessage.text = strMessage;
        objPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        objPanel.SetActive(false);
    }
}
