using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class LoadText : MonoBehaviour {
	public string Tag;
#if UNITY_EDITOR
	public void GetText()
	{        
		string text = TextManager.Instance.GetText(Tag);
		GetComponent<Text>().text = text;
//		GetComponent<Text>().SetAllDirty()
	}
#endif
	void OnEnable()
	{
		TextLoad();
        //if(Scene.GetCurrent() != null)
        //{
        //    if(Scene.GetCurrent().name == "LobbyScene")
        //    {
        //        if(!TextManager.Instance.lstTextObject.Contains(this.gameObject))
        //            TextManager.Instance.lstTextObject.Add(this.gameObject);
        //    }
        //}
	}

	public void TextLoad()
	{
		if(string.IsNullOrEmpty(Tag))
		{
			Tag = GetComponent<Text>().text; 
		}        

		GetComponent<Text>().text = TextManager.Instance.GetText( Tag );
	}
}
