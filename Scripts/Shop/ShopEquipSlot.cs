using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//장비 상점 아래쪽 슬롯
public class ShopEquipSlot : MonoBehaviour, ISlot<ShopSlotData> {

	public delegate void OnClickSlot(int index);
	public OnClickSlot onClickSlot;
    
    public Button button;
	public Image equipIcon;
    public Image equipGrade;
    public Text equipLevel;
    
    public GameObject soldOut;
	public GameObject m_SelectedIcon;
    
	public ShopSlotData slotData;
    public Text _txtStarCnt;
	public Image _imgStarGrade;

    
    //아이템 정보 세팅
	public void InitSlot(ShopSlotData slotData)
	{
		this.slotData = slotData;
        
        EquipmentItem equipmentItem = new EquipmentItem(slotData.shopItem.u2ItemID);

        ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(equipmentItem.GetEquipmentInfo().u2ModelID);

        if(modelInfo != null)
        {
            if(slotData.shopItem.cEquipInfo != null)
            {
                ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[slotData.shopItem.cEquipInfo.u1SmithingLevel-1];
				_imgStarGrade.sprite = equipGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);    
            }
            
            string imagePath = "Sprites/Item/" + modelInfo.sImagePath;         
            equipIcon.sprite = AtlasMgr.Instance.GetSprite(imagePath);
             
            equipIcon.transform.localScale = Vector3.one;
            equipIcon.SetNativeSize();
            equipGrade.SetNativeSize();
        }
        
        //품절 체크
		CheckSoldOut();
	}

	public void CheckSoldOut()                                               
	{
		if(slotData.shopItem.u1SoldOut == 1)
		{
			AtlasMgr.Instance.SetGrayScale(equipIcon);
			AtlasMgr.Instance.SetGrayScale(equipGrade);
           
			soldOut.SetActive(true);
			button.interactable = false;
			equipLevel.text = "";
            _txtStarCnt.transform.parent.gameObject.SetActive(false);
			m_SelectedIcon.SetActive(false);
		}
		else
		{
			AtlasMgr.Instance.SetDefaultShader(equipIcon);
			AtlasMgr.Instance.SetDefaultShader(equipGrade);
            
			soldOut.SetActive(false);
			button.interactable = true;
            _txtStarCnt.transform.parent.gameObject.SetActive(true);
			equipLevel.text = slotData.shopItem.cEquipInfo.u2Level.ToString();
            _txtStarCnt.text = slotData.shopItem.cEquipInfo.u1Completeness.ToString();
		}
	}

	public void SelectedIconEnabled(bool bSelected)
	{
		if(m_SelectedIcon != null)
			m_SelectedIcon.SetActive(bSelected);	
	}

	public void OnClickEvent()
	{
		if(onClickSlot != null)
		{
			if(slotData.shopItem.u1SoldOut == 1)
				return;

			onClickSlot(slotData.index);
		}
	}
}
