using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Button))]
public class RewardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	private Byte itemType;
	private UInt16 itemID;
    private RectTransform rectTransform;
    
    private bool click = false;
	public void SetButton(Byte gtype, UInt16 itemID)
	{
		itemType = gtype;
		this.itemID = itemID;
        rectTransform = GetComponent<RectTransform>();
	}
    
    public void OnPointerDown(PointerEventData eventData)
    {
        click = true;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        //itemType = itemType == 0 ? (byte)GoodsType.RANDOM_REWARD : itemType;
        if(click && eventData.selectedObject == gameObject)
        {
			PopupManager.Instance.ShowItemInfo(itemType, itemID, (transform.root.position - transform.position), transform.root.localScale.x);                                        
        }
    }
}
