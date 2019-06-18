using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

	int SelectCharIndex = 0;
	public Controller cCont;

	public UIController(Controller cont){
		cCont = cont;
	}

	public void SelectChar(int idx){
		DebugMgr.Log("SelectChar"+idx);
		SelectCharIndex = idx;
	}

	public void UseSkill(int idx){
		DebugMgr.Log("UseSkill"+idx);
		cCont.UseSkill (0, SelectCharIndex, idx);
	}
}
