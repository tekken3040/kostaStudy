using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class InvenClassData
{
	public int index;
	public UInt16 id;
}

// 인벤토리 왼쪽 클래스 리스트에 사용됨
public class InvenClassSlot : MonoBehaviour, ISlot<InvenClassData> {

	public delegate void OnClickClass(int index, UInt16 id);
	
	public OnClickClass onClickClass;
//	public RectTransform slotBG;
	public Text charaterName;
	public Image classIcon;
	public Image buttonBg;
	public Sprite _imgNormalButton;
	public Sprite _imgPressedButton;
    public GameObject alramIcon;
	
	public int index;
    public UInt16 id;
	
	public void InitSlot(InvenClassData classData)
	{
		index = classData.index;
        id = classData.id;
		ClassInfo classInfo = ClassInfoMgr.Instance.GetInfo(Convert.ToUInt16(classData.id));
		charaterName.text = TextManager.Instance.GetText(classInfo.sName);
		classIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Inventory/class_01.class_on_" + classInfo.u2ID);
		buttonBg.sprite = _imgNormalButton;
        charaterName.color = Color.gray;
		classIcon.SetNativeSize();
	}
	
    public void RefreshAlram(bool show)
    {
        alramIcon.SetActive(show);
    }
    
    // 온오프 처리
	public void SelectButton(bool selected)
	{
        if(selected)
        {
			buttonBg.sprite = _imgPressedButton;
//            classIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Inventory/class_01.class_on_" + id);
//            charaterName.color = Color.white;
        }
        else
        {
			buttonBg.sprite = _imgNormalButton;
//            classIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Inventory/class_01.class_off_" + id);
//            charaterName.color = Color.gray;
        }
        
//        classIcon.SetNativeSize();
	}
	
	public void OnClickEvent()
	{
		if(onClickClass != null)
			onClickClass(index, id);
	}
}
