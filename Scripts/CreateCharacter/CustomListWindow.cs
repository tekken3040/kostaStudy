using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class CustomListWindow : MonoBehaviour {

	private const int UI_LAYER = 5;

	public CreateCharacterScene createCharacterScene;
	public CurvedScroll[] curvedScorlls;
	public RectTransform[] contents;
//	public float[] characterScales;
//  public Vector3[] characterPosition;
	public Transform characterPos;
//	public Animator zoomAnimator;
	private GameObject costumeItem;
	private ClassInfo classInfo;
	private bool initHairList = false;
	private bool initHairColorList = false;
	private bool initFaceList = false;
	private Vector3[] contentsOriginPos;
	private CanvasGroup[] canvasGroups;
	private bool initCharacter = false;
	private Hero hero;
	private HeroObject heroObject;
	private bool zoomed = false;

    private float topBorder = 35f;

	// 2016. 7. 15 jy
	// 캐릭터 생성창 리뉴얼
	private bool m_bInitClassInfo = false;
	public CreateClassInfoWindow m_cClassInfoWindow;

	// 2016. 7. 15 jy
	// 카메라 이동
	public Transform m_tr3DCamera;
	public Camera m_3DCamera;
	private Vector3 m_vecCamInitPos;
	private Vector3 m_vecTargetPos;
	public Vector3[] m_arrCharacterCameraCenter;

	private bool m_bCamMoving = false;
	private int m_nTogleIndex;
	public bool IsCamMoving
	{
		get { return m_bCamMoving; }
		set { m_bCamMoving = value; }
	}

	private Byte m_u1SelectClassLockType;
	public Byte SelectClassLockType { get {return m_u1SelectClassLockType; } }
	public LockClassInfo m_cLockClassInfo;

	private float m_fMenuSlotSizeY = 0f;

	public Scrollbar[] _scrollBar;
	public Button _scrollUpBtn;
	public Button _scrollDownBtn;

	void Awake()
	{
		m_nTogleIndex = 0;
		contentsOriginPos = new Vector3[contents.Length];
		canvasGroups = new CanvasGroup[contents.Length];
		for(int i=0; i<contentsOriginPos.Length; i++)
		{
			contentsOriginPos[i] = contents[i].localPosition;
			canvasGroups[i] = contents[i].GetComponent<CanvasGroup>();
		}

		// 카메라의 초기 값을 저장한다
		m_vecCamInitPos = m_tr3DCamera.position;
		// 카메라가 첫 타겟의 바라보도록 셋팅한다
		m_tr3DCamera.LookAt(m_arrCharacterCameraCenter[0]);
		if(m_tr3DCamera.eulerAngles.x > 300)
			m_tr3DCamera.rotation = Quaternion.Euler(new Vector3(0, m_tr3DCamera.eulerAngles.y, m_tr3DCamera.eulerAngles.z));

		for(int i = 0; i < curvedScorlls.Length; ++i)
		{
			curvedScorlls[i].SrollEventFuntion(RefreshScrollBtn);
		}
	}

	private void CreateCharacter(int classID)
	{
        PopupManager.Instance.ShowLoadingPopup(1);
        StartCoroutine(WaitInit(classID));
	}
    
    private IEnumerator WaitInit(int classID)
    {
		hero = new Hero((byte)classID, (byte)classID, "", 1, 1, 1);
		//hero.Wear(new UInt16[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        hero.DummyWear(new UInt16[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
		hero.InitModelObject(true);
        Transform character = hero.cObject.transform;        
        heroObject = character.GetComponent<HeroObject>();
        
        while(heroObject.initData == false)
            yield return null;

		character.SetParent(characterPos);

		character.localPosition = Vector3.zero;
		character.localEulerAngles = Vector3.zero;
		character.localScale = Vector3.one;

		m_vecTargetPos = m_arrCharacterCameraCenter[classID-1];
        heroObject.SetAnimations_UI();
        heroObject.PlayAnim("UI_Class_Select"); 
		SoundManager.Instance.PlayEff ("Sound/UI/06. Character/UI_Select_Class_"+classID);
        PopupManager.Instance.CloseLoadingPopup();
        
        initCharacter = true;
    }


    // 클래스 목록 생성
	public void SetClassList()
	{
		Dictionary<ushort, ClassInfo> classData = ClassInfoMgr.Instance.GetClassListInfo();
		GameObject classItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/CreateCharacter/CreateCharacter_Class.prefab", typeof(GameObject)) as GameObject;

		string atlasPath = "Sprites/CreateCharacter/Character_Creat_01.";
		Vector2 size = classItem.GetComponent<RectTransform>().sizeDelta;
        //size += new Vector2(0f, 2f);
		m_fMenuSlotSizeY = (size.y * 0.5f) + topBorder;
        int count = 0;
		/*
        foreach(ushort key in classData.Keys)
		{
			if(classData[key].u1MonsterType != 0)
				continue;

			count++;
        }
*/
		//contents[0].sizeDelta = new Vector2(contents[0].sizeDelta.x, size.y * count + topBorder);// + 20f);
		//count = 0;
        int tempID = 0;
		/*
		foreach(ushort key in classData.Keys)
		{
			if(Legion.Instance.CheckClassAvailable(key) == 1)
			{
				tempID = key;
				break;
			}
		}
		*/
		foreach(ushort key in classData.Keys)
		{			
			ushort id = key;
			int index = count;

			if(classData[id].u1MonsterType != 0)
				continue;
				
			if(Legion.Instance.cTutorial.au1Step[0] < Server.ConstDef.LastTutorialStep)
			{
				if(Legion.Instance.charAvailable[id-1] == 0)
					continue;
			}
			Byte classLockType = Legion.Instance.CheckClassAvailable(id);
			string className = TextManager.Instance.GetText(classData[id].sName);
			Sprite classIcon;
			if(classLockType == 0)		// 잠김
			{
				className += "\n" +  TextManager.Instance.GetText("mark_lock");
				classIcon= AtlasMgr.Instance.GetSprite(atlasPath + "class_Lock");
			}
			else if(classLockType == 1)		// 오픈
			{
				if(tempID == 0)
					tempID = key;
				
				classIcon= AtlasMgr.Instance.GetSprite(atlasPath + "class_" + id);
			}
			else   				// 기타 숨김
			{
				continue;
			}

			GameObject item = Instantiate(classItem) as GameObject;
			RectTransform itemRect = item.GetComponent<RectTransform>();
			itemRect.SetParent(contents[0]);
			itemRect.localPosition = new Vector3(0f, (-size.y / 2f) + (-size.y * count) - topBorder, 0f);
			itemRect.localScale = Vector3.one;

			CustomItem customItem = item.GetComponent<CustomItem>();
			customItem.SetItem(className, classIcon);

			Toggle toggle = item.GetComponent<Toggle>();
			toggle.group = contents[0].GetComponent<ToggleGroup>();

			toggle.onValueChanged.AddListener( (x) => OnClickClassMenu(id, index, toggle) );

			ToggleColor toggleColor = item.GetComponent<ToggleColor>();
			//toggleColor.defaultColor = Color.black;
			//toggleColor.changeColor = Color.white;
			//toggleColor.SetDefaultColor();

			curvedScorlls[0].AddListItem(itemRect);

			if(tempID == id)
				toggle.isOn = true;
			else
				toggle.isOn = false;

			count++;
		}
		contents[0].sizeDelta = new Vector2(contents[0].sizeDelta.x, size.y * count + topBorder);// + 20f);
	}

    // 헤어 목록 생성
	public void SetHairList(int classIndex)
	{
		if(initHairList)
			return;

		if(costumeItem == null)
			costumeItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/CreateCharacter/CreateCharacter_Item.prefab",
			                            typeof(GameObject)) as GameObject;

		classInfo = ClassInfoMgr.Instance.GetInfo(Convert.ToUInt16(classIndex));

		string atlasPath = "Sprites/CreateCharacter/Character_Creat_01.";
		
		Vector2 size = costumeItem.GetComponent<RectTransform>().sizeDelta;
		m_fMenuSlotSizeY = (size.y * 0.5f) + topBorder;
		contents[1].sizeDelta = new Vector2(contents[1].sizeDelta.x, size.y * classInfo.lstHairInfo.Count + topBorder);// + 20f);

		for(int i=0; i<classInfo.lstHairInfo.Count; i++)
		{
			ushort index = classInfo.lstHairInfo[i].u2ID;
			int count = i;
			
			string hair = TextManager.Instance.GetText("btn_hair_type") + " " + (i + 1);
			
			Sprite selectIcon = AtlasMgr.Instance.GetSprite(atlasPath + "select_mark");
			
			GameObject item = Instantiate(costumeItem) as GameObject;

			RectTransform itemRect = item.GetComponent<RectTransform>();
			itemRect.SetParent(contents[1]);
			itemRect.localPosition = new Vector3(0f, (-size.y / 2f) + (-size.y * i) - topBorder, 0f);
			itemRect.localScale = Vector3.one;
			
			CustomItem customItem = item.GetComponent<CustomItem>();
			customItem.SetItem(hair, selectIcon);
			
			Toggle toggle = item.GetComponent<Toggle>();
			toggle.group = contents[1].GetComponent<ToggleGroup>();
			toggle.onValueChanged.AddListener( (x) => OnClickHairMenu(index, count, toggle) );

            curvedScorlls[1].AddListItem(itemRect);
            
            if(i == 0)
			{
				createCharacterScene.HairIndex = i+1;
                toggle.isOn = true;
			}
            else
                toggle.isOn = false;
		}

		initHairList = true;
	}
    
    // 헤어 컬러 목록 생성
    public void SetHairColorList(int classIndex)
	{
		if(initHairColorList)
			return;

		if(costumeItem == null)
			costumeItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/CreateCharacter/CreateCharacter_Item.prefab",
			                            typeof(GameObject)) as GameObject;

		classInfo = ClassInfoMgr.Instance.GetInfo(Convert.ToUInt16(classIndex));
		
		string atlasPath = "Sprites/CreateCharacter/Character_Creat_01.";
		
		Vector2 size = costumeItem.GetComponent<RectTransform>().sizeDelta;
		m_fMenuSlotSizeY = (size.y * 0.5f) + topBorder;
		contents[2].sizeDelta = new Vector2(contents[2].sizeDelta.x, size.y * classInfo.lstHairColor.Count + topBorder );//+ 20f);

		for(int i=0; i<classInfo.lstHairColor.Count; i++)
		{
			ushort index = classInfo.lstHairColor[i].u2ID;
			int count = i;
			
			string hair = TextManager.Instance.GetText(classInfo.lstHairColor[i].strColor);

			Color iconColor = new Color32(classInfo.lstHairColor[i].R[0], classInfo.lstHairColor[i].G[0], classInfo.lstHairColor[i].B[0], 255);
	
			Sprite selectIcon = AtlasMgr.Instance.GetSprite(atlasPath + "select_mark");
			
			GameObject item = Instantiate(costumeItem) as GameObject;
			RectTransform itemRect = item.GetComponent<RectTransform>();
			itemRect.SetParent(contents[2]);
			itemRect.localPosition = new Vector3(0f, (-size.y / 2f) + (-size.y * i) - topBorder, 0f);
			itemRect.localScale = Vector3.one;
			
			CustomItem customItem = item.GetComponent<CustomItem>();
			customItem.SetItem(hair, selectIcon);
			customItem.SetImageColor(iconColor);
			
			Toggle toggle = item.GetComponent<Toggle>();
			toggle.group = contents[2].GetComponent<ToggleGroup>();
			toggle.onValueChanged.AddListener( (x) => OnClickHairColorMenu(index, count, toggle) );

			ToggleImage toggleImage = item.GetComponent<ToggleImage>();
			toggleImage.image.gameObject.SetActive(true);
			toggleImage.changeType = ToggleImage.ChangeType.None;

            curvedScorlls[2].AddListItem(itemRect);
            
            if(i == 0)
			{
				createCharacterScene.HairColorIndex = i+1;
                toggle.isOn = true;
			}
            else
                toggle.isOn = false;
        }

		initHairColorList = true;
    }
    
    // 얼굴 목록 생성
    public void SetFaceList(int classIndex)
    {
		if(initFaceList)
			return;

        if(costumeItem == null)
			costumeItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/CreateCharacter/CreateCharacter_Item.prefab",
			                            typeof(GameObject)) as GameObject;

		classInfo = ClassInfoMgr.Instance.GetInfo(Convert.ToUInt16(classIndex));
		
		string atlasPath = "Sprites/CreateCharacter/Character_Creat_01.";
		
		Vector2 size = costumeItem.GetComponent<RectTransform>().sizeDelta;
		m_fMenuSlotSizeY = (size.y * 0.5f) + topBorder;
		contents[3].sizeDelta = new Vector2(contents[3].sizeDelta.x, size.y * classInfo.lstFaceInfo.Count + topBorder);

		for(int i=0; i<classInfo.lstFaceInfo.Count; i++)
		{
			ushort index = classInfo.lstFaceInfo[i].u2ID;
			int count = i;
			
			string hair = TextManager.Instance.GetText("btn_face_type") + " " + (i + 1);
			
			Sprite selectIcon = AtlasMgr.Instance.GetSprite(atlasPath + "select_mark");
			
			GameObject item = Instantiate(costumeItem) as GameObject;
			RectTransform itemRect = item.GetComponent<RectTransform>();
			itemRect.SetParent(contents[3]);
			itemRect.localPosition = new Vector3(0f, (-size.y / 2f) + (-size.y * i) - topBorder, 0f);
			itemRect.localScale = Vector3.one;
			
			CustomItem customItem = item.GetComponent<CustomItem>();
			customItem.SetItem(hair, selectIcon);
			
			Toggle toggle = item.GetComponent<Toggle>();
			toggle.group = contents[3].GetComponent<ToggleGroup>();
			toggle.onValueChanged.AddListener( (x) => OnClickFaceMenu(index, count, toggle) );
            
            curvedScorlls[3].AddListItem(itemRect);
            
            if(i == 0)
			{
                toggle.isOn = true;
				createCharacterScene.FaceIndex = i+1;
			}
            else
                toggle.isOn = false;
        }

		initFaceList = true;
	}

	//클래스 변경
	public void OnClickClassMenu(int id, int index, Toggle toggle)
	{
		if(toggle.isOn)
		{
			m_u1SelectClassLockType = Legion.Instance.CheckClassAvailable((UInt16)id);

			if (m_u1SelectClassLockType == 1) {
				if (AssetMgr.Instance.CheckDivisionDownload (1, id)) {
					PopupManager.Instance.AddDownloadCompleteEvent (() => OnClickClassMenu (id, index, toggle));
					return;
				}
			}
			
			if(createCharacterScene.ClassID == id)
				return;

			createCharacterScene.ClassID = id;
            createCharacterScene.SetCharacteDefaultName();

            DebugMgr.Log("Class : " + id);
						
			for(int i=1; i<contents.Length; i++)
			{
				for(int j=0; j<contents[i].childCount; j++)
				{
					Destroy(contents[i].GetChild(j).gameObject);
                }
                
                curvedScorlls[i].ClearItemList();
				contents[i].localPosition = contentsOriginPos[i];
			}

			initHairList = false;
			initHairColorList = false;
			initFaceList = false;
			initCharacter = false;
			m_bInitClassInfo = false;

			if(heroObject != null && heroObject.gameObject != null)
				Destroy(heroObject.gameObject);

			SetClassInfo(id);
			if(m_u1SelectClassLockType == 1)
			{
				m_cLockClassInfo.gameObject.SetActive(false);
				SetHairList(id);
				SetHairColorList(id);
				SetFaceList(id);
				CreateCharacter(id);
				createCharacterScene.doneButton.GetComponent<Button>().interactable = true;
			}
			else if(m_u1SelectClassLockType == 0)
			{
				m_cLockClassInfo.SetLockClassInfo((UInt16)id);
				m_cLockClassInfo.gameObject.SetActive(true);
				createCharacterScene.doneButton.GetComponent<Button>().interactable = false;
			}
			else
			{
				m_cLockClassInfo.gameObject.SetActive(false);
				createCharacterScene.doneButton.GetComponent<Button>().interactable = false;
			}
        }
	}

    //머리 변경
    public void OnClickHairMenu(int id, int index, Toggle toggle)
	{
		if(toggle.isOn)
		{
			if(createCharacterScene.HairIndex == index+1)
				return;

			ChangeHair(id);
			createCharacterScene.HairIndex = index+1;
			DebugMgr.Log("Hair : " + id);
		}
	}
	
	//머리색 변경
	public void OnClickHairColorMenu(int id, int index, Toggle toggle)
	{
		if(toggle.isOn)
		{
			if(createCharacterScene.HairColorIndex == index+1)
				return;

			ChangeHairColor(id);
			createCharacterScene.HairColorIndex = index+1;
			DebugMgr.Log("Color : " + id);
		}
	}
	
	//얼굴 변경
	public void OnClickFaceMenu(int id, int index, Toggle toggle)
    {
		if(toggle.isOn)
		{
			if(createCharacterScene.FaceIndex == index+1)
				return;

			ChangeFace(id);
			createCharacterScene.FaceIndex = index+1;
			DebugMgr.Log("Face : " + id);
		}
    }

	private void ChangeHair(int id)
	{
		if(heroObject != null && initCharacter)
		{
			int beforeIndex = hero.u1SelectedHair-1;
			int afterIndex = classInfo.lstHairInfo.FindIndex( (x) => x.u2ID == id);

			ModelInfo beforeModel = classInfo.lstHairInfo[beforeIndex].cModelInfo;
			ModelInfo afterModel = classInfo.lstHairInfo[afterIndex].cModelInfo;

			heroObject.ChangeHair(beforeModel, afterModel);
			hero.u1SelectedHair = (byte)(afterIndex+1);
			ChangeHairColor(classInfo.lstHairColor[hero.u1SelectedHairColor-1].u2ID);
			heroObject.SetAnimations_UI();
		}
	}

	private void ChangeHairColor(int id)
	{
		if(heroObject != null & initCharacter)
		{
			int afterIndex = classInfo.lstHairColor.FindIndex( (x) => x.u2ID == id);
		
			heroObject.SetHairColor(classInfo.lstHairColor[afterIndex]);
			hero.u1SelectedHairColor = (byte)(afterIndex+1);
			heroObject.SetAnimations_UI();
		}
	}

	private void ChangeFace(int id)
	{
		if(heroObject != null & initCharacter)
		{
			int beforeIndex = hero.u1SelectedFace-1;
			int afterIndex = classInfo.lstFaceInfo.FindIndex( (x) => x.u2ID == id);
			hero.ChangeFace((byte)afterIndex);
			heroObject.SetAnimations_UI();
		}
	}

	public void ZoomInOut(/*bool zoom,*/int togleIndex)
	{	
		/*
		if(zoom == true)
		{
			heroObject.OnOffHelmet(false);
			zoomAnimator.Play("ZoomIn");
		}
		else
		{
			heroObject.OnOffHelmet(true);
			zoomAnimator.Play("ZoomOut");
		}
		*/
		RefreshScrollBtn(togleIndex);
		// 메뉴가 변경되면 스크롤 버튼을 새로 고침한다
		// 2016. 7. 15 jy 
		// 3D 카메라 이동 
		if(m_nTogleIndex == togleIndex)
		{
			m_bCamMoving = false;
			return;
		}

		StopCoroutine("CamZoom");
		StopCoroutine("CamUpAndDown");
		bool bZoom = true;
		if( togleIndex == 0 ) 
		{
			// 커스텀 매뉴중 클래스 메뉴을 클릭시 카메라를 줌 아웃 한다
			bZoom = false;
			StartCoroutine(CamZoom(togleIndex));
		}
		else if( togleIndex == 3 )// 커스텀 매뉴중 얼굴 메뉴을 클릭시
		{
			 // 현재 선택되어 있는 메뉴가 헤어 or 헤어 컬러라면 카메라를 낮추고 아니라면 카메라를 줌 인한다
			if( m_nTogleIndex == 1 || m_nTogleIndex == 2)
				StartCoroutine(CamUpAndDown(false));
			else
				StartCoroutine(CamZoom(togleIndex));
		}
		else
		{
			// 현재 선택되어 있는 메뉴가 클래스 메뉴 면 카메라를 줌 인한다
			if( m_nTogleIndex == 0 )
				StartCoroutine(CamZoom(togleIndex));
			else if( m_nTogleIndex == 3 ) 
				StartCoroutine(CamUpAndDown(true));
			else
				m_bCamMoving = false;
		}

		m_nTogleIndex = togleIndex;
		zoomed = bZoom;
		heroObject.OnOffHelmet(!zoomed);
	}

	public void ShowAnim(int index)
	{
		canvasGroups[index].alpha = 0f;
		LeanTween.value(canvasGroups[index].gameObject, 0f, 1f, 0.15f).setDelay(0.02f).setOnUpdate((float alpha)=>{canvasGroups[index].alpha = alpha;});
	}
    
    public void PlayDoneAnim()
    {
        heroObject.PlayAnim("UI_Create"); 
		SoundManager.Instance.PlayEff ("Sound/UI/06. Character/UI_Create_Class_"+classInfo.u2ID);
    }
    
    public bool CheckEndAnim()
    {
        return heroObject.IsPlaying("UI_Create");
    }

	// 2016. 7. 18 jy
	// 클래스 정보장 셋팅
	private void SetClassInfo(int classID)
	{
		if( m_bInitClassInfo == true )
			return;

		ClassInfo classInfo = ClassInfoMgr.Instance.GetInfo((UInt16)classID);
		if(classInfo == null)
		{
			DebugMgr.LogError("classIndex not ClassInfo");
			return;
		}

		m_cClassInfoWindow.SetClassInfo(classInfo);
		m_bInitClassInfo = true;
	}
	// 2016. 7. 18 jy
	// 카메라 줌 인 
	private IEnumerator CamZoom(int nZoomType)
	{
		float fDistance = 0;			// 타겟과의 거리를 담을 변수
		float fDistanceGap = 0.01f;		// 타겟과의 갭을 담을 변수 [카메라 축소시 정확히 원하는 위치에 도달하기 힘들기에 최소 값으로 0.01 셋팅]
		float fTime = 0;				// 프레임 축척 타임을 담을 변수
		Vector3 vecTargetPos;			// 타겟의 위치를 담을 변수
		// 줌 타입은 0. 클래스 1. 헤어 2. 헤어컬러 3. 얼굴 순 탑 메뉴
		if( nZoomType != 0 )			 	// <= 줌 타입이 클래스 메뉴가 아니라면 카메라 ZoomIn 준비를 한다
		{
			fDistanceGap = 0.5f;
			vecTargetPos = m_vecTargetPos;
			if(nZoomType != 3)				// <= ZoomIn 타입중 얼굴이 아니라면 타겟의 높이를 0.07f 더하여 카메라의 위치를 위로 올린다
				vecTargetPos.y += 0.07f;
		}
		else 
			vecTargetPos = m_vecCamInitPos;	// <= ZoomOut시 카메라의 초기 위치를 타겟으로 잡는다

		while(true)
		{
			fTime += Time.deltaTime;
			fDistance = Vector3.Distance(vecTargetPos, m_tr3DCamera.position);	// 타겟과 현재 카메라의 거리를 구한다
			if( fDistance >= fDistanceGap )										// 현재 나온거리와 카메라와의 갭차이보다 크면 카메라를 위치를 컨트롤 한다
			{
				m_tr3DCamera.position = Vector3.Lerp(m_tr3DCamera.position, vecTargetPos, fTime * 0.2f);
				m_tr3DCamera.LookAt(m_vecTargetPos);
				// 예외로 카메라가 타겟을 잡게할때 각도가 0이하로 값이내려가면 360로 값이 반전 되므로 300 보다 값이 크다면 값을 0으로 강제 변경한다
				if(m_tr3DCamera.eulerAngles.x > 300)
					m_tr3DCamera.rotation = Quaternion.Euler(new Vector3(0, m_tr3DCamera.eulerAngles.y, m_tr3DCamera.eulerAngles.z));

				// 헤어 및 헤어컬러 선택 ZoomIn시 fieldOfView 를 변경 요청 받아 간단하게 처리하도록함
				if( nZoomType == 1 || nZoomType == 2)
				{
					if(m_3DCamera.fieldOfView < 46)
						m_3DCamera.fieldOfView += 0.1f;
					else
						m_3DCamera.fieldOfView = 46;
				}
				else if( nZoomType == 0 )
				{
					if(m_3DCamera.fieldOfView > 43)
						m_3DCamera.fieldOfView -= 0.1f;
					else
						m_3DCamera.fieldOfView = 43;
				}
			}
			else
			{
				
				// 카메라 이동시 원하는 위치에 정확히 도착할 수 없어
				// 카메라 이동이 계속 될시 카메라 어긋나게 될 수 있으므로
				// 줌 아웃시 카메라가 이동이 끝낫다면 카메라의 기본값을 재 셋팅한다
				if(nZoomType == 0)
					m_tr3DCamera.position = m_vecCamInitPos;

				m_bCamMoving = false;
				break;
			}
			yield return null;
		} 
	}

	// 2016. 7. 18 jy
	// 카메라 상하 움직임 
	private IEnumerator CamUpAndDown(bool isUp)
	{
		// CamZoom(int nZoomType) 과 비슷함
		Vector3 vecTargetPos = m_tr3DCamera.position;
		float fTime = 0;
		float fFieldViewValue = 0;
		float fValue = 0;
		if(isUp)
		{
			vecTargetPos.y += 0.06f;
			fValue = 0.3f;
		}
		else
		{
			vecTargetPos.y -= 0.06f;
			fValue = -0.3f;
		}

		while( true )
		{
			fTime += Time.deltaTime;
			if(Vector3.Distance(vecTargetPos, m_tr3DCamera.position) >= 0.002f ) 
			{
				m_tr3DCamera.position = Vector3.Lerp(m_tr3DCamera.position, vecTargetPos, fTime);
				m_tr3DCamera.LookAt(m_vecTargetPos);
				if(m_tr3DCamera.eulerAngles.x > 300)
					m_tr3DCamera.rotation = Quaternion.Euler(new Vector3(0, m_tr3DCamera.eulerAngles.y, m_tr3DCamera.eulerAngles.z));
				
				if( isUp )
				{
					if(m_3DCamera.fieldOfView < 46)
						m_3DCamera.fieldOfView += fValue;
					else
						m_3DCamera.fieldOfView = 46;
				}
				else
				{
					if(m_3DCamera.fieldOfView > 43)
						m_3DCamera.fieldOfView += fValue;
					else
						m_3DCamera.fieldOfView = 43;
				}
			}
			else
			{
				m_tr3DCamera.position = vecTargetPos;
				m_bCamMoving = false;
				break;
			}
			yield return null;
		}
	}

	public void OnClickScrollUp()
	{
		
		int itemCount = curvedScorlls[m_nTogleIndex].ListItemCount();
		if(itemCount > 4)
			itemCount -= 4;
		
		_scrollBar[m_nTogleIndex].value += (float)((float)1 / (float)itemCount);

		/*
		if(itemCount > 4)
			itemCount -= 4;

		Vector3 pos = contents[m_nTogleIndex].anchoredPosition3D;
		if( (contents[m_nTogleIndex].anchoredPosition3D.y - m_fMenuSlotSizeY) > 0)
		{
			pos.y -= m_fMenuSlotSizeY;
			contents[m_nTogleIndex].anchoredPosition3D = pos;
		}
		else
		{
			pos.y = 0f;
			contents[m_nTogleIndex].anchoredPosition3D = pos;
		}
		*/
	}

	public void OnClickScrollDown()
	{
		int itemCount = curvedScorlls[m_nTogleIndex].ListItemCount();
		if(itemCount > 4)
			itemCount -= 4;
		
		_scrollBar[m_nTogleIndex].value -= (float)((float)1 / (float)itemCount);
		/*
		if(itemCount > 4)
			itemCount -= 4;

		_scrollUpBtn.interactable = true;
		Vector3 pos = contents[m_nTogleIndex].anchoredPosition3D;
		if( (contents[m_nTogleIndex].anchoredPosition3D.y + m_fMenuSlotSizeY) < (itemCount * m_fMenuSlotSizeY))
		{
			pos.y += m_fMenuSlotSizeY;
			contents[m_nTogleIndex].anchoredPosition3D = pos;
		}
		else
		{
			pos.y = itemCount * m_fMenuSlotSizeY;
			contents[m_nTogleIndex].anchoredPosition3D = pos;
			_scrollDownBtn.interactable = false;
		}
		*/
	}

	public void RefreshScrollBtn(int togleIndex = -1)
	{
		if(togleIndex == -1)
			togleIndex = m_nTogleIndex;
		
		int itemCount = curvedScorlls[togleIndex].ListItemCount();
		if(itemCount > 4)
		{
			_scrollUpBtn.interactable = _scrollBar[togleIndex].value < 0.99f;
			_scrollDownBtn.interactable = _scrollBar[togleIndex].value > 0f;
		}
		else
		{
			_scrollUpBtn.interactable = false;
			_scrollDownBtn.interactable = false;
		}
		/*
		if(togleIndex == -1)
			togleIndex = m_nTogleIndex;
		
		int itemCount = curvedScorlls[togleIndex].ListItemCount();
		if(itemCount > 4)
		{
			itemCount -= 4;
			_scrollUpBtn.interactable = (contents[togleIndex].anchoredPosition3D.y - m_fMenuSlotSizeY) >= 0;
			_scrollDownBtn.interactable = (contents[togleIndex].anchoredPosition3D.y + m_fMenuSlotSizeY) <= (itemCount * m_fMenuSlotSizeY -topBorder);
		}
		else
		{
			_scrollUpBtn.interactable = false;
			_scrollDownBtn.interactable = false;
		}
		*/
	}
}