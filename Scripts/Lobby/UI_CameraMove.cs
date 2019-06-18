using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class UI_CameraMove : MonoBehaviour
{
    public AnimationClip[] _camAnimation;
	private Animator m_Animater;
	private int m_nCameraViewCount = 0;
	private int _aniNum = 0;
	public int CameraViewIndex
	{
		get { return _aniNum; }
	}
	private Transform m_trCamera;

	void Awake()
	{
		m_nCameraViewCount = _camAnimation.Length;
		m_Animater = this.gameObject.GetComponent<Animator>();
		m_trCamera = this.gameObject.GetComponent<Transform>();

		m_Animater.enabled = false;
		SetLobbyCamera();
	}

    public void CamAnimation()
    {
        //_aniNum = Random.Range(0, 3);
		++_aniNum;
		if(_aniNum >= m_nCameraViewCount)
			_aniNum = 0;

		m_Animater.enabled = true;
		m_Animater.applyRootMotion = false;
		m_Animater.Play(_camAnimation[_aniNum].name);
    }
	/*
    public void CamAnimationBack()
    {
		m_Animater.Play(_camAnimationBack[_aniNum].name);
    }
	*/
	public void SetLobbyCamera()
	{
		if( m_trCamera == null )
		{
			DebugMgr.LogError("m_trCamera Null");
			return;
		}
		
		_aniNum = ObscuredPrefs.GetInt("LobbyCameraIndex");
		if(_aniNum != 0 )
		{
			m_trCamera.position = ObscuredPrefs.GetVector3("LobbyCameraPos");
			m_trCamera.rotation = ObscuredPrefs.GetQuaternion("LobbyCameraRot");
		}
	}

	public void SaveCameraInfo()
	{
		if( m_trCamera == null )
		{
			DebugMgr.LogError("m_trCamera Null");
			return;
		}
		m_Animater.enabled = false;
		ObscuredPrefs.SetInt("LobbyCameraIndex", _aniNum);
		ObscuredPrefs.SetVector3("LobbyCameraPos", m_trCamera.position);
		ObscuredPrefs.SetQuaternion("LobbyCameraRot", m_trCamera.rotation);
	}
}
