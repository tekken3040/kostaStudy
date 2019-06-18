using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class QuestInfoMgr : Singleton<QuestInfoMgr>
{
	public Dictionary<UInt16, AchievementTypeData>[,] QuestTypeSet;

	private Dictionary<UInt16, QuestInfo> dicQuestData;
	private Dictionary<UInt16, AchievementInfo> dicAchieveData;
    private Dictionary<UInt16, CharTrainingInfo> dicCharTrainingData;
    private Dictionary<UInt16, EquipTrainingInfo> dicEquipTrainingData;
    //#ODIN [오딘 임무 정보] 변수
    private Dictionary<UInt16, OdinMissionInfo> dicOdinMissionInfoData;

	private List<QuestDirection> lstQuestDriectionData;
	private Dictionary<string, TalkInfo> dicQuestTalkData;
	private Dictionary<Byte, TalkInfo> dicQuestCompTalkData;

	public List<AchieveItem> listAchieveItemData = new List<AchieveItem>();

	public int[,] arrayPoses;
    
	private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

	public void AddAchieveTypeInfo(string[] cols)
	{
		if (cols == null) return;
		
		AchievementTypeData info = new AchievementTypeData();
		info.Set(cols);
		info.basePos = arrayPoses [info.periodType, info.valueType];                
		QuestTypeSet[info.periodType, info.type].Add(info.typeId, info);
		arrayPoses[info.periodType, info.valueType] += info.Length;        
	}
	
	public void AddQuestInfo(string[] cols)
	{
		if (cols == null) return;
		
		QuestInfo info = new QuestInfo();
		info.Set(cols);
		dicQuestData.Add(info.u2ID, info);
	}

	public Dictionary<UInt16, QuestInfo> GetQuestList(){
		return dicQuestData;
	}

	public QuestInfo GetQuestInfo(UInt16 id){
		QuestInfo ret;
		dicQuestData.TryGetValue(id, out ret);
		return ret;
	}

	public void AddAchieveInfo(string[] cols)
	{
		if (cols == null) return;
		
		AchievementInfo info = new AchievementInfo();
		info.Set(cols);
		dicAchieveData.Add(info.u2ID, info);
	}

	public Dictionary<UInt16, AchievementInfo> GetAchieveList(){
		return dicAchieveData;
	}

	public string GetAchieveNameByRewardID(UInt16 id){
		AchievementInfo temp = dicAchieveData.FirstOrDefault (cs => cs.Value.acReward [0].u2ID == id).Value;
		if (temp != null)
			return temp.sName;

		return "";
	}

	public AchievementInfo GetAchieveInfo(UInt16 id){
		AchievementInfo ret;
		dicAchieveData.TryGetValue(id, out ret);
		return ret;
	}
    
	public void AddCharTrainingInfo(string[] cols)
	{
		if (cols == null) return;
		
		CharTrainingInfo info = new CharTrainingInfo();
		info.Set(cols);
		dicCharTrainingData.Add(info.u2ID, info);
	}    
    
    public Dictionary<UInt16, CharTrainingInfo> GetCharTrainingInfo()
    {
        return dicCharTrainingData;
    }
    
	public void AddEquipTrainingInfo(string[] cols)
	{
		if (cols == null) return;
		
		EquipTrainingInfo info = new EquipTrainingInfo();
		info.Set(cols);
		dicEquipTrainingData.Add(info.u2ID, info);
	}    
    
    public Dictionary<UInt16, EquipTrainingInfo> GetEquipTrainingInfo()
    {
        return dicEquipTrainingData;
    }

	public void AddQuestDirctionInfo(string[] cols)
	{
		if (cols == null) return;
		
		QuestDirection info = new QuestDirection();
		info.Set(cols);
		lstQuestDriectionData.Add(info);
	} 

	public QuestDirection GetQuestDirctionInfo(UInt16 id, Byte pos)
	{
		return lstQuestDriectionData.Find (cs => cs.u2QuestID == id && cs.u1DirectionPos == pos);
	}

	public void AddTalkInfo(string[] cols)
	{
		if (cols == null) return;
		
		TalkInfo info = new TalkInfo();
		info.Set(cols, false);
		dicQuestTalkData.Add(info.sTalkName, info);
	}
	
	public TalkInfo GetTalkInfo(string id){
		TalkInfo ret;
		dicQuestTalkData.TryGetValue(id, out ret);
		return ret;
	}

	public void AddCompTalkInfo(string[] cols)
	{
		if (cols == null) return;

		TalkInfo info = new TalkInfo();
		info.Set(cols, false);
		dicQuestCompTalkData.Add(Convert.ToByte(info.fDelay), info);
	}

	public TalkInfo GetCompTalkInfo(string id){
		TalkInfo ret;
		dicQuestCompTalkData.TryGetValue(Convert.ToByte(id), out ret);
		return ret;
	}
    //#ODIN [오딘 임무 정보 셋팅]
    public void AddOdinMissionInfo(string[] cols)
    {
        if (cols == null)
            return;

        OdinMissionInfo info = new OdinMissionInfo();
        info.Set(cols);
        dicOdinMissionInfoData.Add(info.u2ID, info);
    }
    //#ODIN [오딘 임무 정보 받기]
    public bool TryGetOdinMissionInfo(UInt16 u2ID, out OdinMissionInfo info)
    {
        return dicOdinMissionInfoData.TryGetValue(u2ID, out info);
    }

	public void Init()
	{	
		arrayPoses = new int[AchievementTypeData.MAX_PERIOD + 1, AchievementTypeData.MAX_VALUETYPE + 1];
		for (int i = 0; i < AchievementTypeData.MAX_PERIOD; i++)
		{
			arrayPoses[i + 1, 1] = Server.ConstDef.PartPosOfAchievementBoolBuffer[i];
			arrayPoses[i + 1, 2] = Server.ConstDef.PartPosOfAchievementU1Buffer[i];
			arrayPoses[i + 1, 3] = Server.ConstDef.PartPosOfAchievementU2Buffer[i];
			arrayPoses[i + 1, 4] = Server.ConstDef.PartPosOfAchievementU4Buffer[i];
		}

		QuestTypeSet = new Dictionary<UInt16, AchievementTypeData>[AchievementTypeData.MAX_PERIOD + 1, AchievementTypeData.MAX_TYPE + 1];
		for (int i=1; i<AchievementTypeData.MAX_PERIOD + 1; i++) {
			for(int j=1; j<AchievementTypeData.MAX_TYPE + 1; j++) {
				QuestTypeSet[i,j] = new Dictionary<ushort, AchievementTypeData>();
			}
		}
		DataMgr.Instance.LoadTable(this.AddAchieveTypeInfo, "AchievementType");

		dicQuestData = new Dictionary<ushort, QuestInfo>();
		DataMgr.Instance.LoadTable(this.AddQuestInfo, "Quest");
		dicAchieveData = new Dictionary<ushort, AchievementInfo>();
		DataMgr.Instance.LoadTable(this.AddAchieveInfo, "Achievement");
        dicCharTrainingData = new Dictionary<UInt16, CharTrainingInfo>();
        DataMgr.Instance.LoadTable(this.AddCharTrainingInfo, "CharTraining");
        dicEquipTrainingData = new Dictionary<UInt16, EquipTrainingInfo>();
        DataMgr.Instance.LoadTable(this.AddEquipTrainingInfo, "EquipTraining");
        //#ODIN [오딘 미션 데이터 셋팅]
        dicOdinMissionInfoData = new Dictionary<ushort, OdinMissionInfo>();
        DataMgr.Instance.LoadTable(this.AddOdinMissionInfo, "OdinMission");
        
        lstQuestDriectionData = new List<QuestDirection> ();
		DataMgr.Instance.LoadTable(this.AddQuestDirctionInfo, "QuestDirection");

		dicQuestTalkData = new Dictionary<string, TalkInfo> ();
		DataMgr.Instance.LoadTable(this.AddTalkInfo, "QuestTalk");
		dicQuestCompTalkData = new Dictionary<Byte, TalkInfo> ();
		DataMgr.Instance.LoadTable(this.AddCompTalkInfo, "QuestComplete");
	}

	public bool CheckRewardTime(Byte u1Start, Byte u1Retain){
		//if (DateTime.Now.Hour >= u1Start && DateTime.Now.Hour < (u1Start+u1Retain)) {
		if (Legion.Instance.ServerTime.Hour >= u1Start && Legion.Instance.ServerTime.Hour < (u1Start+u1Retain)) {
			return true;
		}

		return false;
	}
}

public class AchieveItem
{
	public Goods cAchieveReward;

	public Byte u1SmithingLevel;
	public UInt16 u2ModelID;
	public UInt16 u2Level;
	public Byte u1Completeness;
	public UInt32[] u4Stat;
	public Byte u1SkillCount;
	public Byte[] u1SkillSlot;
}