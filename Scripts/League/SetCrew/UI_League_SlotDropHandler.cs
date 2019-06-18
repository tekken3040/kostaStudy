using UnityEngine;
using UnityEngine.EventSystems;

public class UI_League_SlotDropHandler : MonoBehaviour, IDropHandler
{
    public GameObject _dragObject;
    public GameObject Eff_Insert;
	public AudioClip insertSnd;

    #region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if(_dragObject.transform.childCount != 0)
        {
            Destroy(_dragObject.transform.GetChild(0).gameObject);
        }
        //DebugMgr.LogError("OnDrop at SlotItem");
        UI_League_Slot slotInfo = transform.GetComponent<UI_League_Slot>();

        if(UI_League_CharElementDragHandler.itemBeingDragged != null)
        {
            UI_League_CharElement listElement = UI_League_CharElementDragHandler.itemBeingDragged.GetComponent<UI_League_CharElement>();
            UI_League_CharElementDragHandler.itemBeingDragged = null;
            listElement.GetComponent<CanvasGroup>().blocksRaycasts = true;
            ExecuteEvents.ExecuteHierarchy<ChangeLeagueCharEvent> (gameObject, null, (x, y) => x.ChangedListToSlot (listElement.cHero, slotInfo.u1Pos));
            //Eff_Insert.GetComponent<ParticleSystem>().Play();
			SoundManager.Instance.PlayEff (insertSnd);
        }

        else if(UI_League_SlotDragHandler.itemBeingDragged != null)
		{
            //DebugMgr.LogError("slot to slot");
			UI_League_Slot charSlot = UI_League_SlotDragHandler.itemBeingDragged.GetComponent<UI_League_Slot> ();
			UI_League_SlotDragHandler.itemBeingDragged = null;

            ExecuteEvents.ExecuteHierarchy<ChangeLeagueCharEvent> (gameObject, null, (x, y) => x.ChangedSlotToSlot (slotInfo.cHero, charSlot.cHero, slotInfo.u1Pos, charSlot.u1Pos));
            //Eff_Insert.GetComponent<ParticleSystem>().Play();
			SoundManager.Instance.PlayEff (insertSnd);
		}
        
		else
		{
			return;
		}
	}
	#endregion
}
