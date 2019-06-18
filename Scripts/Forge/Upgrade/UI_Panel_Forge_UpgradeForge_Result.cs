using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Panel_Forge_UpgradeForge_Result : MonoBehaviour {
	[SerializeField] Text _txtForgeLevel;

	ForgeInfo _cForgeInfo;
	public void SetData(ForgeInfo forgeInfo)
	{
		_cForgeInfo = forgeInfo;
		_txtForgeLevel.text = string.Format("[{0}] "+TextManager.Instance.GetText("mark_upgrade_tier"), TextManager.Instance.GetText(forgeInfo.sName));
	}

}
