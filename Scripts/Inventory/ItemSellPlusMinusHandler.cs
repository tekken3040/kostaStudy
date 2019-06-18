using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class ItemSellPlusMinusHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ItemSellWindow _parentScript;
    public int _type; //0:minus, 1:minus10, 2:plus, 3:plus10
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
            OnClickBtn(_type);
        }
        StopCoroutine("CheckTimer");
    }
    #endregion
    
    public void OnClickBtn(int type)
    {
        switch(type)
        {
            case 0:
                _parentScript.OnClickMinus();
                break;

            case 1:
                _parentScript.OnClickMinus10();
                break;

            case 2:
                _parentScript.OnClickPlus();
                break;

            case 3:
                _parentScript.OnClickPlus10();
                break;
        }
    }

    IEnumerator CheckTimer()
    {
        yield return new WaitForSeconds(0.5f);
        _checkPress = true;
        OnClickBtn(_type);
        while(_checkPress)
        {
            if(_parentScript.GetSellCount < 2)
            {
                yield break;
            }
            else if(_parentScript.GetSellCount >= _parentScript.GetItemCount)
            {
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                OnClickBtn(_type);
            }
        }
    }
}
