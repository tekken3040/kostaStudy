using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class QuestRewardPopup : YesNoPopup {
	public GameObject popup;

	public Image IconBG;
	public Image Icon;

	public Transform rewardsParent;
	GameObject ItemSlot;

	public RectTransform IconTrans;

	void Awake(){
		ItemSlot = AssetMgr.Instance.AssetLoad ("Prefabs/UI/Common/ItemSlot.prefab", typeof(GameObject)) as GameObject;
	}

	void OnEnable(){
		if (Legion.Instance.cTutorial.bIng) {

		}
	}

	public void SetPopup(QuestInfo info){

		for (int i=0; i<info.acReward.Length; i++) {
			if(info.acReward[i] == null) continue;
			GameObject temp = Instantiate(ItemSlot) as GameObject;
			temp.transform.SetParent(rewardsParent);
			temp.transform.localScale = Vector3.one;
			temp.GetComponent<UI_ItemListElement_Common>().SetData(info.acReward[i]);
			//temp.GetComponent<RewardButton>().SetButton(info.acReward[i].u1Type, info.acReward[i].u2ID);
		}

		AddExpPotion(info.u4RewardExp);

		if (lbl_content.preferredWidth > 200f) {
			IconTrans.anchoredPosition = new Vector2(IconTrans.anchoredPosition.x - ((lbl_content.preferredWidth-200f)/2f), IconTrans.anchoredPosition.y);
		}
	}

	public void SetPopup(AchievementInfo info){
		for (int i=0; i<info.acReward.Length; i++) {
			if(info.acReward[i] == null) continue;
			GameObject temp = Instantiate(ItemSlot) as GameObject;
			temp.transform.SetParent(rewardsParent);
			temp.transform.localScale = Vector3.one;
			temp.GetComponent<UI_ItemListElement_Common>().SetData(info.acReward[i]);
			//temp.GetComponent<RewardButton>().SetButton(info.acReward[i].u1Type, info.acReward[i].u2ID);
		}

		if (lbl_content.preferredWidth > 200f) {
			IconTrans.anchoredPosition = new Vector2(IconTrans.anchoredPosition.x - ((lbl_content.preferredWidth-200f)/2f), IconTrans.anchoredPosition.y);
		}
	}

	public void SetPopup(Goods[] acRewards){
		for (int i=0; i<acRewards.Length; i++) {
			if(acRewards[i] == null) continue;
			GameObject temp = Instantiate(ItemSlot) as GameObject;
			temp.transform.SetParent(rewardsParent);
			temp.transform.localScale = Vector3.one;
			temp.GetComponent<UI_ItemListElement_Common>().SetData(acRewards[i]);
			//temp.GetComponent<RewardButton>().SetButton(acRewards[i].u1Type, acRewards[i].u2ID);
		}

		if (lbl_content.preferredWidth > 200f) {
			IconTrans.anchoredPosition = new Vector2(IconTrans.anchoredPosition.x - ((lbl_content.preferredWidth-200f)/2f), IconTrans.anchoredPosition.y);
		}
	}

	public void SetPopup(AchieveItem info)
	{
		if(info == null)
			return;
		
		GameObject temp = Instantiate(ItemSlot) as GameObject;
		temp.transform.SetParent(rewardsParent);
		temp.transform.localScale = Vector3.one;
		temp.GetComponent<UI_ItemListElement_Common>().SetData(info);

		if (lbl_content.preferredWidth > 200f)
			IconTrans.anchoredPosition = new Vector2(IconTrans.anchoredPosition.x - ((lbl_content.preferredWidth-200f)/2f), IconTrans.anchoredPosition.y);
	}

	public void ShowMe(float delay){
		Invoke ("DelayShow", delay);
	}

	void DelayShow(){
		popup.SetActive (true);
		GetComponent<Animator> ().enabled = true;
	}
	
	public void CloseMe(){
		Destroy(gameObject);
	}

	//경험치물약을 위한 인벤 공간 검사
	void AddExpPotion(UInt32 u4Exp)
	{
		//경험치 계산
		Dictionary<UInt16, ConsumableItemInfo> tempConsumeItem = new Dictionary<ushort, ConsumableItemInfo>();
		List<ConsumableItemInfo> lstConsumeItem = new List<ConsumableItemInfo>();
		tempConsumeItem = ItemInfoMgr.Instance.GetConsumableItemInfo();
		Byte itemCnt = 0;
		for(int i=tempConsumeItem.Count; i>0;)
		{
			if(u4Exp < tempConsumeItem[(ushort)(58000+i)].u4Exp)
			{
				if(itemCnt != 0)
				{
					GameObject temp = Instantiate(ItemSlot) as GameObject;
					temp.transform.SetParent(rewardsParent);
					temp.transform.localScale = Vector3.one;
					temp.GetComponent<UI_ItemListElement_Common>().SetData(new Goods((Byte)GoodsType.CONSUME, (ushort)(58000+i), itemCnt));
					//temp.GetComponent<RewardButton>().SetButton((Byte)GoodsType.CONSUME, (ushort)(58000+i));
				}
				i--;
				itemCnt = 0;
				continue;
			}
			else
			{
				if(i != 4)
				{
					u4Exp -= tempConsumeItem[(ushort)(58000+i)].u4Exp;
					itemCnt++;
				}
				else
				{
					i--;
				}
			}
		}
	}

    public override void OnClickNoWithDest()
    {
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.ClearQuest);
        PopupManager.Instance.RemovePopup(gameObject);
        Destroy(gameObject);
    }
}
