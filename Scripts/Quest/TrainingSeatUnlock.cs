using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[System.Serializable]
public class UnlockMaterials
{
    public Image itemIcon;
    public Image itemGrade;
    public Text itemName;
    public Text itemCount;
    public GameObject checkMark;
}

// 잠긴 자리 클릭스 열리는 팝업
public class TrainingSeatUnlock : MonoBehaviour {

    public delegate void OnClickUnlock();
    public OnClickUnlock onClickUnlock;
	public Text _textRoomTitle;
    public UnlockMaterials[] unlockMaterias;
    
    // 재료를 클릭하면 어디서 얻을 수 있는지 바로가기 가능한 팝업
    public UI_SubPanel_Forge_Smithing_MaterialShortcut shortcut;
    
	private int NeedCount = 0;
    private int enoughCount = 0;
    private MaterialItemInfo[] materialItemInfos; // 필요한 재료 목록

    private TrainingInfo trainingInfo;


    public void SetWindow(UInt16 roomID, bool isEqiup)
    {       
        if(!isEqiup)
        {
			_textRoomTitle.text = TextManager.Instance.GetText("popup_title_chartrain_slot_unlock");
            trainingInfo = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
        }
        else
        {
			_textRoomTitle.text = TextManager.Instance.GetText("popup_title_equiptrain_slot_unlock");
            trainingInfo = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
        }        
        
        enoughCount = 0;
		NeedCount = 0;
        
        materialItemInfos = new MaterialItemInfo[unlockMaterias.Length];
        
        for(int i=0; i<unlockMaterias.Length; i++)
        {            
            // 재료 정보 세팅
			if (trainingInfo.arrUnlockGoods [i].u2ID != 0) 
			{
				unlockMaterias [i].itemName.enabled = true;
				unlockMaterias [i].itemCount.enabled = true;
				unlockMaterias [i].itemGrade.enabled = true;
				unlockMaterias [i].itemIcon.enabled = true;

				MaterialItemInfo materialInfo = ItemInfoMgr.Instance.GetMaterialItemInfo (trainingInfo.arrUnlockGoods [i].u2ID);
	            
				unlockMaterias [i].itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo (trainingInfo.arrUnlockGoods [i].u2ID).u2IconID) as Sprite;
				unlockMaterias [i].itemGrade.sprite = AtlasMgr.Instance.GetGoodsGrade (trainingInfo.arrUnlockGoods [i]);
				unlockMaterias [i].itemName.text = TextManager.Instance.GetText (materialInfo.sName);
				materialItemInfos [i] = materialInfo;
	         
				int count = 0;
	            
				if (Legion.Instance.cInventory.dicItemKey.ContainsKey (trainingInfo.arrUnlockGoods [i].u2ID)) {
					UInt16 slotNum = Legion.Instance.cInventory.dicItemKey [trainingInfo.arrUnlockGoods [i].u2ID];
					MaterialItem materialItem = (MaterialItem)Legion.Instance.cInventory.dicInventory [slotNum];
					count = materialItem.u2Count;
				} else {
					count = 0;
				}
				NeedCount++;
	            
				unlockMaterias [i].itemCount.text = count + "/" + trainingInfo.arrUnlockGoods [i].u4Count;
	            
				if (count >= trainingInfo.arrUnlockGoods [i].u4Count) {
					unlockMaterias [i].checkMark.SetActive (true);
					enoughCount++;
				} else {
					unlockMaterias [i].checkMark.SetActive (false);
				}
			} else {
				unlockMaterias [i].itemName.enabled = false;
				unlockMaterias [i].itemCount.enabled = false;
				unlockMaterias [i].itemGrade.enabled = false;
				unlockMaterias [i].itemIcon.enabled = false;
				unlockMaterias[i].checkMark.SetActive (false);
			}
        }
    }
    
    public void OnClickOK()
    {
        if(onClickUnlock != null)
        {
            // 재료가 부족하면 자리를 열 수 없음
			if(enoughCount < NeedCount)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_training_mat_not_enough"), TextManager.Instance.GetText("popup_desc_training_mat_not_enough"), null);
            }
            else
            {
                onClickUnlock();
                OnClickClose();
            }
        }
    }
    
    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
    
    // 바로가기 창 오픈 
    public void OpenShorcut(int index)
    {
		if (materialItemInfos [index] == null)
			return;

        for(int i = 0;i < trainingInfo.arrUnlockGoods.Length;++i)
        {
            if(trainingInfo.arrUnlockGoods[i].u2ID == materialItemInfos[index].u2ID)
            {
                StageInfoMgr.Instance.u4CurTargetItemCount = trainingInfo.arrUnlockGoods[i].u4Count;
                break;
            }
        }
		
        shortcut.SetData(materialItemInfos[index]);
        shortcut.gameObject.SetActive(true);
    }
}
