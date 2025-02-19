using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UIManager : Singleton<UIManager> /*: MonoBehaviour */ {
	public void SetGradientFromElement(Gradient gradientComponent, Byte element)
	{
		Color32 equipElementColors_Start = Color.white;
		Color32 equipElementColors_End = new Color32(160, 160, 160, 255);
		switch(element)
		{
			case 2: 
				equipElementColors_Start = new Color32(217, 104, 104, 255);
				equipElementColors_End = new Color32(135, 43, 39, 255);
				break;
			case 3:
				equipElementColors_Start = new Color32(106, 153, 240, 255);
				equipElementColors_End = new Color32(70, 81, 151, 255);
				break;
			case 4: equipElementColors_Start = new Color32(147, 190, 127, 255); 
				equipElementColors_End = new Color32(106, 139, 83, 255);
				break;
			default:
				equipElementColors_Start = new Color32(255, 255, 255, 255);
				equipElementColors_End = new Color32(160, 160, 160, 255);
				break;
		}

		gradientComponent.StartColor = equipElementColors_Start;
		gradientComponent.EndColor = equipElementColors_End;
		gradientComponent.ReDraw();
	}

	public Color GetColorFromEle(Byte element)
	{
		//Color32 equipElementColors_Start = Color.white;
		Color32 equipElementColors_End = new Color32(160, 160, 160, 255);
		switch(element)
		{
		case 2: 
			equipElementColors_End = new Color32(255, 0, 0, 255);
			break;
		case 3:
			equipElementColors_End = new Color32(0, 128, 255, 255);
			break;
		case 4:
			equipElementColors_End = new Color32(0, 255, 160, 255);
			break;
		default:
			equipElementColors_End = new Color32(160, 160, 160, 255);
			break;
		}
			
		return equipElementColors_End;
	}

	public void SetGradientFromTier(Gradient gradientComponent, Byte tier)
	{
		Color32 equipElementColors_Start = Color.white;
		Color32 equipElementColors_End = new Color32(160, 160, 160, 255);
		switch(tier)
		{
		case 1:
			equipElementColors_Start = new Color32(169, 242, 255, 255);
				equipElementColors_End = new Color32(16, 175, 255, 255);
			break;
		case 2: 
			equipElementColors_Start = new Color32(238, 255, 189, 255);
				equipElementColors_End = new Color32(188, 236, 88, 255);
			break;
		case 3:
			equipElementColors_Start = new Color32(222, 175, 255, 255);
				equipElementColors_End = new Color32(149, 64, 245, 255);
			break;
		case 4: equipElementColors_Start = new Color32(255, 192, 112, 255);
			equipElementColors_End = new Color32(255, 65, 0, 255); 
			break;
		case 5: equipElementColors_Start = new Color32(39, 161, 255, 255);
			equipElementColors_End = new Color32(53, 89, 255, 255); 
			break;
		case 6: equipElementColors_Start = new Color32(253, 113, 207, 255);
			equipElementColors_End = new Color32(255, 61, 147, 255); 
			break;
		case 7: equipElementColors_Start = new Color32(242, 187, 132, 255);
			equipElementColors_End = new Color32(142, 56, 0, 255); 
			break;
		case 8: equipElementColors_Start = new Color32(146, 222, 35, 255);
			equipElementColors_End = new Color32(20, 137, 0, 255); 
			break;
		case 9: equipElementColors_Start = new Color32(255, 149, 149, 255);
			equipElementColors_End = new Color32(255, 5, 10, 255); 
			break;
		case 10: equipElementColors_Start =new Color32(254, 236, 25, 255);
			equipElementColors_End =  new Color32(255, 162, 0, 255); 
			break;

		default:
			equipElementColors_Start = new Color32(255, 255, 255, 255);
			equipElementColors_End = new Color32(160, 160, 160, 255);
			break;
		}

		gradientComponent.StartColor = equipElementColors_Start;
		gradientComponent.EndColor = equipElementColors_End;
		gradientComponent.ReDraw();
	}

	public Color GetColorFromTier(Byte tier)
	{
		Color32 equipElementColors_End = Color.white;
		switch(tier)
		{
		case 1:
			equipElementColors_End = new Color32(0, 227, 255, 255);
			break;
		case 2: 
			equipElementColors_End = new Color32(188, 236, 88, 255);
			break;
		case 3:
			equipElementColors_End = new Color32(149, 64, 245, 255);
			break;
		case 4:
			equipElementColors_End = new Color32(255, 90, 0, 255); 
			break;
		case 5:
			equipElementColors_End = new Color32(53, 89, 255, 255); 
			break;
		case 6:
			equipElementColors_End = new Color32(255, 61, 147, 255); 
			break;
		case 7:
			equipElementColors_End = new Color32(122, 43, 0, 255); 
			break;
		case 8:
			equipElementColors_End = new Color32(20, 137, 0, 255); 
			break;
		case 9:
			equipElementColors_End = new Color32(255, 5, 10, 255); 
			break;
		case 10:
			equipElementColors_End =  new Color32(255, 230, 0, 255); 
			break;

		default:
			equipElementColors_End = new Color32(160, 160, 160, 255);
			break;
		}

		return equipElementColors_End;
	}

	public void SetGradientButton(Gradient gradientComponent, bool active)
	{
		if(active)
		{

		}
		else
		{

		}
	}

	public void SetSizeTextGroup(RectTransform textGroup, int sizePerLetter)
	{
		float totalWidth = 0;
		float posX = 0f;
		float posY = 0f;
		float sizeWidth = 0f;
		float sizeHeight = 0f;
		for(int i=0; i<textGroup.childCount; i++)
		{
			posX = totalWidth;
			posY = textGroup.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y;

			if(textGroup.GetChild(i).GetComponent<Text>() != null)
			{
				//				sizePerLetter = textGroup.GetChild(i).GetComponent<Text>().fontSize / 10 * 10 - 10;
				if(textGroup.GetChild(i).GetComponent<LoadText>() != null)
					textGroup.GetChild(i).GetComponent<LoadText>().TextLoad();
				string text = textGroup.GetChild(i).GetComponent<Text>().text;
				int letterCount = Encoding.Default.GetByteCount (text);
				//				DebugMgr.Log("Text : " + text + "  Letter : " + letterCount);
				sizeWidth = letterCount * sizePerLetter;

				totalWidth += sizeWidth * textGroup.GetChild(i).GetComponent<RectTransform>().localScale.x;
			}
			else
			{
				sizeWidth = textGroup.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;

				totalWidth += sizeWidth * textGroup.GetChild(i).GetComponent<RectTransform>().localScale.x;
			}
			sizeHeight = textGroup.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;

			textGroup.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
			textGroup.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(sizeWidth, sizeHeight);
		}

		textGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, textGroup.GetComponent<RectTransform>().sizeDelta.y);
	}

	public void SetSizeTextGroup(RectTransform textGroup)
	{
		SetSizeTextGroup(textGroup, 20);
	}
}
