using UnityEngine;
using System.Collections;

[System.Serializable]
public class WindowAnimSet
{
	public int level;
	public RectTransform rectLevel;
	public Vector2 posStart;
	public Vector2 posTarget;
	public AnimationCurve animCurve;
	public float animTime;
	public bool showLevel;
}

public class CommonWindowAnim : MonoBehaviour {
	
	public WindowAnimSet[] animSet;

	private bool init = false;
	private CanvasGroup[] canvasGroups;

	public void AnimStart(int index, bool show, System.Action<object> initLevel, object param)
	{
		if(animSet.Length == 0)
			return;

		if(init == false)
		{
			canvasGroups = new CanvasGroup[animSet.Length];
			for(int i=0; i<animSet.Length; i++)
			{
				animSet[i].showLevel = false;
				canvasGroups[i] = animSet[i].rectLevel.GetComponent<CanvasGroup>();
				if(canvasGroups[i] == null)
				{
					canvasGroups[i] = animSet[i].rectLevel.gameObject.AddComponent<CanvasGroup>();
					canvasGroups[i].blocksRaycasts = true;
				}
			}
			init = true;
		}

//		//앞 레벨 창이 열려있지 않으면 리턴
//		if(index > 0)
//		{
//			if(animSet[index].showLevel == false)
//				return;
//		}

		//창 열기
		if(show)
		{
			//레벨 초기화
			if(animSet[index].showLevel == false)
			{
				float delayTime = 0f;
				
				for(int i = animSet.Length - 1; i > 0; i--)
				{
					if(index == i)
						continue;

					if(animSet[i].level == animSet[index].level && animSet[i].showLevel)
					{
						delayTime += animSet[i].animTime;
						LeanTween.move(animSet[i].rectLevel, animSet[i].posStart, animSet[i].animTime);
						animSet[i].showLevel = false;
						canvasGroups[i].blocksRaycasts = false;
					}
				}

				initLevel(param);
				LeanTween.move(animSet[index].rectLevel, animSet[index].posTarget, animSet[index].animTime).setDelay(delayTime).animationCurve = animSet[index].animCurve;
			}
			//딜레이 후 초기화
			else
			{
				float delayTime = 0f;

				for(int i = animSet.Length - 1; i > 0; i--)
				{
					if(index == i)
						continue;

					if(animSet[i].level >= animSet[index].level && animSet[i].showLevel)
					{
						delayTime += animSet[i].animTime;
						LeanTween.move(animSet[i].rectLevel, animSet[i].posStart, animSet[i].animTime);
						animSet[i].showLevel = false;
						canvasGroups[i].blocksRaycasts = false;
					}
				}

				//이미 열려 있으면 닫혔다 열림
				LeanTween.move(animSet[index].rectLevel, animSet[index].posStart, animSet[index].animTime).setDelay(delayTime).setOnComplete((x) => initLevel(param)).animationCurve = animSet[index].animCurve;
				LeanTween.move(animSet[index].rectLevel, animSet[index].posTarget, animSet[index].animTime).setDelay(delayTime + animSet[index].animTime).animationCurve = animSet[index].animCurve;
			}

			animSet[index].showLevel = true;
			canvasGroups[index].blocksRaycasts = true;
		}
		//창 닫기
		else
		{
			float delayTime = 0f;
			
			for(int i = animSet.Length - 1; i > 0; i--)
			{
				if(index == i)
					continue;

				if(animSet[i].level > animSet[index].level && animSet[i].showLevel)
				{
					delayTime += animSet[i].animTime;
					LeanTween.move(animSet[i].rectLevel, animSet[i].posStart, animSet[i].animTime);
					animSet[i].showLevel = false;
					canvasGroups[i].blocksRaycasts = false;
				}
			}

			LeanTween.move(animSet[index].rectLevel, animSet[index].posStart, animSet[index].animTime).setDelay(delayTime).animationCurve = animSet[index].animCurve;

			animSet[index].showLevel = false;
			canvasGroups[index].blocksRaycasts = false;
		}
	}

	public void HideAllLevel()
	{
		for(int i=0; i<animSet.Length; i++)
		{
			animSet[i].rectLevel.anchoredPosition = animSet[i].posStart;
			animSet[i].showLevel = false;
			if(canvasGroups != null && canvasGroups[i] != null)
				canvasGroups[i].blocksRaycasts = false;
		}
	}
}
