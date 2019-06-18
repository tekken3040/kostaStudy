#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

public class AnimationTest : MonoBehaviour {
	public GUIStyle fontStyle;

	AnimationTestCam cam;

	Vector2 scrollPosClass;
	Vector2 scrollPosAnim;

	bool bGameCam;
	bool bGameMove;

	Dictionary<ushort, ClassInfo> classData = new Dictionary<ushort, ClassInfo>();
	List<ushort> classIDs = new List<ushort>();
	List<string> aniNames = new List<string>();

	BattleCharacter btChar;
	GameObject cObject;
	BattleCharacter btTarget;
	GameObject cTarget;
	BattleCharacter btSubTarget;
	GameObject cSubTarget;
	Animator subAnimator;

	GameObject pChar;

	float range = 3f;
	float cross = 0f;

	Battle cBattle;
	Controller cCont;

	// Use this for initialization
	void Start () {
		Legion.Instance.eGameStyle = GameStyle.AnimTest;
		cCont = gameObject.AddComponent<Controller>();
		cBattle = gameObject.AddComponent<Battle>();
		cam = gameObject.GetComponent<AnimationTestCam>();
		pChar = AssetMgr.Instance.AssetLoad("Prefabs/Models/Character.prefab", typeof(GameObject)) as GameObject;

		TextManager.Instance.InitCommonText ();
		DataMgr.Instance.LoadAllStep(OnLoadStart, OnLoadFinished, true);
	}

	void OnLoadStart(){

	}

	void OnLoadFinished(){
		VFXMgr.Instance.Init();
		classData = ClassInfoMgr.Instance.GetClassListInfo();
		foreach (ushort key in classData.Keys) {
			classIDs.Add(key);
		}

		Monster tmpMonster = new Monster(102, "TestMonster");
		Level tmpLv = new Level ();
		tmpLv.u2Level = 1;
		tmpMonster.GetComponent<LevelComponent> ().Set (tmpLv);
		cTarget = Instantiate(pChar) as GameObject;
		
		tmpMonster.attachModel(cTarget);
		tmpMonster.attachAnimator(cTarget);

		Monster tmpMonster2 = new Monster(106, "TestMonster2");
		Level tmpLv2 = new Level ();
		tmpLv2.u2Level = 1;
		tmpMonster2.GetComponent<LevelComponent> ().Set (tmpLv);
		cSubTarget = Instantiate(pChar) as GameObject;
		
		tmpMonster2.attachModel(cSubTarget);
		tmpMonster2.attachAnimator(cSubTarget);

		cSubTarget.transform.position = Vector3.right;

		Crew tmCrew = new Crew(2);
		tmCrew.Assign(tmpMonster, 0);
		tmCrew.Assign(tmpMonster2, 1);

		cBattle.acCrews = new BattleCrew[2];
		cBattle.acCrews[1] = new BattleCrew(tmCrew, cBattle, 1);

		btTarget = cBattle.acCrews[1].acCharacters[0];
		//btTarget = new BattleCharacter(tmpMonster, cBattle, cBattle.acCrews[1]);
		btTarget.iTeamIdx = 1;
		btTarget.cObject = cTarget;
		btTarget.a_col.enabled = false;
		SetAnimator(cTarget, btTarget);

		btSubTarget = cBattle.acCrews[1].acCharacters[1];
		//btTarget = new BattleCharacter(tmpMonster, cBattle, cBattle.acCrews[1]);
		btSubTarget.iTeamIdx = 1;
		btSubTarget.cObject = cSubTarget;
		btSubTarget.a_col.enabled = false;
		SetAnimator(cSubTarget, btSubTarget);
	}

	void Update(){
		if (bGameCam) {
			cam.UpdateView ();
		} else {
			if(cObject != null){
				if (Input.GetKey (KeyCode.A)) {
					 Camera.main.transform.RotateAround (cObject.transform.position, Vector3.up, 90f * Time.deltaTime);
				} else if (Input.GetKey (KeyCode.D)) {
					Camera.main.transform.RotateAround (cObject.transform.position, Vector3.up, -90f * Time.deltaTime);
				}
				Camera.main.transform.LookAt(cObject.transform.position+Vector3.up*0.5f);

				if (Input.GetKey (KeyCode.W)) {
					if (Vector3.Magnitude(Camera.main.transform.position - cObject.transform.position) > 0.8f)
						Camera.main.transform.position += Camera.main.transform.forward*Time.deltaTime;
				} else if (Input.GetKey (KeyCode.S)) {
					if (Vector3.Magnitude(Camera.main.transform.position - cObject.transform.position) < 10.0f)
						Camera.main.transform.position -= Camera.main.transform.forward*Time.deltaTime;
				}
			}
		}

		if(cObject != null){
			if (bGameMove) {
				btChar.a_col.enabled = true;
			} else {
				btChar.a_col.enabled = false;
			}

			cTarget.transform.position = cObject.transform.position + cObject.transform.forward * range + cObject.transform.right * cross;
			cTarget.transform.LookAt(cObject.transform);

			cSubTarget.transform.position = cObject.transform.position + cObject.transform.forward * range + cObject.transform.right * cross + cObject.transform.right;
			cSubTarget.transform.LookAt(cObject.transform);
		}
	}
	
	void OnGUI(){
		GUILayout.BeginArea(new Rect(0f,0f,200f,Screen.height));

		GUILayout.Label("Class List", fontStyle, GUILayout.Height(30));
		GUILayout.Height(10);

		scrollPosClass = GUILayout.BeginScrollView(scrollPosClass, GUILayout.Width(200),GUILayout.Height(350));

		for (int i=0; i<classData.Count; i++) {
			if(GUILayout.Button (TextManager.Instance.GetText(classData[classIDs[i]].sName), GUILayout.Height (30))){
				SelectClass(classIDs[i]);
			}
		}

		GUILayout.EndScrollView();

		GUILayout.Label("Help", fontStyle);
		GUILayout.Height(10);
		GUILayout.Label("W : 확대", fontStyle);
		GUILayout.Label("S : 축소", fontStyle);
		GUILayout.Label("A : 회전-좌", fontStyle);
		GUILayout.Label("D : 회전-우", fontStyle);

		Vector3 dist = Vector3.zero;
		if (cObject != null) {
			dist = cTarget.transform.position - cObject.transform.position;
			range = GUILayout.HorizontalSlider(range, 1f, 10f);
			cross = GUILayout.HorizontalSlider(cross, -10f, 10f);
			GUILayout.Label("Dist = "+Vector3.Magnitude(dist).ToString("0.00"), fontStyle, GUILayout.Height(30));
			GUILayout.Label("AttackDist = "+(Vector3.Magnitude(dist)-(btChar.totalRad+btTarget.totalRad)).ToString("0.00"), fontStyle, GUILayout.Height(30));
		}
			
		GUILayout.EndArea();

		//Option
		GUILayout.BeginArea(new Rect(210f,0f,100f,Screen.height));

		bool tbGC = bGameCam;

		bGameCam = GUILayout.Toggle(bGameCam, "게임 카메라");
		bGameMove = GUILayout.Toggle(bGameMove, "게임 움직임");

		if (tbGC && !bGameCam) {
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 0.5f, Camera.main.transform.position.z);
		}

		GUILayout.EndArea();

		//Anim
		GUILayout.BeginArea(new Rect(Screen.width-200f,0f,200f,Screen.height));

		GUILayout.Label("Anim List", fontStyle, GUILayout.Height(30));
		GUILayout.Height(10);

		scrollPosAnim = GUILayout.BeginScrollView(scrollPosAnim, GUILayout.Width(200), GUILayout.Height(600));

		for (int i=0; i<aniNames.Count; i++) {
			if(GUILayout.Button (aniNames[i], GUILayout.Height (30))){
				SelectAnim(aniNames[i]);
			}
		}
		
		GUILayout.EndScrollView();

		GUILayout.EndArea();
	}

	void SelectClass(ushort id){
		if(cObject != null){
			Destroy(cObject);
			cObject = null;
			btChar = null;
			aniNames.Clear();
		}
		//classData
		if(id < 100){
			Hero tmpHero = new Hero(0, id, "TestCharacter", 1, 1, 1);
			tmpHero.GetComponent<LevelComponent>().Set(1, 0);
			tmpHero.DummyWear(new UInt16[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

			tmpHero.InitModelObject();
			tmpHero.cObject.GetComponent<HeroObject>().SetAnimations_Battle();

			cObject = tmpHero.cObject;
//			cObject = Instantiate(pChar) as GameObject;



			Crew thCrew = new Crew();
			thCrew.Fill(tmpHero, 1);
			cBattle.acCrews[0] = new BattleCrew(thCrew, cBattle, 1);

			btChar = new BattleCharacter(tmpHero, cBattle, cBattle.acCrews[0]);
		}else{
			Monster tmpMonster = new Monster(id, "TestCharacter");

			cObject = Instantiate(pChar) as GameObject;

			tmpMonster.attachModel(cObject);
			tmpMonster.attachAnimator(cObject);

			Crew thCrew = new Crew();
			thCrew.Assign(tmpMonster, 1);
			cBattle.acCrews[0] = new BattleCrew(thCrew, cBattle, 1);

			btChar = new BattleCharacter(tmpMonster, cBattle, cBattle.acCrews[0]);
		}
		btChar.cObject = cObject;
		SetAnimator(cObject, btChar);
		cam.SetChar(btChar);
		scrollPosAnim = Vector2.zero;
		btChar.cTarget = btTarget;
	}

	void SelectAnim(string animName){
		btChar.SubAnimationPlay(animName);
	}

	void SetAnimator(GameObject obj, BattleCharacter btChar){
		subAnimator = obj.transform.FindChild("Animator").GetComponentInChildren<Animator>();

		if (obj == cObject) {
			UnityEditor.Animations.AnimatorController ac = subAnimator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
			UnityEditor.Animations.AnimatorStateMachine sm = ac.layers [0].stateMachine;
			for (int i=0; i<sm.states.Length; i++) {
				aniNames.Add (sm.states [i].state.name);
			}
		}
	}
}
#endif
