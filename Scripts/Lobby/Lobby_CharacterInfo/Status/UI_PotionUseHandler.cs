using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UI_PotionUseHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UI_Status_UseExpPotion _parentScript;
    
    bool _checkPress = false;

    private DateTime pivotTime;
    private TimeSpan tempTime;

    #region IPointerDownHandler implementation
    public void OnPointerDown(PointerEventData eventData)
    {
		pivotTime = Legion.Instance.ServerTime;//DateTime.Now;
        StartCoroutine("CheckTimer");
    }
    #endregion

    #region IPointerUpHandler implementation
    public void OnPointerUp(PointerEventData eventData)
    {
        tempTime = Legion.Instance.ServerTime - pivotTime;//DateTime.Now - pivotTime ;
        _checkPress = false;
        if(tempTime.Milliseconds < 490)
        {
            _parentScript.OnClickUse();
        }
        StopCoroutine("CheckTimer");
    }
    #endregion
    
    IEnumerator CheckTimer()
    {
        yield return new WaitForSeconds(0.5f);
        _checkPress = true;
        _parentScript.OnClickUse();
        while(_checkPress)
        {
            if(_parentScript._selectedConsumableItem == null)
                yield break;
            if(_parentScript.GetHero.cLevel.u2Level >= Server.ConstDef.MaxHeroLevel)
            {
                _parentScript.OnClickUse();
                yield break;
            }
            if(_parentScript._selectedConsumableItem.u2Count - _parentScript._itemCnt > 0)
            {
                yield return new WaitForSeconds(0.1f);
                _parentScript.OnClickUse();
            }
            else
            {
                //_checkPress = false;
                yield break;
            }
        }
    }
}
