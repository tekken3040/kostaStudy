using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RepeatResult : MonoBehaviour {

	public UI_ItemListElement_Common _cRepeatItem;
    
    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
    }

	public void SetInfo(Goods goods)
	{
		_cRepeatItem.SetData(goods);
		PopupManager.Instance.AddPopup(gameObject, OnClickClose);
	}
}
