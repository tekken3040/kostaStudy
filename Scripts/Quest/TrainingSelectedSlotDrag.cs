using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

// 왼쪽에 선택된 슬롯
public class TrainingSelectedSlotDrag : MonoBehaviour {

    public delegate void OnDropSlot(int index);
    public OnDropSlot onDropSlot; 

    public static GameObject dragObject;
    
    public TrainingWindow trainingWindow;
    private int index;
	public int Index { get { return index; }}
    private int itemIndex;
    private int slotIndex;
    //private bool pressed = false;
    private float time;    
    private Vector2 canvasSize;
    
    public void SetIndex(int index, int itemIndex, int slotIndex)
    {
        this.index = index;
        this.itemIndex = itemIndex;    
        this.slotIndex = slotIndex;  
    }
   
    public int GetItemIndex()
    {
        return itemIndex;
    }

	public void OnClick()
	{
		if(onDropSlot != null)
			onDropSlot(this.index);
	}
//    public void OnDrop(int selectedIndex, int itemIndex, int slotIndex)
//    {
//		if (onDropSlot != null) {
//			onDropSlot (index);
//		}
//    }
//
//    public void OnPointerUp(PointerEventData eventData)
//    {        
//        if(!pressed && onDropSlot != null && trainingWindow.selectedSlots[index].seatType == -1)
//            onDropSlot(index);
//        
//        pressed = false;
//        StopCoroutine("CheckPressTime");        
//        
//        if(dragObject != null)		
//            dragObject.GetComponent<TrainingSlot>().DestroyObject();
//    }
//    
//    // 목록 스크롤과 구분하기 위해 누른 시간을 체크
//    public void OnPointerDown(PointerEventData eventData)
//    {
//        time = Time.time;
//        StartCoroutine("CheckPressTime", eventData);
//    }    
//    
//    private IEnumerator CheckPressTime(PointerEventData eventData)
//    {
//        yield return new WaitForSeconds(0.2f);
//        
//        pressed = true;
//        
//        if(trainingWindow.selectedSlots[index].seatType != 1)
//            yield break;
//        
//        // 드래그 아이템 생성
//        dragObject = Instantiate(trainingWindow.SlotObject()) as GameObject;
//        dragObject.transform.SetParent(trainingWindow.transform);
//        dragObject.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
//        dragObject.transform.localPosition = Vector3.zero;
//        dragObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
//        
//        trainingWindow.ShowEffect(dragObject.transform);
//        
//        TrainingSlot slot = dragObject.GetComponent<TrainingSlot>();
//        slot.SetSlot(itemIndex, slotIndex, trainingWindow.IsEquip);
//        slot.onClickSlot = OnDrop;
//        
//        canvasSize = transform.root.GetComponent<RectTransform>().sizeDelta;
//        
//        OnBeginDrag(eventData);
//        OnDrag(eventData);
//        eventData.dragging = true;
//		SoundManager.Instance.PlayEff ("Sound/UI/05. Crew/UI_Button_Icon_2");
//    }    
//    
//	public void OnBeginDrag (PointerEventData eventData)
//	{
//        if(!pressed)
//        {
//            StopCoroutine("CheckPressTime");
//            return;
//        }
//	}
//    
//	public void OnDrag (PointerEventData eventData)
//	{		
//        if(!pressed)
//        {
//            StopCoroutine("CheckPressTime");
//            return;
//        }        
//        
//        if(dragObject == null)
//            return;       
//            
//        Vector2 pos = new Vector2((Input.mousePosition.x / (float)Screen.width) * canvasSize.x - 50f, (Input.mousePosition.y / (float)Screen.height) * canvasSize.y + 50f);
//        
//        dragObject.GetComponent<RectTransform>().anchoredPosition = pos;
//	}
//
//	public void OnEndDrag (PointerEventData eventData)
//	{        
//        if(dragObject != null)		
//            dragObject.GetComponent<TrainingSlot>().DestroyObject();
//	}    
}
