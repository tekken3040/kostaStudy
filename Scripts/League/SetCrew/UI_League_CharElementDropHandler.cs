using UnityEngine;
using UnityEngine.EventSystems;

public class UI_League_CharElementDropHandler : MonoBehaviour, IDropHandler
{
    #region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		//DebugMgr.LogError("OnDrop at ListItem");

		if(UI_League_SlotDragHandler.itemBeingDragged == null) return;

		UI_League_Slot leagueSlot = UI_League_SlotDragHandler.itemBeingDragged.GetComponent<UI_League_Slot>();

		ExecuteEvents.ExecuteHierarchy<ChangeLeagueCharEvent>(gameObject,null,(x, y) => x.ChangedSlotToList(leagueSlot.cHero));

	}
	#endregion
}
