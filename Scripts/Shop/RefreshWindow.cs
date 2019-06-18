using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 상점 정보 갱신 창
public class RefreshWindow : MonoBehaviour {
    
    public delegate void OnClickRefresh();
    public OnClickRefresh onClickRefresh;

    public Transform objItemParent;
    public Text priceValue;
    
    private int priceType;
    public int price;
   
    
    public void SetInfo(int priceType, int price)
    {
        priceValue.text = price.ToString();
        this.priceType = priceType;
        this.price = price;    
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);                
    }
    
    public void OnClick()
    {
		if(!Legion.Instance.CheckEnoughGoods(priceType, (long)price))
		{
			PopupManager.Instance.ShowChargePopup((byte)priceType);
            OnClickClose();
            return;
		}        
        
        if(onClickRefresh != null)
            onClickRefresh();
    }
    
    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
        if(objItemParent != null)
            objItemParent.gameObject.SetActive(true);
    }
}
