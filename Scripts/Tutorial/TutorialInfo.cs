using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TutorialInfo {

	public Byte u1TutorialNo;
	public Byte u1TutorialType;
	public Byte u1TutorialPart;
	public string sAnalyticsEventCode;
	public bool bSkip; 
	public UInt16 u2Screen;
	public UInt16 u2Page;
	public Byte u1PageCount;

	public List<TutorialStructs> lstTalks;

	public Byte Set(string[] cols)
	{
		UInt16 idx = 0;
		u1TutorialNo = Convert.ToByte(cols[idx++]);
		u1TutorialType = Convert.ToByte(cols[idx++]);
		u1TutorialPart = Convert.ToByte(cols[idx++]);
		sAnalyticsEventCode = cols[idx++];
		bSkip = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		
		u2Screen = Convert.ToUInt16(cols[idx++]);
		u2Page = Convert.ToUInt16(cols[idx++]);
		u1PageCount = Convert.ToByte(cols[idx++]);

		lstTalks = new List<TutorialStructs> ();

		for (int i=0; i<u1PageCount; i++) {
			TutorialStructs temp = new TutorialStructs();
			temp.u1Type = Convert.ToByte(cols[idx++]);
			temp.sName = cols[idx++];

			lstTalks.Add (temp);
		}
		
		return u1TutorialNo;
	}
}

[System.Serializable]
public class TutorialStructs{
	public Byte u1Type;
	public string sName;
}

public class TalkInfo {
	public string sTalkName;
	public float fDelay;
	public Byte eTalkType;
	public UInt16 u2TalkPer;
	public AnimationType eAnimType;
	public UInt16 u2ClassID;
	public UInt16 u2SubClassID;
	public UInt16 u2OtherClassID1;
	public UInt16 u2OtherClassID2;
	public Byte u1Pos;
	public bool bBGOnOff;
	public string sBGName;
	public ButtonType eButtonType;
	public string sButtonID;
	public Byte u1TalkCount;
	public List<string> lstSpeechs;

	public string Set(string[] cols, bool bNotice)
	{
		UInt16 idx = 0;
		sTalkName = cols[idx++];
		fDelay = (float)Convert.ToDouble(cols[idx++]);

		if (bNotice) {
			eTalkType = Convert.ToByte (cols [idx++]);
			u2TalkPer = Convert.ToUInt16 (cols [idx++]);
		}

		eAnimType = (AnimationType)Enum.Parse(typeof(AnimationType), cols[idx++]);

//		DebugMgr.LogError(sTalkName);

		u2ClassID = Convert.ToUInt16(cols[idx++]);
		u2SubClassID = Convert.ToUInt16(cols[idx++]);
		u2OtherClassID1 = Convert.ToUInt16(cols[idx++]);
		u2OtherClassID2 = Convert.ToUInt16(cols[idx++]);
		u1Pos = Convert.ToByte(cols[idx++]);

		bBGOnOff = cols[idx][0] == 'T' || cols[idx][0] == 't';
		idx++;

		sBGName = cols[idx++];

		eButtonType = (ButtonType)Enum.Parse(typeof(ButtonType), cols[idx++]);

		sButtonID = cols[idx++];

		u1TalkCount = Convert.ToByte(cols[idx++]);
		
		lstSpeechs = new List<string> ();
		
		for (int i=0; i<u1TalkCount; i++) {
			lstSpeechs.Add (cols[idx++].Replace("\\n", "\n"));
		}
		
		return sTalkName;
	}
}

public class TalkCharacter {
	public UInt16 u2ClassID;
	public bool bUseLoading;
	public string sName;
	public UInt16 u2ImageID;
	public string sDescription;
	
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
	
		u2ClassID = Convert.ToUInt16(cols[idx++]);
		bUseLoading = cols[idx][0] == 'T' || cols[idx][0] == 't';
		idx++;
		sName = cols[idx++];
		u2ImageID = Convert.ToUInt16(cols[idx++]);

		sDescription = cols[idx++];

		sDescription = sDescription.Replace("\\n", "\n");
		
		return u2ClassID;
	}
}

public class TutorialTextData {

	public string keyName;
	public string bg;
	public string text;

	public string Set(string[] cols)
	{
		UInt16 idx = 0;

		keyName = cols [idx++];
		bg = cols [idx++];
		text = cols [idx++];

		return keyName;
	}
}