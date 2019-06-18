using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_League_KeyInfo : MonoBehaviour
{
	public Text keyText;

	void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		if(keyText != null)
			keyText.text = string.Format("{0:#,##0}", Legion.Instance.LeagueKey);
            
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
            if(Legion.Instance.LeagueKey < LegionInfoMgr.Instance.leagueKeyTime.MAX_COUNT)
            {                
               TimeSpan timeSpan = Legion.Instance.nextLeagueEnergyChargeTime - Legion.Instance.ServerTime;
         
		       if(keyText != null)              
					keyText.text = string.Format("{2}/{3} ({0:00}:{1:00})", timeSpan.Minutes, timeSpan.Seconds, Legion.Instance.LeagueKey, LegionInfoMgr.Instance.leagueKeyTime.MAX_COUNT);
            }
            else
            {
                if(keyText != null)
                    keyText.text = string.Format("{0}/{1}", Legion.Instance.LeagueKey, LegionInfoMgr.Instance.leagueKeyTime.MAX_COUNT);
            }
            
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
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
