using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class PressPopupButton : Selectable
{
    public RectTransform targetObjectTr;
    public Vector3 subPosition;

    public Button.ButtonClickedEvent OnPressEvent;

    public override void OnPointerDown(PointerEventData eventData)
    {
        // 누르는 상태 입장시 팝업이 있다면
        if (targetObjectTr != null)
        {
            // 이벤트가 존재 하면 이벤트 실행
            if (OnPressEvent != null)
            {
                OnPressEvent.Invoke();
            }
            // 타겟의 위치를 조정하며
            SetTargetObjPos(eventData.pressPosition);
            // 타겟을 활성화 한다
            targetObjectTr.gameObject.SetActive(true);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // 누르는 상태가 팝업 숨기기
        if (targetObjectTr != null)
        {
            targetObjectTr.gameObject.SetActive(false);
        }
    }

    // [개편 필요] 위치를 자동적으로 셋팅 할 필요성이 있음
    public void SetTargetObjPos(Vector3 pressPoint)
    {
        Vector3 popPos = this.GetComponent<RectTransform>().localPosition + subPosition;
        targetObjectTr.localPosition = popPos;
    }
}
