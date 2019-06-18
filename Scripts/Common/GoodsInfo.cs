using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


//씬마다 있는 재화 정보 처리
public class GoodsInfo : MonoBehaviour {

	public Text cashText;
	public Text goldText;
	public Text keyText;
    public Text friendText;

	void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
        if (this.gameObject.activeSelf == false)
            return;

        if (cashText != null)
			cashText.text = string.Format("{0:#,##0}", Legion.Instance.Cash);
		if(goldText != null)
			goldText.text = string.Format("{0:#,##0}", Legion.Instance.Gold);
		if(keyText != null)
			keyText.text = string.Format("{0:#,##0}", Legion.Instance.Energy);
        if(friendText != null)
            friendText.text = string.Format("{0:#,##0}", Legion.Instance.FriendShipPoint);

        //StopCoroutine("CheckKeyTime");
        //StartCoroutine("CheckKeyTime");
        StartCoroutine("DelayedRefreashKeyTime");
	}

    private IEnumerator DelayedRefreashKeyTime()
    {
        StopCoroutine("CheckKeyTime");
        yield return new WaitForEndOfFrame();
        StartCoroutine("CheckKeyTime");
    }
    
    private IEnumerator CheckKeyTime()
    {
        while (true)
        {
            if(Legion.Instance.Energy < LegionInfoMgr.Instance.keyTime.MAX_COUNT)
            {                
               TimeSpan timeSpan = Legion.Instance.nextEnergyChargeTime - Legion.Instance.ServerTime;
//                if (timeSpan 0)
//                {
//                    Legion.Instance.u2Energy++;
//                    continue;
//                }
                    
		       if(keyText != null)              
                    keyText.text = string.Format("{2}/{3} {0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds, Legion.Instance.Energy, LegionInfoMgr.Instance.keyTime.MAX_COUNT);      
            }
            else
            {
                if(keyText != null)
                    keyText.text = string.Format("{0}/{1}", Legion.Instance.Energy, LegionInfoMgr.Instance.keyTime.MAX_COUNT);                
            }
            
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
    }

	// 친구 포인트 버튼 클릭시 소셜 패널을 오픈한다
	public void OnClickFriendGoods()
	{
		// 현재 씬이 로비가 아니라면 작동 시키지 않는다
		LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
		if(lobbyScene == null)
			return;
		
		lobbyScene.OnClickSocial(0);
	}
    
    public void OnClickGoods(int index)
    {
        StartCoroutine(OpenShop(index));
    }

    private IEnumerator OpenShop(int index)
    {
        yield return Scene.GetCurrent().StartCoroutine(Scene.GetCurrent().ShowShop(true, index));
    }
}
