using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UI_League_CharElementDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject Pref_Cursor;
    public GameObject cursorRoot;
    public GameObject cursorObj;
    public GameObject canvas;

    public static GameObject itemBeingDragged;
    private GameObject _scrollArea;
    private Vector2 canvasSize;

    private DateTime pivotTime;
    private TimeSpan tempTime;

    bool _checkPress = false;

    void Awake()
    {
        Pref_Cursor = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/cursorCharacter_.prefab", typeof(GameObject));
        canvas = GameObject.Find("LeagueScene");
		canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
		cursorRoot = GameObject.Find("DragObject");
        _scrollArea = GameObject.Find("ScrollArea");
		cursorRoot.GetComponent<RectTransform>().sizeDelta = -canvasSize * Legion.Instance.ratio; 
    }
    #region IPointerUpHandler implementation
    public void OnPointerUp(PointerEventData eventData)
    {
        if(_checkPress)
        {
            //OnEndDrag(eventData);
        }
        else if(!eventData.dragging)
        {
            //OnEndDrag(eventData);
        }
        StopAllCoroutines();
    }
    #endregion
    #region IPointerDownHandler implementation
    public void OnPointerDown(PointerEventData eventData)
    {
		pivotTime = Legion.Instance.ServerTime;//DateTime.Now;

		//if (AssetMgr.Instance.CheckDivisionDownload (1, GetComponent<UI_CharElement> ().cHero.cClass.u2ID)) {
		//	DebugMgr.LogError (GetComponent<UI_CharElement> ().cHero.cClass.u2ID);
		//	return;
		//}

        
        StartCoroutine(CheckTimer(eventData));
    }
    #endregion
    IEnumerator CheckTimer(PointerEventData eventData)
    {
        yield return new WaitForSeconds(0.1f);
        _checkPress = true;
        cursorObj = Instantiate(Pref_Cursor) as GameObject;
        cursorObj.transform.SetParent(cursorRoot.transform);
        cursorObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
		cursorObj.transform.localScale = new Vector3(1.5f, 1.5f);
		cursorObj.transform.localPosition = new Vector3(0, 0, -700);
		cursorObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        itemBeingDragged = gameObject;
        cursorObj.GetComponent<UI_CharCursorElement>().SetData(this.GetComponent<UI_League_CharElement>().cHero);
        OnBeginDrag(eventData);
        OnDrag(eventData);
        eventData.dragging = true;
    }
    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
		tempTime = Legion.Instance.ServerTime - pivotTime;//DateTime.Now - pivotTime ;
        if(tempTime.Milliseconds < 99)
        {
            StopAllCoroutines();
            this.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;            
            _scrollArea.GetComponents<ScrollRect>()[0].OnBeginDrag(eventData);
            return;
        }
    }
    #endregion

    #region IDragHandler implementation
	public void OnDrag (PointerEventData eventData)
	{
        if(tempTime.Milliseconds < 99)
        {
            StopAllCoroutines();
            //OnEndDrag(eventData);
            _scrollArea.GetComponents<ScrollRect>()[0].OnDrag(eventData);
            return;
        }

        Vector2 v2 = new Vector2((Input.mousePosition.x/(float)Screen.width)*(canvasSize.x) - (canvasSize.y-100f), 
            (Input.mousePosition.y/(float)Screen.height)*(canvasSize.y) - (canvasSize.y-350f));
        
        if(cursorObj != null)
            cursorObj.GetComponent<RectTransform>().anchoredPosition = v2;
		//DebugMgr.LogError (v2);
	}
	#endregion

	#region IEndDragHandler implementation
	public void OnEndDrag (PointerEventData eventData)
	{
        //DebugMgr.LogError("in EndDrag");
        if(tempTime.Milliseconds < 99)
        {
            StopAllCoroutines();
            _scrollArea.GetComponents<ScrollRect>()[0].OnEndDrag(eventData);
            this.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
            return;
        }
        
        itemBeingDragged = null;
		cursorObj.GetComponent<CanvasGroup>().blocksRaycasts = true;
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(cursorObj);
	}
	#endregion
}
