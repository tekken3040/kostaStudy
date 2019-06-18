using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 2017. 02. 16 jy
// 별도 타임을 체크하는 팝업을 만들어야 하나 지나는 시간이 어긋날 수 있어 별로도 만듬
// 추후 나중에 Popup들에게 Time 체크를 할 수 있도록 추가할 예정
public class BlackShopTimePopup : YesNoPopup
{
    public Text _txtTimeTitle;
    public Text _txtTime;

    public void SetTimeEnable(bool isEnable)
    {
        if (_txtTimeTitle == null)
            _txtTimeTitle.gameObject.SetActive(isEnable);
    }

    public void SetTimeTitle(string title)
    {
        if (_txtTimeTitle != null)
            _txtTimeTitle.text = title;
    }
    public void SetTime(string time)
    {
        if (_txtTime != null)
            _txtTime.text = time;
    }
}
