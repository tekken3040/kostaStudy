using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrayScaleUI : MonoBehaviour {

	void Start()
	{
		AtlasMgr.Instance.SetGrayScale(GetComponent<Image>());
	}
}
