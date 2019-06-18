using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID && !UNITY_EDITOR
public class OverSeerInfoMgr : MonoBehaviour
{
    private Dictionary<UInt16, string> dicCheatAppData;

    void Awake()
    {
        dicCheatAppData = new Dictionary<ushort, string>();

        dicCheatAppData.Add(1, "com.wood.table");
        dicCheatAppData.Add(2, "com.cih.game_cih");
        dicCheatAppData.Add(3, "com.app.gameguardian");
        dicCheatAppData.Add(4, "com.felixheller.sharedprefseditor");
    }
    void Start()
    {
        CheckCheatApp();
    }
    public string GetCheatAppInfo(UInt16 id)
    {
        string ret;
        dicCheatAppData.TryGetValue(id, out ret);

        return ret;
    }

    public void CheckCheatApp()
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass("android.content.pm.PackageManager");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
        
        int flag = pluginClass.GetStatic<int>("GET_META_DATA");
        
        AndroidJavaObject[] arrayOfAppInfo = packageManager.Call<AndroidJavaObject[]>("getInstalledApplications", flag);

        for (int i=1; i<=dicCheatAppData.Count; i++)
        {
            for(int j=0; j<arrayOfAppInfo.Length; j++)
            {
                if(packageManager.Call<AndroidJavaObject>("getApplicationInfo", dicCheatAppData[(ushort)i]) != null)
                {
                    PopupManager.Instance.ShowOKPopup("", "앱 검사 테스트", Server.ServerMgr.Instance.ApplicationShutdown);
                }
            }
        }
    }
}
#endif