using UnityEngine;
using System.Collections;

public class GameCheat{

	public static void ClearStage()
	{
		Battle battle = GameObject.FindObjectOfType<Battle>();
		if(battle != null)
			battle.ClearStage();
	}

}
