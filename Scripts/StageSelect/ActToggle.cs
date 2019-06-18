using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ActToggle : MonoBehaviour {

	public delegate void OnClickToggle(UInt16 id);	
	public OnClickToggle onClickToggle;

    public Text actName;
    public GameObject alramObject;

    public Image btnImage;
    public Sprite defaultBtnSprite;
    public Sprite selectedBtnSprite;

    private UInt16 actID;
	private int index;
    private bool isSelected;
    public bool IsSelected { get { return isSelected; } }
    
    //public void SetToggle(UInt16 actID, int index, string name)
    public void SetToggle(ActInfo actInfo)
    {
        isSelected = false;
        actID = actInfo.u2ID;

        actName.text = TextManager.Instance.GetText(actInfo.strName);
        RefreshAlram();
    }

    public void RefreshAlram()
    {
        alramObject.SetActive(false);
        
        ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];
        
		// 2016. 6. 29 jy
		// 탑 모드는 별과 반복 보상이 없으므로 확인할 필요가 없다
		if(actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
			return;

		// 별 or 반복 보상 느낌표 처리
		for(int i=0; i<actInfo.lstChapterID.Count; i++)
	    {
			if(StageInfoMgr.Instance.dicChapterData[actInfo.lstChapterID[i]].RewardEnable())
			{
				alramObject.SetActive(true);
				break;
			}
		}             
    }
	
    // 클릭 이벤트
	public void OnValueChanged()
	{
        ChangedBtnSprite(true);
        onClickToggle(actID);
	}
    
    // 버튼 이미지 변경
    public void ChangedBtnSprite(bool isSelected)
    {
        this.isSelected = isSelected;
        if (isSelected == true)
            btnImage.sprite = selectedBtnSprite;
        else
            btnImage.sprite = defaultBtnSprite;
    }
}
