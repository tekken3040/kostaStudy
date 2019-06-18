using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatResetPopup : MonoBehaviour {

    public Text title;
    public Text content;

	public GameObject disObj;
	public DiscountUI disScript;

    public void Set(string title, string content)
    {
        this.title.text = title;
        this.content.text = content;
    }

	public void SetDiscount(uint u4Price, byte u1DisPer){
		if (u1DisPer == 0)
			return;
		
		disObj.SetActive (true);
		disScript.SetData (u4Price, u1DisPer);
	}
}
