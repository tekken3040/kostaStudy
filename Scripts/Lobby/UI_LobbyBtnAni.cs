using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UI_LobbyBtnAni : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public GameObject Img;
    public GameObject Text;
    public GameObject Notice;

    public void OnClickBtn()
    {
        //LeanTween.value(Img, 
    }

	void OnEnable(){
		SetSpriteAlpha(0);
	}

    public void OnPointerDown (PointerEventData eventData)
    { 
        for(int i=1; i<this.transform.parent.childCount-3; i++)
        {
            this.transform.parent.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            //LeanTween.cancel(this.transform.parent.GetChild(i).GetChild(0).GetComponent<Image>().gameObject);
            //LeanTween.cancel(this.transform.parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().gameObject);
            //DebugMgr.LogError(this.transform.parent.GetChild(i).gameObject);
        }
        
        LeanTween.value(Img, SetSpriteAlpha, 0f, 1f, 0.3f)/*.setOnComplete(() => {StartCoroutine(DelayTime());})*/;
        LeanTween.value(Img, SetSpriteRectSize, 0f, 274f, 0.3f);
        //DebugMgr.LogError("Pointer Down");
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        //LeanTween.value(Img, SetSpriteAlpha, 0f, 0f, 0.3f);
        //SetSpriteAlpha(0);
        //StartCoroutine(DelayTime());
    }

    IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(0.3f);
        SetSpriteAlpha(0);
    }

    public void SetSpriteAlpha(float val)
    {
        Img.GetComponent<Image>().color = new Color(1f, 1f, 1f, val);
    }

    public void SetSpriteRectSize(float val)
    {
        Img.GetComponent<RectTransform>().sizeDelta = new Vector2(val, 64f);
    }
}
