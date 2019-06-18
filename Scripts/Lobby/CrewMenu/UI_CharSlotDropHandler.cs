using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UI_CharSlotDropHandler : MonoBehaviour, IDropHandler
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
        UI_CharSlot slotInfo = transform.GetComponent<UI_CharSlot>();

        if(UI_CharElementDragHandler.itemBeingDragged != null)
        {
            UI_CharElement listElement = UI_CharElementDragHandler.itemBeingDragged.GetComponent<UI_CharElement>();
            UI_CharElementDragHandler.itemBeingDragged = null;
            listElement.GetComponent<CanvasGroup>().blocksRaycasts = true;
            ExecuteEvents.ExecuteHierarchy<ChangeCharEvent> (gameObject, null, (x, y) => x.ChangedListToSlot (listElement.cHero, slotInfo.u1Pos));
            Eff_Insert.GetComponent<ParticleSystem>().Play();
			SoundManager.Instance.PlayEff (insertSnd);
        }

        else if(UI_CharSlotDragHandler.itemBeingDragged != null)
		{
            //DebugMgr.LogError("slot to slot");
			UI_CharSlot charSlot = UI_CharSlotDragHandler.itemBeingDragged.GetComponent<UI_CharSlot> ();
			UI_CharSlotDragHandler.itemBeingDragged = null;

			//ExecuteEvents.ExecuteHierarchy<ChangeCharEvent> (gameObject, null, (x, y) => x.ChangedSlotToSlot (slotInfo.cHero, charSlot.cHero, slotInfo.u1Pos, charSlot.u1Pos));
            ExecuteEvents.ExecuteHierarchy<ChangeCharEvent> (gameObject, null, (x, y) => x.ChangedSlotToSlot (slotInfo.cHero, charSlot.cHero, slotInfo.u1Pos, charSlot.u1Pos));
            Eff_Insert.GetComponent<ParticleSystem>().Play();
			SoundManager.Instance.PlayEff (insertSnd);
			//charSlot.cHero.cObject.SetActive (true);
		}
        
		else
		{
            if(slotInfo.cHero != null)
			    slotInfo.cHero.cObject.SetActive (true);
			return;
		}
	}
	#endregion
}
