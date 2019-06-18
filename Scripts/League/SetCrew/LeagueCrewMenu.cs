using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Text;

namespace UnityEngine.EventSystems
{
	public interface ChangeLeagueCharEvent : IEventSystemHandler 
	{
		void ChangedSlotToSlot(Hero prevHero, Hero inHero, Byte prevHeroSlotNum, Byte inHeroSlotNum);

		void ChangedListToSlot(Hero inHero, Byte slotNumInCrew);
		
		void ChangedSlotToList(Hero outHero);		
	}
}

public class LeagueCrewMenu : MonoBehaviour, ChangeLeagueCharEvent
{
    [SerializeField] Text[] txtCharPower;           //배치된 캐릭터 각각의 전투력
    [SerializeField] GameObject _objSlotList;       //캐릭터 슬롯 리스트
    [SerializeField] GameObject _objCharList;       //캐릭터 리스트
    [SerializeField] Text txtLeagueCrewPower;       //리그 크루 전체 전투력

    public RectTransform _tfListGroup;

    [SerializeField] UI_League_Slot[] _charSlot;    //캐릭터 슬롯

    GameObject _prefCharElement;                    //캐릭터 프리펩
    GameObject _prefSlotElement;                    //캐릭터 슬롯 프리펩
    GameObject _prefCursorElement;                  //드래그용 캐릭터 프리펩
    StringBuilder tempStringBuilder;                //스트링 빌더
    
    void Awake()
    {
        _prefCharElement = AssetMgr.Instance.AssetLoad("Prefabs/UI/League/Character_.prefab", typeof(GameObject)) as GameObject;
        _prefSlotElement = AssetMgr.Instance.AssetLoad("Prefabs/UI/League/CharacterSlot_.prefab", typeof(GameObject)) as GameObject;
        tempStringBuilder = new StringBuilder();
    }

    public void OnEnable()
    {
        Init();
    }
	
    public void Init()
    {
        for(int i=0; i<Legion.Instance.cLeagueCrew.acLocation.Length; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
                //_charSlot[i].SetData((Byte)(((Hero)Legion.Instance.cLeagueCrew.acLocation[i]).u1Index-1));
                _charSlot[i].SetData((Byte)i);
            else
                _charSlot[i].SetData(0, true);
        }
        Legion.Instance.cLeagueCrew.StartChanging();
        InitCharacterList();
    }

    //캐릭터 리스트 삭제
    public void DeleteCharacterList()
    {
        for(int i=0; i<_objCharList.transform.childCount; i++)
        {
            GameObject.Destroy(_objCharList.transform.GetChild(i).gameObject);
        }
    }
    //캐릭터 리스트 초기화
    public void InitCharacterList()
    {
        if(_objCharList.transform.childCount == 0)
        {
            for(int i=0; i<Legion.Instance.acHeros.Count; i++)
            {
                GameObject charListElement = Instantiate(_prefCharElement);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append("CharElement_").Append(i.ToString());
                RectTransform rtTr = charListElement.GetComponent<RectTransform>();
                rtTr.SetParent(_objCharList.GetComponent<RectTransform>());
                rtTr.name = tempStringBuilder.ToString();
                rtTr.localScale = Vector3.one;
                rtTr.localPosition = Vector3.zero;
                charListElement.GetComponent<UI_League_CharElement>().SetData(Legion.Instance.acHeros[i]);
            }
        }
		else
		{
			// 캐릭터 슬롯이 생성되어 있다면 캐릭터 정보를 새로고침 한다
			for(int i=0; i<_objCharList.transform.childCount; i++)
			{
				if(_objCharList.transform.GetChild(i).gameObject.activeSelf == false)
					continue;
			
				_objCharList.transform.GetChild(i).GetComponent<UI_League_CharElement>().RefreshHeroInfo();
			}
		}

        if (_objSlotList.transform.childCount == 0)
        {
            for(int i=0; i<LegionInfoMgr.Instance.limitCharSlot; i++)
            {
                GameObject CharSlot = Instantiate(_prefSlotElement);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append("CharSlot_").Append(i.ToString());
				RectTransform rectTr = CharSlot.GetComponent<RectTransform>();
				rectTr.SetParent(_objSlotList.GetComponent<RectTransform>());
				rectTr.name = tempStringBuilder.ToString();
				rectTr.localScale = Vector3.one;
				rectTr.localPosition = Vector3.zero;
            }
        }
        RefreashCharListAndSlots();
    }

    #region ChangeLeagueCharEvent implementation
	//캐릭터 리스트에서 슬롯으로 배치
	public void ChangedListToSlot (Hero inHero, Byte slotNumInCrew)
	{
        Character outChar;

        Legion.Instance.cLeagueCrew.Assign(inHero, slotNumInCrew, out outChar);
        RefreashCharListAndSlots();
	}
	//슬롯에서 리스트로 배치
	public void ChangedSlotToList (Hero outHero)
	{
        Legion.Instance.cLeagueCrew.Resign(outHero);
        RefreashCharListAndSlots();
	}
	//슬롯에서 슬롯으로 배치
	public void ChangedSlotToSlot (Hero prevHero, Hero inHero, Byte prevHeroSlotNum, Byte inHeroSlotNum)
	{
        Character outChar;

        if(prevHero == null)
        {
            Legion.Instance.cLeagueCrew.Resign(inHero);
            Legion.Instance.cLeagueCrew.Assign(inHero, prevHeroSlotNum, out outChar);
        }
        else
        {
            Legion.Instance.cLeagueCrew.Resign(inHero);
            Legion.Instance.cLeagueCrew.Assign(inHero, prevHeroSlotNum, out outChar);
            Legion.Instance.cLeagueCrew.Assign(outChar, inHeroSlotNum, out outChar);
        }
        RefreashCharListAndSlots();
	}
	#endregion

    public void RefreashCharListAndSlots()
    {
        for(int i=0; i<_charSlot.Length; i++)
        {
            _charSlot[i].SetLeader(false);
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
            {
                //_charSlot[i].SetData((Byte)(((Hero)Legion.Instance.cLeagueCrew.acLocation[i]).u1Index-1));
                _charSlot[i].SetData((Byte)i);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append(((Hero)Legion.Instance.cLeagueCrew.acLocation[i]).u4Power);
                txtCharPower[i].text = tempStringBuilder.ToString();
            }
            else
            {
                _charSlot[i].SetData(0, true);
                txtCharPower[i].text = "";
            }
        }

        for(int i=0; i<LeagueCrew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
            {
                _charSlot[i].SetLeader(true);
                break;
            }
            else
                _charSlot[i].SetLeader(false);
        }
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_total_power")).Append(" ").Append(Legion.Instance.cLeagueCrew.u4Power);
        txtLeagueCrewPower.text = tempStringBuilder.ToString();
        // 캐릭터 슬롯이 생성되어 있다면 캐릭터 정보를 새로고침 한다
		for(int i=0; i<_objCharList.transform.childCount; i++)
		{
			_objCharList.transform.GetChild(i).GetComponent<UI_League_CharElement>().RefreshHeroInfo();
		}
        StartCoroutine("SlotSizeSetting");
    }

    // 2017. 07. 13 jy 캐릭터 슬롯의 스크롤 가능 사이즈를 셋팅한다
    private IEnumerator SlotSizeSetting()
    {
        // 바로 사이즈 셋팅시 정상적으로 사이즈 조정이 불가능하여 한 프레임 후에 셋팅하도록 코루틴 사용
        yield return null;

        _objCharList.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        Vector2 size = _objCharList.GetComponent<RectTransform>().sizeDelta;
        _tfListGroup.sizeDelta = new Vector2(0, size.y + 65);
    }
}
