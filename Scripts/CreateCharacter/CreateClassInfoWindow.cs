using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CreateClassInfoWindow : MonoBehaviour 
{
	public Image m_imgClassIcon;
	public Image m_imgClassGraph;

	public Text m_textDescription;
	public Text m_textFeature;
	public Text m_textAttribute;

	public void SetClassInfo(ClassInfo classInfo)
	{
		m_imgClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Inventory/class_01.class_on_" + classInfo.u2ID.ToString());
		m_imgClassIcon.SetNativeSize();
		m_imgClassGraph.sprite = AtlasMgr.Instance.GetSprite("Sprites/CreateCharacter/Character_Creat_03.Character_Graph_" + classInfo.u2ID.ToString());
		m_imgClassGraph.SetNativeSize();

		m_textDescription.text = classInfo.sDescription;
		m_textFeature.text = classInfo.Feature;
		m_textAttribute.text = classInfo.AttackAttribute;
	}
}