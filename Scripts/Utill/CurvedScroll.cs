using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class CurvedScroll : MonoBehaviour {

	[Range(1f, 2f)]
	public float curve = 2f;
	private Vector2 rectSize;
	private RectTransform content;
	private List<RectTransform> itemList = new List<RectTransform>();
	private bool init = false;

	public delegate void ChageEventFuntion(int index);
	private ChageEventFuntion chageEventFuntion;
	public void SrollEventFuntion(ChageEventFuntion funtion)
	{
		chageEventFuntion += funtion;
	}

	public void Init()
	{
		
		ScrollRect scrollRect = GetComponent<ScrollRect>();
		scrollRect.onValueChanged.AddListener( (x) => Change(x) );
		rectSize = GetComponent<RectTransform>().sizeDelta;
		content = scrollRect.content;
		init = true;
	}

	public void AddListItem(RectTransform item)
	{
		itemList.Add(item);
		ResetPosition();
	}

	public void ResetPosition()
	{
		Change(Vector2.zero);        
	}

	public void ClearItemList()
	{
		itemList.Clear();
	}

	private void Change(Vector2 pos)
	{
		if(!init)
			Init();

		for(int i=0; i<itemList.Count; i++)
		{
			float x = -(rectSize.x / 2f) + (rectSize.x / 2f) * Mathf.Cos((itemList[i].localPosition.y + content.localPosition.y) / rectSize.x * curve);
			itemList[i].localPosition = new Vector3(x, itemList[i].localPosition.y, 0f);
		}
		/*
		if(customListWinodw != null)
			customListWinodw.RefreshScrollBtn();
			*/
		if(chageEventFuntion != null)
			chageEventFuntion(-1);
	}

	public int ListItemCount()
	{
		if(itemList == null)
			return 0;

		return itemList.Count;
	}
}
