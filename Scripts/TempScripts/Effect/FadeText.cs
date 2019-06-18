using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class FadeText : MonoBehaviour {
	RectTransform _myTransform;
	Text cText;
	Text cDamage;
	Gradient cGradient;
	Gradient cGradient2;
	Shadow cOutline;
	Shadow cOutline2;
	float fontSize;

	Color32 _startColor;
	Color32 _endColor;

	//Color OutlineColor = Color.white;

	CanvasGroup group;

	bool bAnim = false;

	bool bDown = false;
	float downSpeed = 5.0f;

	void Awake () {
		_myTransform = GetComponent<RectTransform>();
		cGradient = GetComponent<Gradient>();
		cGradient2 = transform.GetChild(0).GetComponent<Gradient>();

		cDamage = GetComponent<Text>();
		cText = transform.GetChild(0).GetComponent<Text>();

		cOutline = cDamage.GetComponent<Shadow>();
		cOutline2 = cText.GetComponent<Shadow>();

		group = GetComponent<CanvasGroup>();
		state = Anim_state.stop;
	}

//	void Start(){
//		cOutline.effectColor = OutlineColor;
//	}
	
//	public void SetText (string text, Color color, bool anim, float time) {
//		cText.text = text;
//		//cText.color = color;
//		cGradient.StartColor = new Color(color.r, color.g, color.b, 1);
//		cGradient.EndColor = new Color(color.r/2f, color.g/2f, color.b/2f, 1);
//
//	}

	public void SetText (string text, BattleUIMgr.DAMAGE_TEXT_TYPE textType, Color outline, bool anim, float time, int txtSize = 38) {

		Color32 startColor = new Color32(252, 242, 76, 255);
		Color32 endColor = new Color32(239, 149, 39, 255); 

		switch(textType)
		{
		case BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_HERO:
			startColor = new Color32 (247, 150, 70, 255);
			endColor = new Color32 (247, 100, 70, 255);
			bDown = true;
			break;
//		case DAMAGE_TEXT_TYPE.GUARD:
//				startColor = new Color32(146, 208, 80, 255);
//				endColor = new Color32(0, 208, 80, 255);
//				break;
		case BattleUIMgr.DAMAGE_TEXT_TYPE.DAMAGED_MONSTER:
			startColor = new Color32 (255, 255, 0, 255);
			endColor = new Color32 (255, 200, 0, 255);
			bDown = false;
			break;
//		case DAMAGE_TEXT_TYPE.CRITICAL:
//				startColor = new Color32(190, 15, 15, 255);
//				endColor = new Color32(255, 183, 67, 255);
//				break;
		case BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_HERO:
			startColor = new Color32 (146, 208, 80, 255);
			endColor = new Color32 (0, 208, 80, 255);
			bDown = false;
			break;
		case BattleUIMgr.DAMAGE_TEXT_TYPE.HEAL_MONSTER:
			startColor = new Color32(75, 172, 198, 255);
			endColor = new Color32(75, 100, 198, 255);
			bDown = false;
			break;
//		case DAMAGE_TEXT_TYPE.AVOID:
//				startColor = new Color32(0, 22, 238, 255);
//				endColor = new Color32(1, 68, 139, 255);
//				break;
		case BattleUIMgr.DAMAGE_TEXT_TYPE.CHAIN_DMG:
			startColor = new Color32(200, 50, 20, 255);
			endColor = new Color32(255, 0, 0, 255);
			bDown = false;
			break;
		case BattleUIMgr.DAMAGE_TEXT_TYPE.CHAIN_HEAL:
			startColor = new Color32(0, 255, 198, 255);
			endColor = new Color32(68, 150, 170, 255);
			bDown = false;
			break;
		case BattleUIMgr.DAMAGE_TEXT_TYPE.COMBO_DMG:
			startColor = new Color32(150, 100, 255, 255);
			endColor = new Color32(210, 0, 255, 255);
			bDown = false;
			break;
		case BattleUIMgr.DAMAGE_TEXT_TYPE.REVIVE_HERO:
			time = 0.5f;
			startColor = new Color32 (146, 208, 80, 255);
			endColor = new Color32 (0, 208, 80, 255);
			bDown = false;
			break;
		}


		string[] temp = text.Split('\n');
		if (temp.Length == 1) {
			cText.text = "";
			cDamage.text = text;
		}else{
			cText.text = temp[0];
			if(temp[1] != "0") cDamage.text = temp[1];
			else cDamage.text = "";
		}
//		_startColor = startColor;
//		_endColor = endColor;
		cGradient.StartColor = startColor;
		cGradient.EndColor = endColor;

		cGradient2.StartColor = startColor;
		cGradient2.EndColor = endColor;

		cOutline.effectColor = outline;
		cOutline2.effectColor = outline;

		cText.fontSize = txtSize;
		cDamage.fontSize = txtSize;
//		OutlineColor = outline;

		animTime=time;
		alpha = 1f;
		group.alpha = alpha;
	//	state = Anim_state.expandText;
		state = Anim_state.contractText;
		bAnim = anim;

		if (bAnim)
			_myTransform.localScale = new Vector3 (2f, 2f, 2f);
		else
			_myTransform.localScale = Vector3.one;
	}


//	void Update()
//	{
//
//	}

	enum Anim_state
	{
		stop,
		expandText,
		contractText,
		wait,
		fadeText,
	}
	Anim_state state;

	float animTime=0f;
	float alpha;
	void FixedUpdate()
	{
//		cOutline.effectColor = OutlineColor;
//		cOutline2.effectColor = OutlineColor;

		if (bAnim)
		{
			switch (state) {
			case Anim_state.expandText:
	//			_myTransform.localPosition += Vector3.up * 30f;
				_myTransform.localScale += Vector3.one * 0.5f;
				if (_myTransform.localScale.x > 3f) {
					state = Anim_state.contractText;
				}
				break;
			case Anim_state.contractText:
	//			_myTransform.localPosition += Vector3.up * 30f;
				_myTransform.localScale -= Vector3.one * 0.4f;
				if (_myTransform.localScale.x <= 1f) {
					_myTransform.localScale = Vector3.one;
					state = Anim_state.wait;
				}
				break;
			case Anim_state.wait:
				animTime -= Time.fixedDeltaTime;
				if (animTime <= 0f) {
					animTime = 0.05f;
					state = Anim_state.fadeText;
				}
				break;
			case Anim_state.fadeText:
				alpha -= Time.fixedDeltaTime * downSpeed;
				group.alpha = alpha;
	//			cGradient.StartColor = new Color32(_startColor.r, _startColor.g, _startColor.b, alpha);
	//			cGradient.EndColor = new Color32(_endColor.r, _endColor.g, _endColor.b, alpha);
	//			cGradient.ReDraw();
	//			cOutline.effectColor = new Color32(0, 0, 0, alpha);
				//transform.localScale += new Vector3(-0.1f, -0.1f, 0f);
				if(bDown) transform.localPosition -= Vector3.up * downSpeed;
				else transform.localPosition += Vector3.up * downSpeed;

				if (alpha <= 0) {
					state = Anim_state.stop;
					gameObject.SetActive (false);
				}
				break;
			}
		}
	}
}
