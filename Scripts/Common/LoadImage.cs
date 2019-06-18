using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class LoadImage : MonoBehaviour {
	public string AtlasNumber;
	public string Tag;
	#if UNITY_EDITOR
	public void GetImage()
	{        
		ImageLoad ();
	}
	#endif
	void OnEnable()
	{
		ImageLoad();
	}

	public void ImageLoad()
	{
		if(string.IsNullOrEmpty(Tag))
		{
			Tag = GetComponent<Image>().sprite.name; 
		}        
#if UNITY_EDITOR
		Object[] list = AssetMgr.Instance.AssetLoadAll("Sprites/"+TextManager.Instance.GetImagePath()+AtlasNumber+".png", typeof(Sprite));
		for(int i = 1; i<list.Length; i++){
			if(list[i].name == Tag){
				GetComponent<Image>().sprite = (Sprite)list[i];
				return;
			}
		}
		DebugMgr.LogError("Sprite is not exists");
#else
		GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/"+TextManager.Instance.GetImagePath()+AtlasNumber+"."+Tag);
#endif

	}
}

