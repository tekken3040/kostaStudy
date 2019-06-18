using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Text;

// 슬롯 순서를 바꾸려면 enum의 값을 변경하면 순서가 변경
public enum VIP_INFO_TYPE
{
	MAX_INCREASE = 0,			// 최대치 증가
	DAILY_OFFER = 1,			// 메일 제공
	COMPLETE_TIME_DOWN = 2,		// 완료 시간 단축
	BUY_ADD_OFFER = 3,			// 구매시 추가 제공
	ALL_TIME_OPEN = 4,			// 상시 오픈
	ADD_REWARD_INCREASE = 5,	// 추가 보상 제공
	ADD_LEVEL_UP_POINT = 6,		// 추가 레벨업 포인트
	BUY_POINT_INCREASE = 7,		// 구매 포인트 수량 증가
	MAX_TYPE = 8,
};

public class UI_VIPInfoPopUp : MonoBehaviour {
	
    private int TEXT_MAX = 14;
    private int m_currentPage;
    public GameObject m_LeftText;
    public GameObject m_RightText;

    public GameObject objLeftBtn;
    public GameObject objRightBtn;

    public Text m_DescTitleText;

	public UI_VIPInfoSlot[] arrVIPInfoSlot;
	private VipInfo _vipInfo;

    public Text txtNextOdinReward;
    public UI_ItemListElement_Common[] cGradeUpRewardIcon;

    public Text txtRewardZeroText;

    void OnEnable()
    {

        m_currentPage = Legion.Instance.u1VIPLevel;
        RefleshPlayerVIPInfo();
    }

    public void RefleshPlayerVIPInfo()
    {
        VipInfo nextVipInfo;
        if (m_currentPage != 18)
        {
            nextVipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)(m_currentPage + 1));
        }
        else
        {
            nextVipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)m_currentPage);
        }
        RefleshVIPreward();
    }

    public void RefleshVIPreward()
    {
        if (m_currentPage == 0)
            m_currentPage = 1;

        _vipInfo = LegionInfoMgr.Instance.GetVipInfo((Byte)m_currentPage);
        string gradeName = TextManager.Instance.GetText(string.Format("odin_name_{0}", _vipInfo.u1Level));
        m_DescTitleText.text = gradeName;
        for (int i = 0; i < (int)VIP_INFO_TYPE.MAX_TYPE; ++i)
        {
            SetVIPRewardInfo(i);
        }

        if (_vipInfo.u1Level > 1)
        {
            txtRewardZeroText.gameObject.SetActive(false);
            txtNextOdinReward.gameObject.SetActive(true);
            // 셋팅 vip 레벨이 최대 레벨이라면
            if(_vipInfo.u1Level == LegionInfoMgr.Instance.dicVipData.Count - 1)
            {
                PageChangeBtnActive(true, false);
            }
            else
            {
                PageChangeBtnActive(true, true);
            }

            txtNextOdinReward.text = string.Format(TextManager.Instance.GetText("odin_advance_reward"), gradeName);
            int slotIdx = 0;
            for (int i = 0; i < _vipInfo.acOnceReward.Length; ++i)
            {
                if (_vipInfo.acOnceReward[i].u1Type == 0)
                {
                    continue;
                }

                cGradeUpRewardIcon[slotIdx].gameObject.SetActive(true);
                cGradeUpRewardIcon[slotIdx].SetData(_vipInfo.acOnceReward[i]);
                slotIdx++;
            }
            for(;slotIdx < cGradeUpRewardIcon.Length; ++slotIdx)
            {
                cGradeUpRewardIcon[slotIdx].gameObject.SetActive(false);
            }
        }
        else
        {
            txtRewardZeroText.text = TextManager.Instance.GetText("odin_info_lv1");

            txtRewardZeroText.gameObject.SetActive(true);
            txtNextOdinReward.gameObject.SetActive(false);

            PageChangeBtnActive(false, true);
        }
    }

	void SetVIPRewardInfo(int type)
	{
		int lineCount = 0;
		arrVIPInfoSlot[type].Clear();
		switch((VIP_INFO_TYPE)type)
		{
		case VIP_INFO_TYPE.MAX_INCREASE:
			lineCount = SetMaxIncreaseContents(type);
			break;
		case VIP_INFO_TYPE.COMPLETE_TIME_DOWN:
			lineCount = SetCompleteTimeDownContents(type);
			break;
		case VIP_INFO_TYPE.DAILY_OFFER:
			lineCount = SetDailyOfferContents(type);
			break;
		case VIP_INFO_TYPE.BUY_ADD_OFFER:
			lineCount = SetBuyAddOfferContents(type);
			break;
		case VIP_INFO_TYPE.BUY_POINT_INCREASE:
			lineCount = SetBuyPointIncreaseContents(type);
			break;
		case VIP_INFO_TYPE.ALL_TIME_OPEN:
			lineCount = SetAllTimeOpenContents(type);
			break;
		case VIP_INFO_TYPE.ADD_REWARD_INCREASE:
			lineCount = SetAddRewardIncrease(type);
			break;
		case VIP_INFO_TYPE.ADD_LEVEL_UP_POINT:
			lineCount = SetAddLevelUpPoint(type);
			break;
		}

		arrVIPInfoSlot[type].SetSlot(lineCount);
	}

	int SetMaxIncreaseContents(int type)
	{
		string contentKey ="vip_max_";
		if(_vipInfo.u2AddMaxEnergy != 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 0, _vipInfo.u2AddMaxEnergy.ToString());
		if(_vipInfo.u2AddMaxFSPoint != 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 1, _vipInfo.u2AddMaxFSPoint.ToString());
		if(_vipInfo.u2BonusFSPoint != 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 2, _vipInfo.u2BonusFSPoint.ToString());

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return arrVIPInfoSlot[type].LineIndex;
	}

	int SetCompleteTimeDownContents(int type)
	{
		string contentKey ="vip_time_";
		if(_vipInfo.u1ReduceCharTrPer != 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 0, _vipInfo.u1ReduceCharTrPer.ToString());
		if(_vipInfo.u1ReduceEquipTrPer != 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 1, _vipInfo.u1ReduceEquipTrPer.ToString());
		if(_vipInfo.u1ReduceDispatchPer != 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 2, _vipInfo.u1ReduceDispatchPer.ToString());

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return  arrVIPInfoSlot[type].LineIndex;
	}

	int SetDailyOfferContents(int type)
	{
		string contentKey ="vip_daily_";
		UInt32 itemCount = Legion.Instance.cQuest.dicAchievements[(UInt16)(50015 + m_currentPage)].GetInfo().acReward[0].u4Count;//Legion.Instance.cQuest.dicAchievements[(UInt16)(50877+m_currentPage)].GetInfo().acReward[0].u4Count;
		if(itemCount > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 0, itemCount.ToString());
		itemCount = Legion.Instance.cQuest.dicAchievements[(UInt16)(50033 + m_currentPage)].GetInfo().acReward[0].u4Count;
		if(itemCount > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 1, itemCount.ToString());
		if(_vipInfo.m_nShowGoodInfoID != 0) // 10)
		{
			ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.getDeepCopyShopGoodInfo(_vipInfo.m_nShowGoodInfoID);
			if(shopGoodInfo != null)
				arrVIPInfoSlot[type].AddContents(contentKey, 2, TextManager.Instance.GetText(shopGoodInfo.title));
		}

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return arrVIPInfoSlot[type].LineIndex;
	}

	int SetBuyAddOfferContents(int type)
	{
		string contentKey ="vip_add_";
		if(_vipInfo.acBuyBonus[0].u4Count > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 0, _vipInfo.acBuyBonus[0].u4Count.ToString());
		if(_vipInfo.acBuyBonus[1].u4Count > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 1, _vipInfo.acBuyBonus[1].u4Count.ToString());
		if(_vipInfo.u1VIPGachaPer > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 2, _vipInfo.u1VIPGachaPer.ToString());

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return arrVIPInfoSlot[type].LineIndex;
	}

	int SetBuyPointIncreaseContents(int type)
	{
		string contentKey ="vip_point_";
		if(_vipInfo.u1AddSkillBuyPt > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 0, _vipInfo.u1AddSkillBuyPt.ToString());
		if(_vipInfo.u1AddStatusBuyPt > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 1, _vipInfo.u1AddStatusBuyPt.ToString());

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return arrVIPInfoSlot[type].LineIndex;
	}

	int SetAllTimeOpenContents(int type)
	{
		string contentKey ="vip_open_";
		if(_vipInfo.bOpenEventGoodsCondition == true)
			arrVIPInfoSlot[type].AddContents(contentKey, 0);
		if(_vipInfo.bOpenEventGoodsAlways == true)
			arrVIPInfoSlot[type].AddContents(contentKey, 1);
		if(_vipInfo.bOpenForest == true)
			arrVIPInfoSlot[type].AddContents(contentKey, 2);
		if(_vipInfo.bOpenBlackMarket == true)
			arrVIPInfoSlot[type].AddContents(contentKey, 3);

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return arrVIPInfoSlot[type].LineIndex;
	}

	int SetAddRewardIncrease(int type)
	{
		string contentKey ="vip_reward_";
		if(_vipInfo.bVisitAddBonus == true)
			arrVIPInfoSlot[type].AddContents(contentKey, 0);

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return arrVIPInfoSlot[type].LineIndex;
	}

	int SetAddLevelUpPoint(int type)
	{
		string contentKey ="vip_lvup_";
		if(_vipInfo.u1LvUpAddSkillPt > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 0, _vipInfo.u1LvUpAddSkillPt.ToString());
		if(_vipInfo.u1LvUpAddStatusPt > 0)
			arrVIPInfoSlot[type].AddContents(contentKey, 1, _vipInfo.u1LvUpAddSkillPt.ToString());

		arrVIPInfoSlot[type].SetTitle(contentKey);
		return arrVIPInfoSlot[type].LineIndex;
	}
		
    public void onClickLeft()
    {
        if (m_currentPage <= 1)
            return;

        m_currentPage--;
        RefleshVIPreward();
    }

    public void onClickRight()
    {
        if (m_currentPage >= 18)
            return;

        m_currentPage++;
        RefleshVIPreward();
    }

    public void PageChangeBtnActive(bool left, bool right)
    {
        objLeftBtn.SetActive(left);
        objRightBtn.SetActive(right);
    }

    public void onClickClose()
    {
		PopupManager.Instance.RemovePopup (gameObject);
		Destroy(gameObject);
    }
}

