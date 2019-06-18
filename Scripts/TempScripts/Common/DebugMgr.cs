using UnityEngine;
using System.Collections;

static public class DebugMgr {

	static public void Log(object debugText)
	{
		#if dev || qa
		Debug.Log(debugText);
		#endif
	}

	static public void Log(object debugText, Object debugObj)
	{
		#if dev || qa
		Debug.Log(debugText, debugObj);
		#endif
	}

	static public void LogWarning(object debugText)
	{
		#if dev || qa
		Debug.LogWarning(debugText);
		#endif
	}

	static public void LogWarning(object debugText, Object debugObj)
	{
		#if dev || qa
		Debug.LogWarning(debugText, debugObj);
		#endif
	}

	static public void LogError(object debugText)
	{
		#if dev || qa
		Debug.LogError(debugText);
		#endif
	}

	static public void LogError(object debugText, Object debugObj)
	{
		#if dev || qa
		Debug.LogError(debugText, debugObj);
		#endif
	}
}
