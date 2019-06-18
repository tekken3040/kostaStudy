using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UI_CharElementDropHandler : MonoBehaviour, IDropHandler
{
    #region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		//DebugMgr.LogError("OnDrop at ListItem");
		if (UI_CharSlotDragHandler.itemBeingDragged == null) {
			return;
		}

		UI_CharSlot charSlot = UI_CharSlotDragHandler.itemBeingDragged.GetComponent<UI_CharSlot>();

		ExecuteEvents.ExecuteHierarchy<ChangeCharEvent>(gameObject,null,(x, y) => x.ChangedSlotToList(charSlot.cHero));

	}
	#endregion
}
