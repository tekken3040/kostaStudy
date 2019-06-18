using UnityEngine;
using UnityEngine.EventSystems;
using System.Text;
using System.Linq;
using System.IO;

public class FixTouchDpi : MonoBehaviour
{
    private const float inchToCm = 2.54f;
     
    [SerializeField]
    private EventSystem eventSystem = null;
         
    [SerializeField]
    private float dragThresholdCM = 0.5f;
    //For drag Threshold
         
    private void SetDragThreshold()
    {
        if (eventSystem != null)
        {
            eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
        }
    }
      
      
    void Awake()
    {
        SetDragThreshold();
    }
    //StringBuilder sb;
    //StreamWriter _sw;
    //private void Start()
    //{
    //    var sortedAll = FindObjectsOfTypeIncludingAssets(typeof(Sprite)).OrderBy(go=>Profiler.GetRuntimeMemorySize(go)).ToList();
    //    //var sortedAll = Resources.FindObjectsOfTypeAll(typeof(Sprite)).OrderBy(go=>Profiler.GetRuntimeMemorySize(go)).ToList();
    //    var sortedAll2 = FindObjectsOfTypeIncludingAssets(typeof(Texture2D)).OrderBy(go=>Profiler.GetRuntimeMemorySize(go)).ToList();
    //    sb = new StringBuilder("");
    //    int memTexture = 0;
    //    for(int i = sortedAll.Count-1;i>=0;i--)
    //    {
    //        if(!sortedAll[i].name.StartsWith("d_"))
    //        {
    //            memTexture += Profiler.GetRuntimeMemorySize(sortedAll[i]);
    //            sb.Append(typeof(Sprite).ToString());
    //            sb.Append(" Size#");
    //            sb.Append(sortedAll.Count - i);
    //            sb.Append(" : ");
    //            sb.Append(sortedAll[i].name);
    //            sb.Append(" / Instance ID : ");
    //            sb.Append(sortedAll[i].GetInstanceID());
    //            sb.Append(" / Mem : ");
    //            sb.Append(Profiler.GetRuntimeMemorySize(sortedAll[i]).ToString());
    //            sb.Append("B / Total : ");
    //            sb.Append(memTexture/1024);
    //            sb.Append("KB ");
    //            sb.Append("\n");
    //        }
    //    }
    //    memTexture = 0;
    //    for(int i = sortedAll2.Count-1;i>=0;i--)
    //    {
    //        if(!sortedAll2[i].name.StartsWith("d_"))
    //        {
    //            memTexture += Profiler.GetRuntimeMemorySize(sortedAll2[i]);
    //            sb.Append(typeof(Texture2D).ToString());
    //            sb.Append(" Size#");
    //            sb.Append(sortedAll2.Count - i);
    //            sb.Append(" : ");
    //            sb.Append(sortedAll2[i].name);
    //            sb.Append(" / Instance ID : ");
    //            sb.Append(sortedAll2[i].GetInstanceID());
    //            sb.Append(" / Mem : ");
    //            sb.Append(Profiler.GetRuntimeMemorySize(sortedAll2[i]).ToString());
    //            sb.Append("B / Total : ");
    //            sb.Append(memTexture/1024);
    //            sb.Append("KB ");
    //            sb.Append("\n");
    //        }
    //    }
    //    if(Scene.GetCurrent() != null)
    //    {
    //        #if UNITY_EDITOR
    //        _sw = new StreamWriter(Application.dataPath + "/SceneMemoryLog"+(Scene.GetCurrent().name)+".txt");
    //        #else
    //        _sw = new StreamWriter(Application.persistentDataPath + "/SceneMemoryLog"+(Scene.GetCurrent().name)+".txt");
    //        #endif		
    //        _sw.Write(sb);
    //        _sw.Flush();
    //        _sw.Close();
    //    }
    //}
}
