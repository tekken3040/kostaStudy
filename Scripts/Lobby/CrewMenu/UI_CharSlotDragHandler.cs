using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UI_CharSlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;
    public GameObject canvas;
	public GameObject cursorRoot;
	public GameObject cursorObj;
    public GameObject Pref_Cursor;
    public GameObject Eff_Eject;
	GameObject BeforeSlot;
    private Vector2 canvasSize;
	public Hero hero;

    void Awake ()
	{
		Pref_Cursor = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/cursorCharacter_.prefab", typeof(GameObject));
        canvas = GameObject.Find("LobbyScene");
		canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
		cursorRoot = GameObject.Find("DragObject");
	}

    #region IBeginDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
		UI_CharSlot charSlot = this.GetComponent<UI_CharSlot>();
		BeforeSlot = charSlot.gameObject;
		hero = charSlot.cHero;
		if(charSlot.cHero == null) return;

		cursorObj = Instantiate(Pref_Cursor) as GameObject;
		cursorObj.transform.SetParent(cursorRoot.transform);
        cursorObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
		cursorObj.transform.localScale = new Vector3(1.5f, 1.5f);
		cursorObj.transform.localPosition = Vector3.zero;
		cursorObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;
		itemBeingDragged = gameObject;
		cursorObj.GetComponent<UI_CharCursorElement>().SetData(this.GetComponent<UI_CharSlot>().cHero);
		hero.DestroyModelObject ();
        Eff_Eject.GetComponent<ParticleSystem>().Play();
		GetComponent<Button>().interactable = false;
	}
	#endregion

	#region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
		if(hero == null) return;
		Vector2 v2 = new Vector2((Input.mousePosition.x/(float)Screen.width)*(canvasSize.x) - (canvasSize.y-100f), 
            (Input.mousePosition.y/(float)Screen.height)*(canvasSize.y) - (canvasSize.y-350f));
        
        cursorObj.GetComponent<RectTransform>().anchoredPosition = v2;
	}
	#endregion

	void OnApplicationPause(bool bPause)
	{
		if (bPause) 
		{
			if(hero == null) return;

			ReturnIcon ();
		}
	}

	void ReturnIcon()
	{
		hero.cObject.SetActive (true);

		itemBeingDragged = null;

		GetComponent<Button> ().interactable = true;
		this.GetComponent<CanvasGroup> ().blocksRaycasts = true;
		Destroy (cursorObj);
	}

	#region IEndDragHandler implementation
	public void OnEndDrag (PointerEventData eventData)
	{
		if(hero == null) return;

		if (eventData.pointerCurrentRaycast.gameObject == null) {
			ReturnIcon ();
			return;
		}

		if (eventData.pointerCurrentRaycast.gameObject.transform != null)
        {
			if(eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<UI_CharElementDropHandler>() == null)
            {
				if (eventData.pointerCurrentRaycast.gameObject.transform.GetComponent<UI_CharSlotDropHandler> () == null)
                {
					hero.SetActiveWithBeforeParent ();
				}
                else
                {
					if (BeforeSlot == eventData.pointerCurrentRaycast.gameObject)
                        hero.SetActiveWithBeforeParent ();
					else
                        hero.cObject.SetActive (true);
				}
			}
		}

		itemBeingDragged = null;

		GetComponent<Button> ().interactable = true;
		this.GetComponent<CanvasGroup>().blocksRaycasts = true;
//		cursorObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
//        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(cursorObj);
	}
	#endregion
}
