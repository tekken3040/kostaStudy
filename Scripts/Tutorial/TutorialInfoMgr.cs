using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
	
public class TutorialInfoMgr : Singleton<TutorialInfoMgr>
{
	
	private Dictionary<Byte, TutorialInfo> dicTutorialData;
	private Dictionary<string, TutorialTextData> dicStoryData;
	private Dictionary<string, TalkInfo> dicTalkData;
	private Dictionary<string, TalkInfo> dicNoticeData;
	private Dictionary<UInt16, TalkCharacter> dicTalkCharacterData;
	private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}
	
	public void AddTutorialInfo(string[] cols)
	{
		if (cols == null) return;
		
		TutorialInfo info = new TutorialInfo();
		info.Set(cols);
		//DebugMgr.Log (info.u1TutorialType + "-" + info.u1TutorialNo);
		dicTutorialData.Add(info.u1TutorialNo, info);
	}
	
	public TutorialInfo GetTutorialInfo(Byte no){
		TutorialInfo ret;
		dicTutorialData.TryGetValue(no, out ret);
		return ret;
	}

	public Byte GetTutorialInfoByScreen(Byte type, Byte part, UInt16 screen, UInt16 popup){
		Byte Key = dicTutorialData.FirstOrDefault(cs => cs.Value.u1TutorialType == type && cs.Value.u1TutorialPart == part && cs.Value.u2Screen == screen && cs.Value.u2Page == popup).Key;
		return Key;
	}

	public Byte GetTutorialInfoByScreen(Byte type, Byte part, UInt16 screen){
		Byte Key = dicTutorialData.FirstOrDefault(cs => cs.Value.u1TutorialType == type && cs.Value.u1TutorialPart == part && cs.Value.u2Screen == screen).Key;
		return Key;
	}

	public Byte GetNextTutorialPart(TutorialInfo info){
		Byte Key = dicTutorialData.FirstOrDefault(cs => cs.Value.u1TutorialType == info.u1TutorialType && cs.Value.u1TutorialPart == info.u1TutorialPart+1).Key;
		return Key;
	}

	public bool CheckLast(TutorialInfo info){
		if(dicTutorialData.FirstOrDefault(cs => cs.Value.u1TutorialType == info.u1TutorialType
		                                  && cs.Value.u1TutorialPart > info.u1TutorialPart).Equals(default(Dictionary<Byte, TutorialInfo>)) )
		return true;

		return false;
	}
	
//	public void AddStoryInfo(TutorialTextData tData)
//	{
//		if (tData == null) return;
//		string parseData = "";
//		string[] Data = tData.text.Split(';');
//		for (int i=0; i<Data.Length; i++) {
//			parseData += Data[i]+"\n";
//		}
//
//		tData.text = parseData;
//
//		dicStoryData.Add(tData.keyName, tData);
//	}

	public void AddStoryInfo(string[] cols)
	{
		if (cols == null) return;

		TutorialTextData info = new TutorialTextData();
		info.Set(cols);
		dicStoryData.Add(info.keyName, info);
	}
	
	public TutorialTextData GetStoryInfo(string id){
		TutorialTextData ret;
		dicStoryData.TryGetValue(id, out ret);
		return ret;
	}

	public void AddTalkInfo(string[] cols)
	{
		if (cols == null) return;
		
		TalkInfo info = new TalkInfo();
		info.Set(cols, false);
		dicTalkData.Add(info.sTalkName, info);
	}
	
	public TalkInfo GetTalkInfo(string id){
		TalkInfo ret;
		dicTalkData.TryGetValue(id, out ret);
		return ret;
	}

	public void AddNoticeTalkInfo(string[] cols)
	{
		if (cols == null) return;
		
		TalkInfo info = new TalkInfo();
		info.Set(cols, true);
		dicNoticeData.Add(info.sTalkName, info);
	}
	
	public TalkInfo GetNoticeTalkInfo(string id, Byte u1Type){
        
		List<KeyValuePair<string, TalkInfo>> temp = dicNoticeData.Where (cs => cs.Value.fDelay.ToString() == id && cs.Value.eTalkType == u1Type).ToList();

		if(temp.Count == 0) return null;

		if (u1Type == 1) {
			int rand = UnityEngine.Random.Range (0, 100);
			int total = 0;
			for (int i = 0; i < temp.Count; i++) {
				total += temp [i].Value.u2TalkPer;
				if (total > rand) {
					return temp [i].Value;
				}
			}
		} else if (u1Type == 2) {
			return temp [UnityEngine.Random.Range (0, temp.Count)].Value;
		}

		return null;
	}

	public TalkInfo GetGachaTalkInfo(UInt16 u2Star){

		TalkInfo temp = dicNoticeData.FirstOrDefault (cs => cs.Value.eTalkType == 3 && cs.Value.u2TalkPer == u2Star).Value;

		if (temp == null) {
			DebugMgr.LogError ("Talk null");
			return null;
		}

		return temp;
	}

	public TalkInfo GetForgeTalkInfo(UInt16 u2Star){

		TalkInfo temp = dicNoticeData.FirstOrDefault (cs => cs.Value.eTalkType == 4 && cs.Value.u2TalkPer == u2Star).Value;

		if (temp == null) {
			DebugMgr.LogError ("Talk null");
			return null;
		}

		return temp;
	}

	public TalkInfo GetNoticeTalkInfo(string id){
		return dicNoticeData [id];
	}

	public int GetTalkCharCnt(){
		return dicTalkCharacterData.Count;
	}

	public List<KeyValuePair<UInt16, TalkCharacter>> GetLoadingChars(int loadingtype){
		List<KeyValuePair<UInt16, TalkCharacter>> temp = new List<KeyValuePair<ushort, TalkCharacter>> ();
		if(loadingtype == 0)
			temp = dicTalkCharacterData.Where (cs => cs.Value.bUseLoading == true && cs.Value.u2ClassID < 200).ToList();
		else
			temp = dicTalkCharacterData.Where (cs => cs.Value.bUseLoading == true && cs.Value.u2ClassID > 200).ToList();

		return temp;
	}

	public void AddTalkCharInfo(string[] cols)
	{
		if (cols == null) return;
		
		TalkCharacter info = new TalkCharacter();
		info.Set(cols);
		dicTalkCharacterData.Add(info.u2ClassID, info);
	}
	
	public TalkCharacter GetTalkCharInfo(UInt16 id){
		TalkCharacter ret;
		dicTalkCharacterData.TryGetValue(id, out ret);
		return ret;
	}
	
	public void Init()
	{	
		dicTutorialData = new Dictionary<Byte, TutorialInfo>();
		DataMgr.Instance.LoadTable(this.AddTutorialInfo, "Tutorial");

		dicStoryData = new Dictionary<string, TutorialTextData>();
		DataMgr.Instance.LoadTable(this.AddStoryInfo, "TutorialText");

		dicTalkData = new Dictionary<string, TalkInfo>();
		DataMgr.Instance.LoadTable(this.AddTalkInfo, "TutorialTalk");

		dicNoticeData = new Dictionary<string, TalkInfo>();
		DataMgr.Instance.LoadTable(this.AddNoticeTalkInfo, "NoticeTalk");

		dicTalkCharacterData = new Dictionary<UInt16, TalkCharacter>();
		DataMgr.Instance.LoadTable(this.AddTalkCharInfo, "TalkCharacter");
        
//        UnityEngine.Object[] tutorialData = AssetMgr.Instance.AssetLoadAll("Tutorial/Data");
//        
//        for(int i=0; i<tutorialData.Length; i++)
//        {
//            TutorialData data = tutorialData[i] as TutorialData;
//            data.AddInfo();
//        }
//        
//        UnityEngine.Object[] tutorialTalk = AssetMgr.Instance.AssetLoadAll("Tutorial/Talk");
//        
//        for(int i=0; i<tutorialTalk.Length; i++)
//        {
//            TutorialTalkData data = tutorialTalk[i] as TutorialTalkData;
//            data.AddInfo();
//        }        
//        
       
        
        
//        for(int i=0; i<2; i++)
//        {
//            UnityEngine.Object tutorialText = AssetMgr.Instance.AssetLoad("Tutorial/Text/TutorialText" + (i+1) + ".asset" , typeof(TutorialTextData));
//            TutorialTextData data = tutorialText as TutorialTextData;
//			dicStoryData.Add(data.keyName, data);
//			//AddStoryInfo(data);
//        }                
	}
}

