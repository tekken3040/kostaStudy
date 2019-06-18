using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UI_League_SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;
    public GameObject canvas;
	public GameObject cursorRoot;
	public GameObject cursorObj;
    public GameObject Pref_Cursor;
    public GameObject Eff_Eject;
    private Vector2 canvasSize;
	public Hero hero;

    void Awake ()
	{
		Pref_Cursor = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/cursorCharacter_.prefab", typeof(GameObject));
        canvas = GameObject.Find("LeagueScene");
		canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
		cursorRoot = GameObject.Find("DragObject");
	}

    #region IBeginDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
		UI_League_Slot charSlot = this.GetComponent<UI_League_Slot>();
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
		cursorObj.GetComponent<UI_CharCursorElement>().SetData(this.GetComponent<UI_League_Slot>().cHero);
        //hero.cObject.SetActive(false);
        //Eff_Eject.GetComponent<ParticleSystem>().Play();
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

	#region IEndDragHandler implementation
	public void OnEndDrag (PointerEventData eventData)
	{
		itemBeingDragged = null;
		if(hero == null) return;
        //hero.cObject.SetActive(true);
		cursorObj.GetComponent<CanvasGroup>().blocksRaycasts = true;
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(cursorObj);
	}
	#endregion
}
