using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class UI_DivisionMark : MonoBehaviour 
{
	public Image _imgDivisionMark;			// 디비전 마크
	public Image _imgDivisionMarkGloriole;	// 디비전 마크 후광
	public GameObject[] _objDivisionEffect;	// 디비전 파티클 이펙트
	private StringBuilder _tempStringBuilder = new StringBuilder();

	public void SetDivisionMark(byte division)
	{
		if(division <= 0)
			division = 1;
			
		// 마크 셋팅
		_tempStringBuilder.Remove(0, _tempStringBuilder.Length);
		_tempStringBuilder.Append("Sprites/BattleField/league_06.division_").Append(division);
		_imgDivisionMark.sprite = AtlasMgr.Instance.GetSprite(_tempStringBuilder.ToString());
		_imgDivisionMark.SetNativeSize();
        
		// 후광 셋팅
		_tempStringBuilder.Remove(0, _tempStringBuilder.Length);
		_tempStringBuilder.Append("Sprites/BattleField/league_07.Division_Gloriole_").Append(division);
		_imgDivisionMarkGloriole.sprite = AtlasMgr.Instance.GetSprite(_tempStringBuilder.ToString());
		_imgDivisionMarkGloriole.SetNativeSize();

        if(division == 6)
        {
            _imgDivisionMark.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
            _imgDivisionMarkGloriole.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
        }
        else
        {
            _imgDivisionMark.transform.localScale = Vector3.one;
            _imgDivisionMarkGloriole.transform.localScale = Vector3.one;
        }
		for(int i = 0; i < _objDivisionEffect.Length; ++i)
		{
			_objDivisionEffect[i].SetActive((division - 1) == i ? true : false);
		}
	}
}
