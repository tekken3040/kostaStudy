using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EventPopup : Popup {
	[SerializeField] Image _imgEventImage;
	//public delegate void OnClickEvent(object[] param);
	//public static event OnClickEvent OK

	// Use this for initialization
	new void Start () {
		base.Start();
	}

	event PopupManager.OnClickEvent _okEvent;
	object[] _okEventParam;
	public void Init(string url, PopupManager.OnClickEvent okEvent, object[] okEventParam)
	{
		_okEvent = okEvent;
		_okEventParam = okEventParam;

		StartCoroutine(downloadImage(url));
	}

	private IEnumerator downloadImage(string url)
	{
		
		WWW hsPost =  new WWW(url);
		
		// yield return www;
//		while (!hsPost.isDone && hsPost.error == null)
//		{
//			yield return null;
//		}
		yield return hsPost;
		if(hsPost.texture != null) DebugMgr.Log("다운로드 완료");
		Sprite spr = new Sprite();
		spr = Sprite.Create(hsPost.texture, new Rect(0,0,800,400), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, Vector4.one);
		_imgEventImage.sprite = spr;
//		_imgEventImage.sprite = 
	}

	public void OnClickImage()
	{
		if(_okEvent != null)
			_okEvent(_okEventParam);
	}

	public void OnClickClose()
	{
		Close();
        PopupManager.Instance.RemovePopup(gameObject);
	}
}
