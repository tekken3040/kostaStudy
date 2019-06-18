using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class UI_VIPInfoSlot : MonoBehaviour 
{
	public Text _textSlotTitle;
	public Text _textContents;
	private StringBuilder _contents = new StringBuilder();

	private int _nLineIndex = 0;
	public int LineIndex { get { return _nLineIndex; }}

	public void SetSlot(int lineCount)
	{
		if( lineCount > 0 ) 
		{
			if(_textContents != null)
				_textContents.text = _contents.ToString();
			
			gameObject.SetActive(true);
		}
		else
		{
			gameObject.SetActive(false);
		}

		_nLineIndex = 0;
		_contents.Remove(0, _contents.Length);
	}

	public void SetTitle(string titleKey)
	{
		if(_textSlotTitle != null)
			_textSlotTitle.text = TextManager.Instance.GetText(titleKey);
	}

	public void AddContents(string contentKey, int index)
	{
		if(_nLineIndex > 0 )
			_contents.Append("\n");
		_contents.Append(TextManager.Instance.GetText(contentKey + index.ToString()));
		++_nLineIndex;
	}

	public void AddContents(string contentKey, int index ,string param)
	{
		if(_nLineIndex > 0 )
			_contents.Append("\n");
		_contents.Append(TextManager.Instance.GetText(contentKey + index.ToString()).Replace("{0}", param));
		++_nLineIndex;
	}

	public void Clear()
	{
		_textSlotTitle.text = "";
		_textContents.text = "";

		_contents.Remove(0, _contents.Length);
		_nLineIndex = 0;
	}
}
