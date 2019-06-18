using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemSellResult : MonoBehaviour {

    public Text resultText;
    public void SetText(string itemText)
    {
        resultText.text = itemText;
        PopupManager.Instance.AddPopup(gameObject, OnClick);
    }
    
    public void OnClick()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
    }
}
