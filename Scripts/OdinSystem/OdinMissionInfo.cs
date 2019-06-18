using System;
using System.Collections;

public struct OdinMissionInfo
{
    public UInt16 u2ID;                // 임무 정보 ID
    public string strName;             // 임무 이름
    public Byte u1MinssionCountType;   // 미션 구분  # 1.누적, 2.시점
    public string strDescription;      // 임무 내용
    public Byte u1Group;               // 임무 그룹
    public Byte u1RollingType;           // 임무 그룹 타입 #1.일반, 2.중간
    public Byte u1MinssionType;        // 임무 타입
    public UInt16 u2MinssionTypeID;    // 임무 ID
    public Byte[] au1Delemiter;        // 임무 부항목
    public UInt32 u4TargetValue;       // 임무 목표 값
    public Goods cFastReward;           // 즉시 지급 보상
    public Goods[] acReward;           // 임무 보상 정보
    public Goods cMissionOpenInfo;     // 미션 오픈 조건
    public UInt16 u2ShortCut;          // 바로 가기 위치 ID
    public UInt16 u2ShortCutDetail;    // 이동후 세부 정보 값 ID
    public UInt16 u2ShortCutChapter;   // 바로갈 챕터 ID
    public Byte u1MissionKind;         // 미션 종류에 따라서 슬롯변경 값에 쓰임

    public UInt16 Set(string[] cols)
    {
        UInt16 idx = 0;

        u2ID = Convert.ToUInt16(cols[idx++]);
        strName = cols[idx++];
        u1MinssionCountType = Convert.ToByte(cols[idx++]);
        strDescription = cols[idx++];
        u1Group = Convert.ToByte(cols[idx++]);
        u1RollingType = Convert.ToByte(cols[idx++]);

        u1MinssionType = Convert.ToByte(cols[idx++]);
        u2MinssionTypeID = Convert.ToUInt16(cols[idx++]);
        au1Delemiter = new byte[3];
        for(int i =0;i < au1Delemiter.Length; ++i)
        {
            au1Delemiter[i] = Convert.ToByte(cols[idx++]);
        }
        u4TargetValue = Convert.ToUInt32(cols[idx++]);

        cFastReward = new Goods(cols, ref idx);
        acReward = new Goods[2];
        for(int i = 0;i < acReward.Length; ++i)
        {
            acReward[i] = new Goods(cols, ref idx);
        }

        cMissionOpenInfo = new Goods(cols, ref idx);

        u2ShortCut = Convert.ToUInt16(cols[idx++]);
        u2ShortCutDetail = Convert.ToUInt16(cols[idx++]);
        u2ShortCutChapter = Convert.ToUInt16(cols[idx++]);

        u1MissionKind = Convert.ToByte(cols[idx++]);

        return u2ID;
    }
    // [임시] 테스트용 확인 코드
    public override string ToString()
    {
        return string.Format(
            "임무ID = {0}\n임무 카운트 구분 = {1}\n임무 임무 타입 = {2}\n임무 목표 ID = {3}\n임무 목표 = {4}\n 부항목값1 = {5}, 부항목값2 = {6}, 부항목값3 = {7}\n",
            u2ID,
            u1MinssionCountType == 1 ? "누적" : "시점",
            u1MinssionType,
            u2MinssionTypeID,
            u4TargetValue,
            au1Delemiter[0], au1Delemiter[1], au1Delemiter[2]
            );
    }

}

public struct NextOdinMission
{
    public UInt16 missionID;
    public UInt16 progerssCount;
    public NextOdinMission( UInt16 ID, UInt16 count)
    {
        missionID = ID;
        progerssCount = count;
    }
}

public class UserOdinMission
{
    public enum StateType
    {
        NONE = 0,
        COMPLETE,       // 완료
        PROGRESS,       // 진행 중
        WAIT,           // 대기
        MAX
    }

    // V 클리어 여부 0.대기 1.진행중 2.완료
    private StateType eMissionState;
    public StateType MissionState
    {
        set { eMissionState = value; }
        get { return eMissionState; }
    }
    private UInt32 u4MissionCount;   // 미션 진행 값 
    public UInt32 MissionProgressCount { get { return u4MissionCount; } }
    private OdinMissionInfo stuMissionInfo;

    public UserOdinMission(UInt16 missionID, UInt32 count)
    {
        if (!QuestInfoMgr.Instance.TryGetOdinMissionInfo(missionID, out stuMissionInfo))
        {
            eMissionState = StateType.NONE;
        }
        else
        {
            u4MissionCount = count;
            IsPossible();
        }
    }
    
    // 임무 완료 여부
    public bool IsClear()
    {
        // 현재 미션 완료 상태라면
        return eMissionState == StateType.COMPLETE;
    }

    // 진행 가능 여부
    public bool IsPossible()
    {
        if(!Legion.Instance.CheckEnoughGoods(stuMissionInfo.cMissionOpenInfo))
        {
            eMissionState = StateType.WAIT;
            return false;
        }

        if (stuMissionInfo.u4TargetValue <= u4MissionCount)
        {
            eMissionState = StateType.COMPLETE;
            u4MissionCount = stuMissionInfo.u4TargetValue;
        }
        else
        {
            eMissionState = StateType.PROGRESS;
        }
        return true;
    }

    public OdinMissionInfo GetInfo()
    {
        return stuMissionInfo;
    }

    public Goods GetFistReward()
    {
        if(eMissionState == StateType.NONE)
        {
            return null;
        }
        return stuMissionInfo.cFastReward;
    }

    public Goods[] GetReward()
    {
        if (eMissionState == StateType.NONE)
        {
            return null;
        }
        return stuMissionInfo.acReward;
    }

    // 임무 진행 상황 업데이트
    public void UpdateMissionCnt(Byte minssionType, UInt16 u2minssionTypeID, Byte u1Delemiter1, Byte u1Delemiter2, Byte u1Delemiter3, UInt32 uCount)
    {
        // 정보가 정상적으로 셋팅되지 않았다면
        if (eMissionState == StateType.NONE)
            return;

        // 임무 클리어 여부
        if (IsClear())
            return;

        // 임무 진행 가능 여부 확인
        if (!IsPossible())
            return;

        // 현재 임무 타입과 업데이트할 임무의 타입이 다르면
        if (stuMissionInfo.u1MinssionType != minssionType)
            return;

        if (stuMissionInfo.u2MinssionTypeID > 0)
        {
            if (minssionType == AchievementTypeData.KillMonster && StageInfoMgr.Instance.dicChapterData.ContainsKey(stuMissionInfo.u2MinssionTypeID))
            {
                if (Legion.Instance.SelectedStage == null)
                    return;

                if (Legion.Instance.SelectedStage.chapterInfo.u2ID != stuMissionInfo.u2MinssionTypeID)
                    return;
            }
            else if (stuMissionInfo.u2MinssionTypeID != u2minssionTypeID)
                return;
        }

        if (stuMissionInfo.au1Delemiter[0] > 0)
        {
            if (stuMissionInfo.au1Delemiter[0] != u1Delemiter1)
                return;
        }
        if (stuMissionInfo.au1Delemiter[1] > 0)
        {
            if(stuMissionInfo.u1MinssionType == AchievementTypeData.LeagueMatch)
            {
                if (stuMissionInfo.au1Delemiter[1] > u1Delemiter2)
                    return;
            }
            else
            {
                if (stuMissionInfo.au1Delemiter[1] != u1Delemiter2)
                    return;
            }
            
        }
        if (stuMissionInfo.au1Delemiter[2] > 0)
        {
            if (stuMissionInfo.au1Delemiter[2] != u1Delemiter3)
                return;
        }

        if (stuMissionInfo.u1MinssionType == AchievementTypeData.CharLevel ||
            stuMissionInfo.u1MinssionType == AchievementTypeData.EquipLevel)
        {
            if (u4MissionCount < uCount)
            {
                u4MissionCount = uCount;
            }
        }
        else
        {
            u4MissionCount += uCount;
        }

        if (u4MissionCount >= stuMissionInfo.u4TargetValue)
        {
            u4MissionCount = stuMissionInfo.u4TargetValue;
            eMissionState = StateType.COMPLETE;
            LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
            if (lobbyScene != null)
            {
                lobbyScene.CheckAlram_VIP();
            }
            if (stuMissionInfo.u1MinssionType != AchievementTypeData.Gold)
                Legion.Instance.cQuest.u2ClearOdinMissionID = stuMissionInfo.u2ID;
        }
    }
}