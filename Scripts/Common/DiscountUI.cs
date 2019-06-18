using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiscountUI : MonoBehaviour {
	public Text originalPrice;
	public Text cancelLine;
	public Text discountPer;

	public void SetData(uint u4Price, byte u1DisPer){
		if (u1DisPer == 0) {
			gameObject.SetActive (false);
			return;
		}
		originalPrice.text = u4Price.ToString ();
		string cancelText = "";
		for (int i = 0; i < originalPrice.text.Length; i++) {
			cancelText += "-";
		}
		cancelLine.text = cancelText;
		discountPer.text = u1DisPer.ToString ()+"%";
	}
}
