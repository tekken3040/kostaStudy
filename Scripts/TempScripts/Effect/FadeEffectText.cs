using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffectText : MonoBehaviour 
{
	public Text _Text;
	public CanvasGroup _AlphaControll;

	private string m_textValue;

	public void SetText(string textValue)
	{
		m_textValue = textValue;
		_Text.text = m_textValue;
	}

	public void StartTextEffect()
	{
		if(this.gameObject.activeSelf == false)
			this.gameObject.SetActive(true);
		
		StartCoroutine("TextEffect");
	}

	public void EndTextEffect()
	{
		this.gameObject.SetActive(false);
		_AlphaControll.alpha = 0f;
	}

	private IEnumerator TextEffect()
	{
		while(true)
		{
			_AlphaControll.alpha += Time.deltaTime * 3f;
			if(_AlphaControll.alpha  >= 1)
				break;

			yield return null;
		}

		yield return new WaitForSeconds(0.3f);

		// 불투명화 된 텍스트를 투명화 한다
		while(true)
		{
			_AlphaControll.alpha -= Time.deltaTime * 2.5f;
			if(_AlphaControll.alpha <= 0)
				break;

			yield return null;
		}
		EndTextEffect();
	}
}