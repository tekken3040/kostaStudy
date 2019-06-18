using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class QuestInfo {
	public const int MAX_REWARD_COUNT = 2;

	public UInt16 u2ID;
	public string sName;
	public Byte u1MainType;
	public string sDescription;
	
	public string sSummary;

	public Byte u1QuestType;
	public UInt16 u2QuestTypeID;
	public Byte u1Delemiter1;
	public Byte u1Delemiter2;
	public Byte u1Delemiter3;

	//public AchievementTypeData cType;

	public UInt16 u2MaxCount;

	public UInt32 u4RewardExp;

	public Goods[] acReward;
	public UInt16 u2Parent;

	public Goods cOpenGoods;

	public UInt16 u2ShortCut;
	public UInt16 u2ShortCutDetail;
	public UInt16 u2ShortCutChapter;
	public bool bDirection;

	public string sIconImage;
	
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		sName = cols[idx++];
		u1MainType = Convert.ToByte(cols[idx++]);
		sDescription = cols[idx++];
		sSummary = cols[idx++];

		u1QuestType = Convert.ToByte(cols[idx++]);
		u2QuestTypeID = Convert.ToUInt16(cols[idx++]);
		u1Delemiter1 = Convert.ToByte(cols[idx++]);
		u1Delemiter2 = Convert.ToByte(cols[idx++]);
		u1Delemiter3 = Convert.ToByte(cols[idx++]);

		u2MaxCount = Convert.ToUInt16(cols[idx++]);

		u4RewardExp = Convert.ToUInt32(cols[idx++]);

		acReward = new Goods[MAX_REWARD_COUNT];
		for (int i=0; i<MAX_REWARD_COUNT; i++) {
			Byte type = Convert.ToByte (cols [idx++]);
			if (type != 0){
				acReward [i] = new Goods();
				acReward [i].u1Type = type;
				acReward [i].u2ID = Convert.ToUInt16 (cols [idx++]);
				acReward [i].u4Count = Convert.ToUInt32 (cols [idx++]);
			}else{
				idx++;
				idx++;
			}
		}

		u2Parent = Convert.ToUInt16(cols[idx++]);

		cOpenGoods = new Goods(cols, ref idx);
		
		u2ShortCut = Convert.ToUInt16(cols[idx++]);
		u2ShortCutDetail = Convert.ToUInt16(cols[idx++]);
		u2ShortCutChapter = Convert.ToUInt16(cols[idx++]);
		bDirection = cols[idx] == "T" || cols[idx] == "t";
		idx++;
		sIconImage = cols[idx++];
		
		return u2ID;
	}
}

public class AchievementInfo {
	public const int MAX_REWARD_COUNT = 1;
	
	public UInt16 u2ID;
	public string sName;
	public string sDescription;
	
	public Byte u1PeriodType;
    public UInt16 u2EventID;
	public Byte u1AchievementType;
	public UInt16 u2AchievementTypeID;
	public Byte u1Delemiter1;
	public Byte u1Delemiter2;
	public Byte u1Delemiter3;

//	public AchievementTypeData cType;

	public UInt32 u4MaxCount;
	
	public Goods[] acReward;
	public UInt16 u2Parent;

	public Goods cOpenGoods;
    public Goods cCloseGoods;

	public UInt16 u2ShortCut;
	public UInt16 u2ShortCutDetail;

	public Byte u1StartTime;
	public Byte u1RetainTime;

	public string sIconImage;
	
	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2ID = Convert.ToUInt16(cols[idx++]);
		sName = cols[idx++];
		sDescription = cols[idx++];
		u1PeriodType = Convert.ToByte (cols [idx++]);
        u2EventID = Convert.ToUInt16(cols[idx++]);
		
		u1AchievementType = Convert.ToByte(cols[idx++]);
		u2AchievementTypeID = Convert.ToUInt16(cols[idx++]);
		u1Delemiter1 = Convert.ToByte(cols[idx++]);
		u1Delemiter2 = Convert.ToByte(cols[idx++]);
		u1Delemiter3 = Convert.ToByte(cols[idx++]);

		u4MaxCount = Convert.ToUInt32(cols[idx++]);

		acReward = new Goods[MAX_REWARD_COUNT];
		for (int i=0; i<MAX_REWARD_COUNT; i++) {
			Byte type = Convert.ToByte (cols [idx++]);
			if (type != 0){
				acReward [i] = new Goods();
				acReward [i].u1Type = type;
				acReward [i].u2ID = Convert.ToUInt16 (cols [idx++]);
				acReward [i].u4Count = Convert.ToUInt32 (cols [idx++]);
			}else{
				idx++;
				idx++;
			}
		}

		u2Parent = Convert.ToUInt16(cols[idx++]);

		cOpenGoods = new Goods(cols, ref idx);

		u2ShortCut = Convert.ToUInt16(cols[idx++]);
		u2ShortCutDetail = Convert.ToUInt16(cols[idx++]);

		u1StartTime = Convert.ToByte(cols[idx++]);
		u1RetainTime = Convert.ToByte(cols[idx++]);

		sIconImage = cols[idx++];

        cCloseGoods = new Goods(cols, ref idx);
		
		return u2ID;
	}
}

public class AchievementTypeData
{
	public const Byte Connect = 1;				// 접속
	// 1 일별
	// 1 시간별
	public const Byte StageClear = 2;			// 스테이지 클리어
	// 1 일반, 2 보스
	// 1 쉬움, 2 보통, 3 지옥
	// 챕터별(1~ ) 
	public const Byte ModeClear = 3;			// 모드 클리어
	// 챕터별(1~ ) 
	public const Byte LeagueMatch = 4;			// 리그대전
	// 리그등급별(1~ ) 
	// 1 정규리그, 2 스플릿리그
	public const Byte MakeEquip = 5;			// 장비제작
	// 등급별(1~10) 
	// 장비착용 위치별(1~10)
	// 클래스별(1~10)
	public const Byte ChangeLook = 6;			// 외형 변경
	// 장비착용 위치별(1~10)
	// 클래스별(1~10)
	public const Byte Lottery = 7;				// 뽑기
	// 뽑기 항목별(?)
	public const Byte Training = 8;				// 수련의방 진행
	public const Byte EqTraining = 9;			// 병기훈련 진행
	public const Byte CharStatPoint = 10;		// 캐릭터 스탯포인트 사용
	public const Byte EquipStatPoint = 11;		// 장비 스탯포인트 사용
	public const Byte SkillSelect = 12;			// 스킬 장착
	public const Byte SkillUpgrade = 13;		// 스킬 업그레이드
	public const Byte CreateChar = 14;			// 캐릭터 생성
	public const Byte KillMonster = 15;			// 몬스터 처치
	// 속성별(1~4)
	// 1 일반, 2 보스
	public const Byte MakeRune = 16;			// 룬 제작
	// 등급별(1~10) 
	public const Byte StageStar = 17;			// 스테이지 별 획득
	// 1 쉬움, 2 보통, 3 지옥
	public const Byte Dispatch = 18;			// 파견 완료
	public const Byte Sweep = 19;				// 소탕 완료
	public const Byte ChapterClear = 20;		// 챕터 클리어
	// 1 쉬움, 2 보통, 3 지옥
	public const Byte CrewOpen = 21;			// 크루 오픈
	// 크루별 1~5
	public const Byte Friend = 22;				// 친구 수
	public const Byte ForgeLevel = 23;			// 대장간 레벨
	// 대장간 레벨별(1~10) 
	public const Byte CharLevel = 24;			// 캐릭터 레벨
	// 클래스별(1~10)
	public const Byte EquipLevel = 25;			// 장비 레벨
	// 장비착용 위치별(1~10)
	public const Byte FuseEquip = 26;			// 장비합성
	public const Byte RetireChar = 27;			// 캐릭터 은퇴

	public const Byte VIP = 28;					// VIP 달성
	public const Byte CollectItem = 29;			// 재료 수집
	
	public const Byte StageClearMode1 = 30;		// 시련의 탑 스테이지 클리어
	public const Byte ChapterClearMode1 = 31;	// 시련의 탑 챕터 클리어
	
	public const Byte StageClearMode2 = 32;		// 탐색의 숲 스테이지 클리어
	public const Byte ChapterClearMode2 = 33;	// 탐색의 숲 챕터 클리어

	public const Byte Gold = 34;				// 골드 획득량
	public const Byte Cash = 35;				// 캐쉬 획득량
	public const Byte StageKey = 36;			// 열쇠 획득량

	public const Byte Achievement = 37;			// 업적 완료 횟수
	public const Byte ClearQuest = 38;			// 퀘스트 완료 횟수
	public const Byte GetEquip = 39;			// 장비 획득 횟수

    public const Byte CrewPower = 40;			// 크루 전투력 도달
    public const Byte LeagueDivision = 41;		// 리그 디비전 도달
    public const Byte CreateEquipGrade = 42;	// 제작 장비 등급 1.별, 2.티어, 3.클래스

    public const Byte MAX_TYPE = 43;
	
	public const Byte MAX_PERIOD = 3;
	public const Byte MAX_VALUETYPE = 4;

	public Byte periodType;
	public Byte type;
	public UInt16 typeId;
	public Byte delimiter1Range;
	public Byte delimiter2Range;
	public Byte delimiter3Range;
	public Byte valueType;	// 1 bool, 2 u1, 3 u2, 4 u4

	public int basePos;
	public byte delimiterCount;

	public void Set(string[] cols)
	{
		UInt16 idx = 0;
		periodType = Convert.ToByte(cols[idx++]);
		type = Convert.ToByte(cols[idx++]);
		typeId = Convert.ToUInt16(cols[idx++]);
		delimiter1Range = Convert.ToByte(cols[idx++]);
		delimiter2Range = Convert.ToByte(cols[idx++]);
		delimiter3Range = Convert.ToByte(cols[idx++]);

		if(delimiter1Range == 0) delimiterCount = 0;
		else
		{
			delimiter1Range++;
			if(delimiter2Range == 0) delimiterCount = 1;
			else
			{
				delimiter2Range++;
		
				if (delimiter3Range == 0) delimiterCount = 2;
				else 
				{
					delimiter3Range++;
					delimiterCount = 3;
				}
			}
		}
		valueType = Convert.ToByte(cols[idx++]);
	}

	public int Length
	{
		get {
			int length = 1;
			if (delimiter1Range > 0) length *= delimiter1Range;
			if (delimiter2Range > 0) length *= delimiter2Range;
			if (delimiter3Range > 0) length *= delimiter3Range;
			return length;
		}
	}

	public void Update(byte delimiter1, byte delimiter2, byte delimiter3, uint addcount)
	{
		byte[] buffer = GetBuffer();

		//writeToBuffer(buffer, basePos, addcount);
		//if (delimiterCount == 0) return;
		//writeToBuffer(buffer, basePos + delimiter1, addcount);
		//if (delimiterCount == 1) return;
		//writeToBuffer(buffer, basePos + delimiter2 * delimiter1Range, addcount);
		//writeToBuffer(buffer, basePos + delimiter1 + delimiter2 * delimiter1Range, addcount);
		//if (delimiterCount == 2) return;
		//writeToBuffer(buffer, basePos + delimiter3 * delimiter1Range * delimiter2Range, addcount);
		//writeToBuffer(buffer, basePos + delimiter1 + delimiter3 * delimiter1Range * delimiter2Range, addcount);
		//writeToBuffer(buffer, basePos + delimiter2 * delimiter1Range + delimiter3 * delimiter1Range * delimiter2Range, addcount);
		//writeToBuffer(buffer, basePos + delimiter1 + delimiter2 * delimiter1Range + delimiter3 * delimiter1Range * delimiter2Range, addcount);
        
        bool bDelimiter1 = delimiter1 > 0 && delimiter1 < delimiter1Range;
        bool bDelimiter2 = delimiter2 > 0 && delimiter2 < delimiter2Range;
        bool bDelimiter3 = delimiter3 > 0 && delimiter3 < delimiter3Range;

        writeToBuffer(buffer, basePos, addcount);
        if (delimiterCount == 0) return;
        if (bDelimiter1)
            writeToBuffer(buffer, basePos + delimiter1, addcount);

        if (delimiterCount == 1) return;
        if (bDelimiter2)
            writeToBuffer(buffer, basePos + delimiter2 * delimiter1Range, addcount);

        if (bDelimiter1 && bDelimiter2)
            writeToBuffer(buffer, basePos + delimiter1 + delimiter2 * delimiter1Range, addcount);

        if (delimiterCount == 2 || !bDelimiter3) return;

        writeToBuffer(buffer, basePos + delimiter3 * delimiter1Range * delimiter2Range, addcount);

        if (bDelimiter1)
            writeToBuffer(buffer, basePos + delimiter1 + delimiter3 * delimiter1Range * delimiter2Range, addcount);
        if (bDelimiter2)
            writeToBuffer(buffer, basePos + delimiter2 * delimiter1Range + delimiter3 * delimiter1Range * delimiter2Range, addcount);
        if (bDelimiter1 && bDelimiter2)
            writeToBuffer(buffer, basePos + delimiter1 + delimiter2 * delimiter1Range + delimiter3 * delimiter1Range * delimiter2Range, addcount);
    }

	byte[] GetBuffer()
	{
		switch (valueType) {
		case 1: return Legion.Instance.cQuest.abBuffer;
		case 2: return Legion.Instance.cQuest.au1Buffer;
		case 3: return Legion.Instance.cQuest.au2Buffer;
		case 4: return Legion.Instance.cQuest.au4Buffer; 
		}

		return new byte[1];
	}

	void writeToBuffer(byte[] buffer, int pos, uint addcount)
	{
		uint oldcount = 0;
		int bitpos = 0;
		switch (valueType)
		{
		case 1:
			bitpos = pos / 8;
			buffer[bitpos] |= BitMask(pos - bitpos * 8);
			break;
		case 2:
			if (type == CharLevel || type == EquipLevel)
			{
				if (addcount > buffer[pos])
					buffer[pos] = (byte)addcount;
			}
			else
				buffer[pos] += (byte)addcount;
			break;
		case 3:
			oldcount = BitConverter.ToUInt16(buffer, pos * 2);
			if (type == CharLevel || type == EquipLevel)
			{
				if (addcount > oldcount)
					Array.Copy(BitConverter.GetBytes(addcount), 0, buffer, pos * 2, 2);
			}
			else
				Array.Copy(BitConverter.GetBytes(oldcount + addcount), 0, buffer, pos * 2, 2);
			break;
		case 4:
			oldcount = BitConverter.ToUInt32(buffer, pos * 4);
			if (type == CharLevel || type == EquipLevel)
			{
				if (addcount > oldcount)
					Array.Copy(BitConverter.GetBytes(addcount), 0, buffer, pos * 4, 4);
			}
			else
				Array.Copy(BitConverter.GetBytes(oldcount + addcount), 0, buffer, pos * 4, 4);
			break;
		}
	}

	public uint GetCount(byte delimiter1, byte delimiter2, byte delimiter3)
	{
		byte[] buffer = GetBuffer ();

		int pos = basePos + delimiter1 + delimiter2 * delimiter1Range + delimiter3 * delimiter1Range * delimiter2Range;

		int bitpos = 0;
		switch (valueType)
		{
		case 1:
			bitpos = pos / 8;
			return (uint)(buffer[bitpos] & BitMask(pos - bitpos * 8));
		case 2:
			return buffer[pos];
		case 3:
			return BitConverter.ToUInt16(buffer, pos * 2);
		case 4:
			return BitConverter.ToUInt32(buffer, pos * 4);
		}
		return 0;
	}

	public static byte BitMask(int bit)
	{
		switch (bit)
		{
		case 0: return 0x01;
		case 1: return 0x02;
		case 2: return 0x04;
		case 3: return 0x08;
		case 4: return 0x10;
		case 5: return 0x20;
		case 6: return 0x40;
		case 7: return 0x80;
		}
		return 0x00;
	}

}


public class Quest {
    public UInt16 u2IngQuest;
    public UInt32 u4QuestCount;
    public bool bIngDirection;
    bool bEndDirection;

    public Byte[] abBuffer = new Byte[Server.ConstDef.SizeOfAchievementBoolBuffer];
    public Byte[] au1Buffer = new Byte[Server.ConstDef.SizeOfAchievementU1Buffer];
    public Byte[] au2Buffer = new Byte[Server.ConstDef.SizeOfAchievementU2Buffer];
    public Byte[] au4Buffer = new Byte[Server.ConstDef.SizeOfAchievementU4Buffer];

    public Dictionary<UInt16, UserQuest> dicQuests;
    public Dictionary<UInt16, UserAchievement> dicAchievements;

    private Byte[] levelUpAchieveTypes = new Byte[3] { 23, 24, 25 };

    //#ODIN [유저 오딘 미션 정보]
    public List<UserOdinMission> userOdinMissionList; //현재 진행중인 오딘 임무
    public Queue<UserOdinMission> receiveOdinMissionList; // 서버에 요청해서 받은 미션
    public UInt16 u2ClearOdinMissionID;

    public Quest()
    {
        dicQuests = new Dictionary<UInt16, UserQuest>();
        dicAchievements = new Dictionary<UInt16, UserAchievement>();
        userOdinMissionList = new List<UserOdinMission>();
        receiveOdinMissionList = new Queue<UserOdinMission>();
    }

    public bool isClearQuest() {
        if (CurrentQuest() == null) return false;

        if (u4QuestCount >= CurrentQuest().u2MaxCount) {
            return true;
        }

        return false;
    }

    public bool IsHaveOpenQuest() {
        foreach (UserQuest data in Legion.Instance.cQuest.dicQuests.Values) {
            if (!data.bRewarded && data.CheckOpen()) {
                return true;
            }
        }

        return false;
    }

    public UserQuest CurrentUserQuest() {
        return dicQuests[u2IngQuest];
    }

    public QuestInfo CurrentQuest() {
        return QuestInfoMgr.Instance.GetQuestInfo(u2IngQuest);
    }

    public bool CheckQuestAlarm(MENU menu, UInt16 sub) {
        if (u2IngQuest == 0) return false;

        QuestInfo temp = CurrentQuest();

        if (temp == null) {
            DebugMgr.LogError("Quest is Not Exist");
            return false;
        }

        if (isClearQuest()) return false;

        if (sub == 0) {
            if ((MENU)temp.u2ShortCut == menu) {
                return true;
            }
        } else {
            if ((MENU)temp.u2ShortCut == menu && temp.u2ShortCutDetail == sub) {
                return true;
            }
        }

        return false;
    }

    public bool CheckAlarmAchievement(Byte type) {

        foreach (UserAchievement temp in Legion.Instance.cQuest.dicAchievements.Values) {
            if (temp.GetInfo().u2EventID > 0)
                continue;

            if (temp.GetInfo().u1PeriodType == type) {
                if (temp.isOpen && temp.isClear() && !temp.bRewarded) {
                    return true;
                }
            }
        }

        return false;
    }

    public void InitList() {
        foreach (QuestInfo quest in QuestInfoMgr.Instance.GetQuestList().Values) {
            UserQuest tmpQuest = new UserQuest(quest);

            dicQuests.Add(tmpQuest.u2ID, tmpQuest);
        }

        foreach (AchievementInfo achieve in QuestInfoMgr.Instance.GetAchieveList().Values) {
            UserAchievement tmpAchieve = new UserAchievement(achieve);

            dicAchievements.Add(tmpAchieve.u2ID, tmpAchieve);
        }
    }

    public void UpdateLoginAchievement() {
        //if (Legion.Instance.checkLoginAchievement != 1) {
        //	if (Legion.Instance.checkLoginAchievement == 0) {
        //		Legion.Instance.ShowCafeOnce ();
        //	}
        //	return;
        //}
        if (Legion.Instance.checkLoginAchievement != 1)
            return;

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.LegionMark(2, UpdateLoginAchieveCnt);
        //Legion.Instance.u1MarkType = 2;
    }

    void UpdateLoginAchieveCnt(Server.ERROR_ID err) {
        PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.LEGION_MARK, err) + TextManager.Instance.GetText("popup_desc_server_error_critical"), Server.ServerMgr.Instance.ApplicationShutdown);
            return;
        }
        else if (err == Server.ERROR_ID.NONE) {
            Legion.Instance.checkLoginAchievement = 0;
            if (Legion.Instance.cTutorial.au1Step[0] == Server.ConstDef.LastTutorialStep && !Legion.Instance.cTutorial.bIng)
            {
                Legion.Instance.u1RecvLoginReward = 1;
                LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
                if (EventInfoMgr.Instance.u1EventCount != 0 && lobbyScene != null)
                {
                    lobbyScene._eventPanel.OnClickMenu(0);
                }
                else
                {
                    Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.LOGIN_REWARED);
                }

            } else {
                Legion.Instance.checkLoginAchievement = 2;
            }

            UpdateAchieveCnt(AchievementTypeData.Connect, 0, 0, 0, 0, 1);
        }
    }

    public void UpdateAchieveCnt(Byte u1AchievementType, UInt16 u2AchievementTypeID, Byte u1Delemiter1, Byte u1Delemiter2, Byte u1Delemiter3, UInt32 uCount)
    {
        List<ushort> nonclearedList = new List<ushort>();
        List<UInt32> changeList = new List<UInt32>();
        foreach (UserAchievement temp in Legion.Instance.cQuest.dicAchievements.Values) {

            if (!temp.isClear())
            {
                nonclearedList.Add(temp.u2ID);
            }

            changeList.Add(temp.GetCount());
        }

        for (Byte i = 1; i < AchievementTypeData.MAX_PERIOD + 1; i++) {
            if (QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType] != null) {
                if (QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType].ContainsKey(u2AchievementTypeID)) {
                    if (u2AchievementTypeID > 0) {
                        QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType][u2AchievementTypeID].Update(u1Delemiter1, u1Delemiter2, u1Delemiter3, uCount);

                        if (QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType].ContainsKey(0)) {
                            QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType][0].Update(u1Delemiter1, u1Delemiter2, u1Delemiter3, uCount);
                        }
                    } else {
                        QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType][u2AchievementTypeID].Update(u1Delemiter1, u1Delemiter2, u1Delemiter3, uCount);
                    }
                } else {
                    if (QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType].ContainsKey(0)) {
                        QuestInfoMgr.Instance.QuestTypeSet[i, u1AchievementType][0].Update(u1Delemiter1, u1Delemiter2, u1Delemiter3, uCount);
                    }
                }
            }
        }

        UpdateQuestCnt(u1AchievementType, u2AchievementTypeID, u1Delemiter1, u1Delemiter2, u1Delemiter3, uCount);

        for (int i = 0; i < userOdinMissionList.Count; ++i)
        {
            userOdinMissionList[i].UpdateMissionCnt(u1AchievementType, u2AchievementTypeID, u1Delemiter1, u1Delemiter2, u1Delemiter3, uCount);
        }

        int count = 0;
        foreach (UserAchievement temp in Legion.Instance.cQuest.dicAchievements.Values) {

            if (!temp.isClear())
            {
                nonclearedList.Remove(temp.u2ID);
            }

            if (temp.GetCount() != changeList[count])
            {
                Legion.Instance.UpdateOSAchievement(temp.u2ID);
            }

            count++;
        }

        if (nonclearedList.Count > 0) {
            for (int i = 0; i < nonclearedList.Count; i++)
            {
                PopupManager.Instance.ShowQuestComp(2, nonclearedList[i]);
            }
        }
    }

    void UpdateQuestCnt(Byte u1QuestType, UInt16 u2QuestTypeID, Byte u1Delemiter1, Byte u1Delemiter2, Byte u1Delemiter3, uint uCount) {
        if (u2IngQuest == 0) return;

        if (isClearQuest())
            return;

        QuestInfo tempQuest = CurrentQuest();

        if (tempQuest.u1QuestType != u1QuestType) return;

        if (tempQuest.u2QuestTypeID > 0) {
            if (u1QuestType == 15 && StageInfoMgr.Instance.dicChapterData.ContainsKey(tempQuest.u2QuestTypeID) == true)
            {
                if (Legion.Instance.SelectedStage == null)
                    return;

                if (Legion.Instance.SelectedStage.chapterInfo.u2ID != tempQuest.u2QuestTypeID)
                    return;
            }
            else if (tempQuest.u2QuestTypeID != u2QuestTypeID)
                return;
        }

        if (tempQuest.u1Delemiter1 > 0) {
            if (tempQuest.u1Delemiter1 != u1Delemiter1)
                return;
        }
        if (tempQuest.u1Delemiter2 > 0) {
            if (tempQuest.u1Delemiter2 != u1Delemiter2)
                return;
        }
        if (tempQuest.u1Delemiter3 > 0) {
            if (tempQuest.u1Delemiter3 != u1Delemiter3)
                return;
        }

        if (!CheckLevelUpQuest(true)) {
            if (u4QuestCount < CurrentQuest().u2MaxCount) {
                u4QuestCount += uCount;
            }
        }

        PopupManager.Instance.ShowQuestComp(1, CurrentQuest().u2ID);

        if (u4QuestCount >= CurrentQuest().u2MaxCount) {
            u4QuestCount = CurrentQuest().u2MaxCount;
            //QuestClear();

            LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;

            if (lobbyScene != null) {
                lobbyScene.Btn_QuickQuest.SetButton();
            }
        }
    }

    public void StartQuest(UInt16 qID) {
        bEndDirection = false;
        bIngDirection = false;
        u2IngQuest = qID;
        u4QuestCount = 0;
        CheckLevelUpQuest(false);
    }

    public void CancelQuest() {
        u2IngQuest = 0;
        u4QuestCount = 0;
    }

    public void EndQuest() {
        dicQuests[u2IngQuest].bRewarded = true;
        u2IngQuest = 0;
        u4QuestCount = 0;
    }

    public void CheckIngDirection() {
        if (u2IngQuest == 0 || bIngDirection)
            return;

        QuestDirection qd = QuestInfoMgr.Instance.GetQuestDirctionInfo(u2IngQuest, 2);
        if (qd != null) {
            bIngDirection = true;
            PopupManager.Instance.showLoading = true;
            PopupManager.Instance.SetQuestDirectionPopup(qd, null);
        }
    }

    public void CheckEndDirection(Byte _u1Type) {       
        if (!isClearQuest())
            return;

        if (bEndDirection)
            return;

        if (CurrentQuest().u1QuestType == _u1Type) {
            bEndDirection = true;
            Legion.Instance.AUTOCONTINUE = false;
            PopupManager.Instance.SetQuestCompletePopup(_u1Type);
        }
    }

    public bool CheckLevelUpQuest(bool bIng) {
        if (u2IngQuest == 0) return false;

        for (int i = 0; i < levelUpAchieveTypes.Length; i++) {
            if (CurrentQuest().u1QuestType == levelUpAchieveTypes[i])
            {

                if (levelUpAchieveTypes[i] == AchievementTypeData.CharLevel)
                {
                    u4QuestCount = Legion.Instance.TopLevel;
                    return true;
                }
                else if (levelUpAchieveTypes[i] == AchievementTypeData.EquipLevel)
                {
                    if (bIng) u4QuestCount = Legion.Instance.u2EquipTopLevel;
                    else u4QuestCount = Legion.Instance.GetTopEquipLevel();

                    return true;
                }
                else if (levelUpAchieveTypes[i] == AchievementTypeData.ForgeLevel)
                {
                    u4QuestCount = Legion.Instance.u1ForgeLevel;
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckQuestRelationInAct(ActInfo actInfo) {
        QuestInfo info = CurrentQuest();
        if (info.u1QuestType == 2) {
            if (info.u2QuestTypeID > 0) {
                if (StageInfoMgr.Instance.dicStageData.ContainsKey(info.u2QuestTypeID)) {
                    if (StageInfoMgr.Instance.dicStageData[info.u2QuestTypeID].u2ActID == 0 || StageInfoMgr.Instance.dicStageData[info.u2QuestTypeID].u2ActID == actInfo.u2ID) {
                        return true;
                    }
                }
            }
        }
        else if (info.u1QuestType == 15)
        {
            bool isChapterID = false;
            ChapterInfo chapterInfo = null;
            StageInfoMgr.Instance.dicChapterData.TryGetValue(info.u2QuestTypeID, out chapterInfo);
            if (chapterInfo != null)
                isChapterID = true;

            if (actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE) {
                foreach (StageInfo temp in StageInfoMgr.Instance.dicStageData.Values) {
                    if ((temp.u2ActID == 0 || temp.u2ActID == actInfo.u2ID) && temp.chapterInfo != null) {
                        if (temp.chapterInfo == null)
                            continue;

                        // 퀘스트 타입 ID가 챕터 아이디이며 stgae의 챕터 ID가 퀘스트 ID와 같은지 확인
                        if (isChapterID == true && temp.chapterInfo.u2ID != info.u2QuestTypeID)
                            continue;

                        for (int i = 0; i < temp.acPhases.Length; i++)
                        {
                            FieldInfo fieldInfo = StageInfoMgr.Instance.GetFieldInfo(temp.acPhases[i].u2FieldID);
                            if (fieldInfo == null)
                                continue;

                            for (int j = 0; j < fieldInfo.acMonsterGroup.Length; j++)
                            {
                                for (int k = 0; k < fieldInfo.acMonsterGroup[j].acMonsterInfo.Length; k++)
                                {
                                    ClassInfo monsterInfo = ClassInfoMgr.Instance.GetInfo(fieldInfo.acMonsterGroup[j].acMonsterInfo[k].u2MonsterID);
                                    if (monsterInfo == null)
                                        continue;

                                    if (isChapterID == false)
                                    {
                                        if (info.u2QuestTypeID > 0 && monsterInfo.u2ID != info.u2QuestTypeID)
                                            continue;
                                    }
                                    if (info.u1Delemiter1 > 0 && monsterInfo.u1Element != info.u1Delemiter1)
                                        continue;

                                    if (info.u1Delemiter2 > 0 && monsterInfo.u1MonsterType != info.u1Delemiter2)
                                        continue;

                                    if (info.u1Delemiter3 > 0 && Legion.Instance.SelectedDifficult != info.u1Delemiter3)
                                        continue;

                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        } else if (info.u1QuestType == 29) {
            foreach (StageInfo temp in StageInfoMgr.Instance.dicStageData.Values) {
                if (temp.u2ActID == 0 || temp.u2ActID == actInfo.u2ID)
                {
                    if (info.u2QuestTypeID > 0)
                    {
                        if (temp.CheckRewardInStage(info.u2QuestTypeID) > 0)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool CheckQuestRelationInStage(StageInfo stageInfo) {
        QuestInfo tempQuest = CurrentQuest();
        // 퀘스트에 지정된 챕터의 아이디가 퀘스트 마크를 셋팅하지 않는다
        if (tempQuest.u2ShortCutChapter != 0 && tempQuest.u2ShortCutChapter != stageInfo.chapterInfo.u2ID)
            return false;

        if (stageInfo.u1StageNum == 10 && tempQuest.u1QuestType == 20)
        {
            if (stageInfo.GetActInfo().u1Number == tempQuest.u1Delemiter1)
            {
                if (tempQuest.u2QuestTypeID == stageInfo.chapterInfo.u2ID)
                {
                    if (tempQuest.u1Delemiter2 > 0) {
                        if (tempQuest.u1Delemiter2 == Legion.Instance.SelectedDifficult)
                        {
                            return true;
                        }
                    } else {
                        return true;
                    }
                }
            }
        }
        else if (tempQuest.u1QuestType == 15)
        {
            bool isChapterID = false;
            ChapterInfo chapterInfo = null;
            StageInfoMgr.Instance.dicChapterData.TryGetValue(tempQuest.u2QuestTypeID, out chapterInfo);
            if (chapterInfo != null)
            {
                if (tempQuest.u2QuestTypeID != stageInfo.chapterInfo.u2ID)
                    return false;

                isChapterID = true;
            }

            for (int i = 0; i < stageInfo.acPhases.Length; i++)
            {
                FieldInfo fieldInfo = StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID);
                if (fieldInfo == null)
                    return false;

                for (int j = 0; j < fieldInfo.acMonsterGroup.Length; j++)
                {
                    for (int k = 0; k < fieldInfo.acMonsterGroup[j].acMonsterInfo.Length; k++)
                    {
                        ClassInfo monsterInfo = ClassInfoMgr.Instance.GetInfo(fieldInfo.acMonsterGroup[j].acMonsterInfo[k].u2MonsterID);
                        if (monsterInfo == null)
                            continue;

                        // u2QuestTypeID가 챕터 아이디가 아니라면
                        if (isChapterID == false & tempQuest.u2QuestTypeID > 0 && monsterInfo.u2ID != tempQuest.u2QuestTypeID)
                            continue;

                        // 몬스터 속성 확인
                        if (tempQuest.u1Delemiter1 > 0 && monsterInfo.u1Element != tempQuest.u1Delemiter1)
                            continue;

                        // 몬스터 타입 확인
                        if (tempQuest.u1Delemiter2 > 0 && monsterInfo.u1MonsterType != tempQuest.u1Delemiter2)
                            continue;

                        // 난이도 확인
                        if (tempQuest.u1Delemiter3 > 0 && Legion.Instance.SelectedDifficult != tempQuest.u1Delemiter3)
                            continue;

                        return true;
                    }
                }
            }
        } else if (tempQuest.u1QuestType == 29) {
            if (tempQuest.u2QuestTypeID > 0) {
                if (stageInfo.CheckRewardInStage(tempQuest.u2QuestTypeID) > 0) {
                    return true;
                }
            }
        } else {
            Byte bossType = stageInfo.u1BossType;
            if (bossType < 2) bossType = 1;
            else if (bossType == 3) bossType = 2;

            if (tempQuest.u2QuestTypeID > 0) {
                if (stageInfo.u2ID == tempQuest.u2QuestTypeID) {
                    if ((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == Legion.Instance.SelectedDifficult)
                        && (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == bossType)
                        && (tempQuest.u1Delemiter3 == 0 || tempQuest.u1Delemiter3 == stageInfo.GetActInfo().u1Number))
                    {
                        return true;
                    }
                }
            } else {
                if ((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == Legion.Instance.SelectedDifficult)
                    && (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == bossType)
                    && (tempQuest.u1Delemiter3 == 0 || tempQuest.u1Delemiter3 == stageInfo.GetActInfo().u1Number))
                {
                    return true;
                }
            }
        }

        return false;
    }
    //#ODIN [오딘 임무 셋팅 관련 함수]
    public bool OdinMissionSeting(UInt16 missionID, UInt32 count)
    {
        OdinMissionInfo missionInfo;
        if (!QuestInfoMgr.Instance.TryGetOdinMissionInfo(missionID, out missionInfo))
        {
            return false;
        }

        //#ODIN [오딘 미션 순서는 추후 프로토콜 추가후 변경하도록 한다]
        UserOdinMission userMission = new UserOdinMission(missionID, count);
        if (userMission.MissionState != UserOdinMission.StateType.NONE)
        {
            userOdinMissionList.Add(userMission);
        }

        return true;
    }
    //#ODIN [오딘 임무 변경]
    public bool ChangeOdinMission(UInt16 prevMissionID, UserOdinMission newOdinMission)
    {
        //#ODIN [오딘 미션 순서는 추후 프로토콜 추가후 변경하도록 한다]
        for (int i = 0; i < userOdinMissionList.Count; ++i)
        {
            if (userOdinMissionList[i].GetInfo().u2ID == prevMissionID)
            {
                userOdinMissionList[i] = newOdinMission;
                return true;
            }
        }

        return false;
    }
    //#ODIN [오딘 임무 삭제] 임무 완료시 해당 미션 리스트에서 제거
    public bool RemoveOdinMission(UInt16 missionID)
    {
        // 임시 리스트 형태로 해당 아이디 값을 찾아 삭제
        for (int i = 0; i < userOdinMissionList.Count;)
        {
            if (userOdinMissionList[i].GetInfo().u2ID == missionID)
            {
                userOdinMissionList.RemoveAt(i);
                break;
            }
            ++i;
        }
        return true;
    }
    //#ODIN [오딘 임무 찾기] 
    public UserOdinMission GetUserOdinMssion(UInt16 missionID)
    {
        for (int i = 0; i < userOdinMissionList.Count; ++i)
        {
            if (userOdinMissionList[i].GetInfo().u2ID == missionID)
            {
                return userOdinMissionList[i];
            }
        }

        return null;
    }
    //#ODIN [오딘 임무 클리어 확인] 
    public bool IsClearOdinMission()
    {
        for(int i = 0; i < userOdinMissionList.Count; ++i)
        {
            if (!userOdinMissionList[i].IsPossible())
                continue;

            if(userOdinMissionList[i].IsClear())
            {
                return true;
            }
        }
        return false;
    }
}

public class UserQuest {
	public UInt16 u2ID;
	public UInt32 u4MaxCount;
	public bool bDirectionView;
	public bool bRewarded;
	public bool bNew;

	ushort parent;
	public bool isOpen;

	public UserQuest()
	{
	}

	public UserQuest(QuestInfo info)
	{
		u2ID = info.u2ID;
		u4MaxCount = info.u2MaxCount;
		parent = info.u2Parent;
		if (info.u2Parent == 0)
			isOpen = true;
	}

	public bool CheckOpen(){
		if (parent > 0) {
			if (Legion.Instance.cQuest.dicQuests.ContainsKey (parent)) {
				isOpen = Legion.Instance.cQuest.dicQuests [parent].bRewarded;
			}
		}

		if(isOpen){
			isOpen = CheckGoods();
		}

		return isOpen;
	}

	public bool CheckGoods(){
		if(GetInfo().cOpenGoods.u1Type > 0){
			if(Legion.Instance.CheckEnoughGoods(GetInfo().cOpenGoods)) return true;
			else return false;
		}

		return true;
	}

	public bool isIng(){
		if (u2ID == Legion.Instance.cQuest.u2IngQuest)
			return true;

		return false;
	}

	public bool isClear(){
		if (u2ID != Legion.Instance.cQuest.u2IngQuest)
			return false;
		
		if(Legion.Instance.cQuest.u4QuestCount >= u4MaxCount) return true;

		return false;
	}

	public QuestInfo GetInfo(){
		return QuestInfoMgr.Instance.GetQuestInfo(u2ID);
	}

	public Goods[] GetReward(){
		return GetInfo().acReward;
	}
}

public class UserAchievement {
	public UInt16 u2ID;
	public UInt32 u4MaxCount;
	bool bClear;
	public bool bRewarded;

	ushort parent;
	public bool isOpen;

	AchievementInfo info;
	public UserAchievement()
	{
	}

	public UserAchievement(AchievementInfo tInfo)
	{
		info = tInfo;
		u2ID = info.u2ID;
		u4MaxCount = info.u4MaxCount;
		parent = info.u2Parent;
		if (info.u2Parent == 0)
			isOpen = true;
	}
	
	public bool CheckOpen(){
		AchievementInfo info = GetInfo ();
		if (info.u1AchievementType == 0) 
        {
			isOpen = false;
			if (info.u1RetainTime != 0) {
                if (QuestInfoMgr.Instance.CheckRewardTime(info.u1StartTime, info.u1RetainTime))
                {
                    isOpen = true;
                }
					
			}
            else if (info.cOpenGoods.u1Type == (Byte)GoodsType.VIP_LEVEL)
            {
                //DebugMgr.Log("info.u2ID   ::::  " + info.u2ID);
                if (info.cOpenGoods.u4Count == Legion.Instance.u1VIPLevel)
                    isOpen = true;

            }
		} 
        else 
        {

            if (parent > 0) 
            {
                if (Legion.Instance.cQuest.dicAchievements.ContainsKey (parent))
                    isOpen = Legion.Instance.cQuest.dicAchievements [parent].bRewarded;
            }

            if (isOpen) {
                isOpen = CheckGoods ();
            }
			
		}
		return isOpen;
	}

	public bool CheckGoods(){
		if (info.u1AchievementType == 0) {
			if (info.u1RetainTime != 0) {
				if (QuestInfoMgr.Instance.CheckRewardTime (info.u1StartTime, info.u1RetainTime))
					return true;
			}
			return false;
		}

		if(GetInfo().cOpenGoods.u1Type > 0){
			if(Legion.Instance.CheckEnoughGoods(GetInfo().cOpenGoods)) return true;
			else return false;
		}else{
			return true;
		}
	}

	public UInt32 GetCount(){
		AchievementInfo info = GetInfo ();
		if (info.u1AchievementType == 0) {
			if (info.u1RetainTime != 0) {
				if (QuestInfoMgr.Instance.CheckRewardTime (info.u1StartTime, info.u1RetainTime))
					return info.u4MaxCount;
			}
			return 0;
		}
		return QuestInfoMgr.Instance.QuestTypeSet[info.u1PeriodType, info.u1AchievementType][info.u2AchievementTypeID].GetCount(info.u1Delemiter1, info.u1Delemiter2, info.u1Delemiter3);
	}

	public bool isClear(){
		if(bClear) return true;
		AchievementInfo info = GetInfo ();
		if (info.u1AchievementType == 0) 
        {
            if (info.cOpenGoods.u1Type == (Byte)GoodsType.VIP_LEVEL)
            {
                return true;
            }
			else if (info.u1RetainTime != 0) 
            {
				if (QuestInfoMgr.Instance.CheckRewardTime (info.u1StartTime, info.u1RetainTime))
					return true;
			}
			return false;
		}
		//DebugMgr.LogError("PeriodType : " + info.u1PeriodType + ", AchievementType : " + info.u1AchievementType + ", AchievementTypeID : " + info.u2AchievementTypeID);
		UInt32 u4Count = QuestInfoMgr.Instance.QuestTypeSet[info.u1PeriodType, info.u1AchievementType][info.u2AchievementTypeID].GetCount(info.u1Delemiter1, info.u1Delemiter2, info.u1Delemiter3);
        //if (info.u2ID == 50006)
        //{
        //    DebugMgr.LogError("Count : " + u4Count);
        //}
		if(u4Count >= u4MaxCount){
			bClear = true;
			return true;
		}

		return false;
	}

	public AchievementInfo GetInfo(){
		return info;
	}

	public Goods[] GetReward(UInt32 cnt)
	{
		return QuestInfoMgr.Instance.GetAchieveInfo(u2ID).acReward;
	}
}

public class QuestDirection{
	public UInt16 u2QuestID;
	public Byte u1DirectionPos;
	public bool bSkip;

	public Byte u1PageCount;
	
	public List<string> lstTalks;

	public UInt16 Set(string[] cols)
	{
		UInt16 idx = 0;
		u2QuestID = Convert.ToUInt16(cols[idx++]);
		u1DirectionPos = Convert.ToByte(cols[idx++]);
		bSkip = cols[idx] == "T" || cols[idx] == "t";
		idx++;

		u1PageCount = Convert.ToByte(cols[idx++]);
		
		lstTalks = new List<string> ();
		
		for (int i=0; i<u1PageCount; i++) {
			lstTalks.Add (cols[idx++]);
		}
		
		return u2QuestID;
	}
}