using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_PrevLeagueResult : MonoBehaviour 
{
	public GameObject objEffectClose;			// 연출 닫음 오브젝트
	public RectTransform rtDivisionList;		// 디비전 리스트
	public CanvasGroup[] arrDivisionIconList;	// 디비전 아이콘 리스트

	public Text txtPrevDivision;	// 이전 디비전
	public Text txtPrevRank;		// 이전 랭크 

	private float m_fTargetPosX;
	private int m_nPrveDivision;	 // 이전 리그 인덱스 

	void Awake()
	{
		// 배열 인덱스는 0부터 시작해서 -1 함
		m_nPrveDivision = UI_League.Instance.cLeagueMatchList.GetPrveDivisionIndex - 1;

		txtPrevDivision.text = TextManager.Instance.GetText("mark_division_" + UI_League.Instance.cLeagueMatchList.GetPrveDivisionIndex);
		txtPrevRank.text = UI_League.Instance.cLeagueMatchList.u4PrevMyRank + " " + TextManager.Instance.GetText("mark_league_rank");

		m_fTargetPosX = arrDivisionIconList[m_nPrveDivision].GetComponent<RectTransform>().anchoredPosition3D.x;
	}

	void OnEnable()
	{
		StartCoroutine("PrevResultEffect");
	}

	public void OnClickClose()
	{
		objEffectClose.GetComponent<Button>().interactable = false;
		Server.ServerMgr.Instance.RequestLeagueDivisionCheck(UI_League.Instance.cLeagueMatchList.u1DivisionIndex, RequestCheck);
	}

	private IEnumerator PrevResultEffect()
	{
		float time = 0f;
		Vector3 targetPos = new Vector3(m_fTargetPosX * -1, rtDivisionList.anchoredPosition3D.y, 0f);

		yield return new WaitForSeconds(1f);
		// 해당 티어로 이동 연출
		while(true)
		{
			float f = Vector3.Distance(targetPos, rtDivisionList.anchoredPosition3D);
			if(f >= 0.1f)
			{
				rtDivisionList.anchoredPosition3D = Vector3.Lerp(rtDivisionList.anchoredPosition3D, targetPos, time * 0.05f);
				time += Time.deltaTime;

				yield return null;
			}
			else
			{
				break;
			}
		}

		yield return new WaitForSeconds(0.5f);
		// 이전 티어의 아이콘을 제외하고 투명화 처리 한다
		float alpha = 1f;
		while(true)
		{
			for(int i = 0; i < arrDivisionIconList.Length; ++i)
			{
				if(i == m_nPrveDivision)
					continue;

				arrDivisionIconList[i].alpha = alpha;
			}

			if(alpha <= 0)
				break;

			alpha -= 0.1f;
			yield return new WaitForSeconds(0.025f);
		}

		// 텍스트 연출 시작
		txtPrevDivision.gameObject.SetActive(true);
		txtPrevRank.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);
		objEffectClose.SetActive(true);
	}

	public void RequestCheck(Server.ERROR_ID err)
	{
		this.gameObject.SetActive(false);

		// 새로운 리그가 시작되었다고 알림 팝업 띄움
		PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("popup_desc_new_season"), null);

		Destroy(this.gameObject);
	}
}
