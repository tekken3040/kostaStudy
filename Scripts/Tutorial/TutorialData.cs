using UnityEngine;
using System.Collections.Generic;
using System;
public class TutorialData : ScriptableObject{
    
	public Byte u1TutorialNo;
	public Byte u1TutorialType;
	public Byte u1TutorialPart;
	public bool bSkip; 
	public UInt16 u2Screen;
    public UInt16 u2Popup;   
    
    public TutorialStructs[] lstTalks;
    
    public void AddInfo()
    {        
        List<string> data = new List<string>();
        
        data.Add(u1TutorialNo.ToString());
        data.Add(u1TutorialType.ToString());
        data.Add(u1TutorialPart.ToString());
        data.Add(bSkip ? "T" : "F");
        data.Add(u2Screen.ToString());
        data.Add(u2Popup.ToString());
        data.Add(lstTalks.Length.ToString());
        
        for(int i=0; i<lstTalks.Length; i++)
        {
            data.Add(lstTalks[i].u1Type.ToString());
            data.Add(lstTalks[i].sName);
        }        
        
        TutorialInfoMgr.Instance.AddTutorialInfo(data.ToArray());
    }
}
