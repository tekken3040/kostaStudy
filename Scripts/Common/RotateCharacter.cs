using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class RotateCharacter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{

	public Transform characterTransform;
	protected Vector2 startPos;

	public Camera m_3DCamera;
	protected float m_fClickMoveSensitivity;
	protected bool m_bModelRotation;
	public bool ModelRotation {	get { return m_bModelRotation; } }

	private bool m_bModelClick;
	public bool ModelClick { get { return m_bModelClick; } }

	// 2016. 07. 25 jy;
	// 로테이션 캐릭터 스크립는 UI 부분의 붙어 있으나 카메라로 스크립트 위치 변경
	void Awake()
	{
		m_fClickMoveSensitivity = 3;

		if(m_3DCamera == null)
			m_3DCamera = this.GetComponent<Camera>();
	}

	// 2016. 07. 25 jy
	// 모바일 빌드하여 테스트를 진행해야 함
	// 모바일 상에서도 터치하여 캐릭터가 회전 여부 확인
	void Update()
	{
		if(m_3DCamera != null)
		{
			OnClickDown();
		}
	}

	protected IEnumerator CharacterRotation()
	{
		Vector3 currentPos;
		while(m_bModelClick)
		{
			#if UNITY_EDITOR
			currentPos = Input.mousePosition;
			#else
			currentPos = Input.GetTouch(0).position;
			#endif

			// 움직임이 민감도를 넣어 움직임을 조절
			if((startPos.x - currentPos.x) > m_fClickMoveSensitivity)
			{
				m_bModelRotation = true;
				float spd = startPos.x - currentPos.x;
				characterTransform.localEulerAngles = new Vector3(0f, characterTransform.localEulerAngles.y + spd, 0f);
				startPos = currentPos;
			}
			else if((startPos.x - currentPos.x) < -m_fClickMoveSensitivity)
			{
				m_bModelRotation = true;
				float spd = startPos.x - currentPos.x;
				characterTransform.localEulerAngles = new Vector3(0f, characterTransform.localEulerAngles.y + spd, 0f);
				startPos = currentPos;
			}

			yield return null;
		}
	}

#if UNITY_EDITOR
	protected void OnClickDown()
	{
		if(Input.GetMouseButtonDown(0))
		{
			startPos = Input.mousePosition;
			Ray ray = m_3DCamera.ScreenPointToRay(startPos);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 5f))
			{
				if(hit.transform.CompareTag("Player"))
				{
					m_bModelClick = true;
					m_bModelRotation = false;
					characterTransform = hit.transform;
					StartCoroutine("CharacterRotation");
				}
			}
		}
		else if(Input.GetMouseButtonUp(0) && m_bModelClick == true)
		{
			m_bModelClick = false;
			characterTransform = null;
		}
	}

#else
	private void OnClickDown()
	{
		if(Input.touchCount > 0)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				startPos = Input.GetTouch(0).position;
				Ray ray = m_3DCamera.ScreenPointToRay(startPos);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 5f))
				{
					if(hit.transform.CompareTag("Player"))
					{
					m_bModelClick = true;
					m_bModelRotation = false;
					characterTransform = hit.transform;
					StartCoroutine("CharacterRotation");
					}
				}
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended && m_bModelClick == true)
			{
				m_bModelClick = false;
				characterTransform = null;
			}
		}
	}
#endif

	// 사용하지 않지만 언젠가 쓸수 있으므로 방치
	public void OnBeginDrag(PointerEventData eventData)
	{
		startPos = eventData.position;
       	if(this.GetComponent<CanvasGroup>() != null)
           	this.GetComponent<CanvasGroup>().interactable = false;
	}

	public void OnDrag(PointerEventData data)
	{
		if( characterTransform != null )
		{
			if(startPos.x - data.position.x > 0)
			{
				float spd = startPos.x - data.position.x;
				characterTransform.localEulerAngles = new Vector3(0f, characterTransform.localEulerAngles.y + spd, 0f);
				startPos = data.position;
			}
			else
			{
				float spd = startPos.x - data.position.x;
				characterTransform.localEulerAngles = new Vector3(0f, characterTransform.localEulerAngles.y + spd, 0f);
				startPos = data.position;
			}
		}
		else
		{
			DebugMgr.LogError("characterTransform == null");
		}
	}

    public void OnEndDrag(PointerEventData data)
    {
       	if(this.GetComponent<CanvasGroup>() != null)
           	this.GetComponent<CanvasGroup>().interactable = true;
    }
}
