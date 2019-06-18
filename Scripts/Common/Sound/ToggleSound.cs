using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Toggle))]
public class ToggleSound : MonoBehaviour {

	public AudioClip buttonClip;

	void Start()
	{
		Toggle button = GetComponent<Toggle> ();
		button.onValueChanged.AddListener((value) => {PlaySound(value);} );
	}

	void PlaySound(bool bOn)
	{
		if(bOn) SoundManager.Instance.PlayEff (buttonClip);
	}
}