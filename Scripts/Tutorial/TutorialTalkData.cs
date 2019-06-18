using UnityEngine;
using System.Collections.Generic;
using System;

public enum ButtonType{
	RoundRect = 0,
	Round = 1,
	None = 2
}

public enum AnimationType{
	None = 0,
	InOut = 1,
	Out = 2,
	In = 3,
	FixInOut = 4,
	FixOut = 5,
	FixIn = 6,
}

[System.Serializable]
public class TutorialTalk
{
	public string sTalkName;
	public float fDelay;
	public AnimationType eAnimType;
	public UInt16 u2ClassID;
	public UInt16 u2SubClassID;
	public Byte u1Pos;
	public bool bBGOnOff;
	public string sBGName = "0";
	public ButtonType eButtonType;
	public string sButtonID = "0";
	public string[] lstSpeechs;    
}

public class TutorialTalkData : ScriptableObject {

    public TutorialTalk[] tutorialTalks;
    
    public void AddInfo()
    {
        for(int i=0; i<tutorialTalks.Length; i++)
        {        
            List<string> data = new List<string>();
            
            data.Clear();
            data.Add(tutorialTalks[i].sTalkName);
			data.Add(tutorialTalks[i].fDelay.ToString());
			data.Add(tutorialTalks[i].eAnimType.ToString());
            data.Add(tutorialTalks[i].u2ClassID.ToString());
			data.Add(tutorialTalks[i].u2SubClassID.ToString());
            data.Add(tutorialTalks[i].u1Pos.ToString());
			data.Add(tutorialTalks[i].bBGOnOff.ToString());
			data.Add(tutorialTalks[i].sBGName);
			data.Add(tutorialTalks[i].eButtonType.ToString());
			if(tutorialTalks[i].sButtonID.ToString() != "0") DebugMgr.Log(tutorialTalks[i].sTalkName);
			data.Add(tutorialTalks[i].sButtonID.ToString());
            data.Add(tutorialTalks[i].lstSpeechs.Length.ToString());
            
            for(int j=0; j<tutorialTalks[i].lstSpeechs.Length; j++)
            {
                data.Add(tutorialTalks[i].lstSpeechs[j].Replace("\\n", "\n"));
            }

            TutorialInfoMgr.Instance.AddTalkInfo(data.ToArray());
        }
    }
}
