using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour {

	public AudioClip buttonClip;

	void Start()
	{
		Button button = GetComponent<Button> ();
		button.onClick.AddListener(PlaySound);
	}

	void PlaySound()
	{
		if(buttonClip != null) SoundManager.Instance.PlayEff (buttonClip);
	}
}
