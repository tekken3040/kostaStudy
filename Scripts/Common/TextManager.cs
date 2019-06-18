using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class TextManager : Singleton<TextManager> {
	public enum LANGUAGE_TYPE
	{
		NONE=0,
		ENGLISH = 1,
		KOREAN = 2,
        JAPANESE = 3,
	}
	public LANGUAGE_TYPE eLanguage;

	Dictionary<string, TextValue> dicText;
	Dictionary<string, StageValue> dicStage;
	Dictionary<string, StageName> dicStageName;
	Dictionary<string, ErrorValue> dicError;
	Dictionary<string, ErrorTextValue> dicErrorText;

    public List<GameObject> lstTextObject;

	private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

	public void AddText(string[] cols)
	{
		if (cols == null) return;
		TextValue textValue = new TextValue();
		textValue.Set(cols);
		try{
			dicText.Add(textValue.sKeyWord, textValue);
		}catch(System.Exception ex)
		{
			System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
			for(int i=0; i<cols.Length; i++)
			{
				strBuilder.Append(cols[i]);
				strBuilder.Append(", ");
			}
			//DebugMgr.LogError(strBuilder.ToString());
		}
	}

	public void AddStage(string[] cols)
	{
        try
        {
            if(cols == null) return;
            StageValue stageValue = new StageValue();
            stageValue.Set(cols);
            dicStage.Add(stageValue.sKeyWord, stageValue);
        }
        catch(System.Exception e)
        {
            DebugMgr.LogError(e);
        }
		
	}

	public void AddStageName(string[] cols)
	{
		if(cols == null) return;
		StageName stageValue = new StageName();
		stageValue.Set(cols);
		dicStageName.Add(stageValue.sKeyWord, stageValue);
	}

	public void AddError(string[] cols)
	{
		if(cols == null)
			return;
		ErrorValue errorValue = new ErrorValue();
		errorValue.Set(cols);
		dicError.Add(errorValue.sKeyword, errorValue);
	}

	public void AddErrorText(string[] cols)
	{
		if(cols == null)
			return;
		ErrorTextValue errorTextValue = new ErrorTextValue();
		errorTextValue.Set(cols);
		try
		{
			dicErrorText.Add(errorTextValue.sKeyword, errorTextValue);
		}
		catch(System.Exception e)
		{
			System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
			for(int i=0; i<cols.Length; i++)
			{
				strBuilder.Append(cols[i]);
				strBuilder.Append(", ");
			}
		}
	}

	public string GetText(string keyWord)
	{
        if(dicText == null)
            return keyWord;
            
		string ret="";

		TextValue value=null;
		dicText.TryGetValue(keyWord, out value);
		if(value == null)
        {
            DebugMgr.LogError("Key Not Found");
            DebugMgr.LogError(keyWord);
            return keyWord;
        }
		switch(eLanguage)
		{
			case LANGUAGE_TYPE.ENGLISH:
				ret = value.sEnglish;
				ret = ret.Replace("\\n", "\n");
				break;
			case LANGUAGE_TYPE.KOREAN:
				ret = value.sKorean;
				ret = ret.Replace("\\n", "\n");
				break;
            case LANGUAGE_TYPE.JAPANESE:
                ret = value.sJapan;
				ret = ret.Replace("\\n", "\n");
				break;
		}

		return ret;
	}

	public string GetImagePath()
	{
		string fullpath = "TextImage/";

		//fullpath += ObscuredPrefs.GetString ("LANGUAGE_TYPE", "ENGLISH");

		switch(eLanguage)
		{
		case LANGUAGE_TYPE.ENGLISH:
			fullpath += "ENGLISH";
			break;
		case LANGUAGE_TYPE.KOREAN:
			fullpath += "KOREAN";
			break;
		case LANGUAGE_TYPE.JAPANESE:
			fullpath += "ENGLISH";
			break;
		}

		return fullpath;
	}

	public string GetStageNum(string keyWord)
	{
		string ret="";
		
		StageValue value;
		dicStage.TryGetValue(keyWord, out value);
		if(value == null)
			return "Key Not Found";

		ret = GetStageName(value.sStageName);
		
		return ret;
	}

	public string GetStageName(string keyword)
	{
		string ret="";
		
		StageName value;
		dicStageName.TryGetValue(keyword, out value);
		if(value == null)
			return "Key Not Found";
		
		ret = value.sStageName;
		
		return ret;
	}

    // GetError(호출 패킷 타입, 발생한 에러 ID)
    public string GetError(Server.MSGs packetType, Server.ERROR_ID errorID)
	{
		string result = null;
		string errorCode = ((int)packetType * 1000 +(int)errorID).ToString();
		ErrorValue value;
		dicError.TryGetValue(errorCode, out value);
		if(value == null)
			result = GetError(((int)errorID).ToString(), errorCode);
		else
			result = GetErrorText(value.sErrorCode, errorCode);

		return result;
	}

	public string GetError(string errorID, string errorCode = "")
	{
		string ret = "";
		if(errorCode == "")
			errorCode = errorID;

		ErrorValue value;
		dicError.TryGetValue(errorID, out value);
		if(value == null)
		{
			AccountManager.Instance.FBEventLog("Error_"+errorCode);
			return string.Format(GetText("popup_mark_error_server"), errorCode);
		}
		if (value.sErrorCode == "SERVER_ERROR")
			ret = (string)(GetErrorText (value.sErrorCode, errorCode));
		//14일 빌드시 "[" + Server.ServerMgr.Instance.errorString + "]" 부분 제거후 빌드
		else if (value.sErrorCode == "WRONG_SERVER")
			ret = (string)(GetErrorText (value.sErrorCode, errorCode)/* + "[" + Server.ServerMgr.Instance.errorString + "]"*/);
		else
			ret = GetErrorText(value.sErrorCode, errorCode);
        
        return ret;
	}
	 
	public string GetErrorText(string keyword, string errorCode = "", bool isErrorCodeOutput = true)
	{
		string ret = "";
		if(errorCode == "")
			errorCode = keyword;

		ErrorTextValue value;
		dicErrorText.TryGetValue(keyword, out value);
		if(value == null)
        {
			AccountManager.Instance.FBEventLog("Error_"+errorCode);
			ret = string.Format(GetText("popup_mark_error_server"), errorCode);
			return ret;
        }

		switch(eLanguage)
		{
		case LANGUAGE_TYPE.ENGLISH:
			ret = value.sEnglishErrorCode;
			ret = ret.Replace("\\n", "\n");
			break;
		case LANGUAGE_TYPE.KOREAN:
			ret = value.sKoreanErrorCode;
			ret = ret.Replace("\\n", "\n");
			break;
		}

		AccountManager.Instance.FBEventLog("Error_"+errorCode);
		//ret = value.sErrorCode;
		if(isErrorCodeOutput == false)
			return ret;
		else
			return ret + "\n[" + errorCode + "]";
	}

	// 2016. 07. 26 jy 
	// [임시] 메일 타이틀 조합하여 얻기
	// 추후 규칙성 및 별도로 빼내어 보관하여 String 값 관리하기
	public string GetMailTitle(UInt16 nMailTtileCode, string sTitleParam)
	{
		string resultMailTitle = null;
        if (nMailTtileCode == 100)
            resultMailTitle = TextManager.Instance.GetText(sTitleParam);
        else
        {
            resultMailTitle = TextManager.Instance.GetText("post_title_" + nMailTtileCode);
            if (sTitleParam != null)
                resultMailTitle = resultMailTitle.Replace("{0}", sTitleParam);
        }
		return resultMailTitle;
	}

	public void InitCommonText()
	{
		if (loadedInfo)
			return;

        lstTextObject = new List<GameObject>();
		dicText = new Dictionary<string, TextValue>();
		dicError = new Dictionary<string, ErrorValue>();
		dicErrorText = new Dictionary<string, ErrorTextValue>();
		DataMgr.Instance.LoadTable(this.AddText, "Common");
		DataMgr.Instance.LoadTable(this.AddError, "Error");
		DataMgr.Instance.LoadTable(this.AddErrorText, "ErrorText");
        //SetLanguage(Application.systemLanguage.ToString());
        //DebugMgr.LogError(Application.systemLanguage.ToString());
		//1.0.18 OneStore 한글만 되게 수정
		//SetLanguage("KOREAN");
        if(Application.systemLanguage.ToString().ToUpper() == "KOREAN")
            SetLanguage(ObscuredPrefs.GetString("LANGUAGE_TYPE", Application.systemLanguage.ToString()));
        else if(Application.systemLanguage.ToString().ToUpper() == "ENGLISH")
            SetLanguage(ObscuredPrefs.GetString("LANGUAGE_TYPE", "ENGLISH"));
        else if(Application.systemLanguage.ToString().ToUpper() == "JAPANESE")
            SetLanguage(ObscuredPrefs.GetString("LANGUAGE_TYPE", "JAPANESE"));
        else
            SetLanguage(ObscuredPrefs.GetString("LANGUAGE_TYPE", "ENGLISH"));
	}

	public void Init()
	{
//		dicText = new Dictionary<string, TextValue>();
		if (loadedInfo)
			return;
		
		dicStage = new Dictionary<string, StageValue>();
		dicStageName = new Dictionary<string, StageName>();

//        DataMgr.Instance.LoadTable(this.AddText, "Common");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/Achievement");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/Campaign");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/Class");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Condition");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Equipment");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/EquipModel");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Item");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Material");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Monster");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Quest");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/QuestTalk");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Oxquiz");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/Shop");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Skill");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Stage");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/TalkCharacter");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/Tip");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/TutorialTalk");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/TutorialText");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_Battle");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_BattleField");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_CampaignString");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_CharString");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_CrewString");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_Event");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_GuildString");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_InvenString");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_ShopString");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_SmithingString");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_Social");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_VipString");
		DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_League");
		DataMgr.Instance.LoadTable(this.AddStage, "Stage");
		DataMgr.Instance.LoadTable(this.AddStageName, "StageName");
        DataMgr.Instance.LoadTable(this.AddText, "GameText/UI_GuildText");

		loadedInfo = true;
		//SetLanguage(Application.systemLanguage.ToString());
	}

	public void SetLanguage(string language)
	{
		switch(language.ToUpper())
		{
		    case "ENGLISH":
                eLanguage = LANGUAGE_TYPE.ENGLISH;
                break;
		    case "KOREAN":
                eLanguage = LANGUAGE_TYPE.KOREAN;
                break;
            case "JAPANESE":
                eLanguage = LANGUAGE_TYPE.JAPANESE;
                break;
		}
        ObscuredPrefs.SetString("LANGUAGE_TYPE", language);
	}
}

public class TextValue
{
	public string sKeyWord;
	public string sKorean;
	public string sEnglish;
    public string sJapan;
	public string Set(string[] cols)
	{
		int idx=0;
		sKeyWord = cols[idx++];
        idx++;
        sKorean = cols[idx++];
		sEnglish = cols[idx++];
        sJapan = cols[idx++];
		return sKeyWord;
	}
}

public class StageValue
{
	public string sKeyWord;
	public string sStageName;
	public string Set(string[] cols)
	{
		int idx=0;
		sKeyWord = cols[idx++];
		sStageName = cols[++idx];
		return sKeyWord;
	}
}

public class StageName
{
	public string sKeyWord;
	public string sStageName;
	public string Set(string[] cols)
	{
		int idx=0;
		sKeyWord = cols[idx++];
		sStageName = cols[++idx];
		return sKeyWord;
	}
}

public class ErrorValue
{
	public string sKeyword;
	public string sErrorCode;
	public string Set(string[] cols)
	{
		int idx = 0;
		sKeyword = cols[idx++];
		sErrorCode = cols[++idx];

		return sKeyword;
	}
}

public class ErrorTextValue
{
	public string sKeyword;
	//public string sErrorCode;
	public string sKoreanErrorCode;
	public string sEnglishErrorCode;
    public string sJapanErrorCode;
	public string Set(string[] cols)
	{
		int idx = 0;
		sKeyword = cols[idx++];
		//sErrorCode = cols[++idx];
		sKoreanErrorCode = cols[++idx];
		sEnglishErrorCode = cols[++idx];
        sJapanErrorCode = cols[++idx];
		return sKeyword;
	}
}