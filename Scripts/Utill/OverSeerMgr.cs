using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.Detectors;

public class OverSeerMgr : MonoBehaviour
{
    private bool bDetectedCheat = false;

    void Awake()
    {
        if(!this.GetComponent<SpeedHackDetector>().enabled)
        {
            this.GetComponent<SpeedHackDetector>().enabled = true;
        }
        if(!this.GetComponent<ObscuredCheatingDetector>().enabled)
        {
            this.GetComponent<ObscuredCheatingDetector>().enabled = true;
        }
        //GoogleAnalytics.StartTracking();
    }

    public void OnDetectCheat(int type)
    {
        DebugMgr.LogError("Type : " + type);
        PopupManager.Instance.AddPopup(this.gameObject, ForceQuitApplication);
        switch(type)
        {
            case 0:
                PopupManager.Instance.ShowOKPopup("스피드핵", "잡았다! 요놈!", Server.ServerMgr.Instance.ApplicationShutdown);
                return;
            case 1:
                PopupManager.Instance.ShowOKPopup("메모리 변조", "잡았다! 요놈!", Server.ServerMgr.Instance.ApplicationShutdown);
                return;
            case 2:
                PopupManager.Instance.ShowOKPopup("DLL Injection", "잡았다! 요놈!", Server.ServerMgr.Instance.ApplicationShutdown);
                return;
        }
    }

    public void ForceQuitApplication()
    {
        Server.ServerMgr.Instance.ApplicationShutdown(null);
    }
}
