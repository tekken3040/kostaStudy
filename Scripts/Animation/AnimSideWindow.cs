using UnityEngine;
using System.Collections;


[RequireComponent(typeof(LeanTween))]
public class AnimSideWindow : MonoBehaviour {
	
	void Start()
	{
		LeanTween.moveLocalX(gameObject, 0f, 1f).setEase(LeanTweenType.easeInBack);
	}
}
