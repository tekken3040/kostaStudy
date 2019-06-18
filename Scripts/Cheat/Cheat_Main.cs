using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Cheat_Main : MonoBehaviour {
	[SerializeField] GameObject _objMainPanel;
	[SerializeField] GameObject[] _objSubPanel;
	Byte _u1SelectedMenu=1;

	void Awake()
	{
		// 2016. 10. 22 jy
		// 이벤트 패널에서 터치영역을 방해함
		// 유니티 엔진 상태에서만 활성화한다
		// 활성화가 DontDestory 스크립트가 작동하지 않아
		// 씬변경시 치트 메인은 자동으로 삭제된다
		#if UNITY_EDITOR
		#else
		this.gameObject.SetActive(false);
		#endif
	}

	public void OnClickOpen()
	{
		// 2016. 08. 23 jy 
		// 에디터 모드에서만 치트가 가능하도록 변경
		#if UNITY_EDITOR
		_objMainPanel.SetActive(!_objMainPanel.activeSelf);

		#endif
	}

	public void OnClickMenu(int menuNum)
	{
		_objSubPanel[(int)(_u1SelectedMenu-1)].SetActive(false);
		_objSubPanel[(int)(menuNum-1)].SetActive(true);

		_u1SelectedMenu=(Byte)menuNum;
	}

	public void OnClickStageClear()
	{
		#if UNITY_EDITOR
		GameCheat.ClearStage();
		#endif
	}

	public void ReLogin()
	{
		#if UNITY_EDITOR
		DataMgr.Instance.ReLoadUserData(null);
		#endif
	}
}
