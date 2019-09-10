using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "1.0";                        // 게임버전
    [SerializeField] private string nickName = "Admin";                         // 닉네임 로그인 구현 후 수정
    [SerializeField] private GameObject _player;                                // 플레이어 프리펩 오브젝트
    [SerializeField] PlayerSetting playerSetting = PlayerSetting.NFD_Kosuzu;    // 생성할 아바타 타입
    [SerializeField] GameObject _gun;                                           // 총 프리펩 오브젝트
    [SerializeField] GameObject _light;                                         // 손전등 프리펩 오브젝트
    [SerializeField] GameObject _eventSystem;                                   // UI컨트롤에 사용될 이벤트 시스템
    [SerializeField] GameObject _visualizer;                                    // UI에 사용될 포인터 오브젝트
    [SerializeField] GameObject _youTubeObj;                                    // 유튜브 플레이어 게임 오브젝트
    [SerializeField] Canvas _youtubeCanvas;                                     // 유튜브 플레이어 컨트롤 패널
    [SerializeField] Canvas _youtubeKeyboardCanvas;                             // 유튜브 플레이어 키보드 컨트롤 패널

    public enum PlayerSetting
    {
        PUNObject,      // 일반 오브젝트
        NFD_Kosuzu,     // 아바타 오브젝트
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        OnLogin();  // 로그인
    }

    private void OnLogin()
    {
        PhotonNetwork.GameVersion = this.gameVersion;   // 버전
        PhotonNetwork.NickName = this.nickName;         // 닉네임
        PhotonNetwork.ConnectUsingSettings();           // PUN2
    }

    // 포톤 연결 성공 콜백 메서드
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // 룸연결 실패 콜백 매서드
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방생성");
        this.CreateRoom();
    }

    // 방에 접속시 캐릭터 생성
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        Debug.Log("캐릭터 생성");
        Invoke("CharacterInit", 1.0f);
    }

    private void CreateRoom()
    {
        // CreateRoom(방이름, 방옵션)
        PhotonNetwork.CreateRoom(null, new RoomOptions{MaxPlayers = 4 });
    }

    // 캐릭터 생성
    private void CharacterInit()
    {
        // 바디
        GameObject player = Instantiate(_player);
        player.name = "Player";
        //player.transform.position = new Vector3(0, 0.689f, 0);
        player.transform.position = Vector3.zero;

        //GameObject player = GameObject.Find("Player");
        if(playerSetting == PlayerSetting.PUNObject)
        {
            // 네트워크 오브젝트 생성
            GameObject punObject = PhotonNetwork.Instantiate("PUNObject", new Vector3(21.87f, 0.875f, 1.558f), Quaternion.identity);
            punObject.transform.parent = GameObject.Find("Player").transform;

            // 위치 머리
            punObject.transform.Find("Head T").transform.parent = player.transform.Find("CenterEyeAnchor").transform;

            // 위치 왼손
            punObject.transform.Find("LeftHand T").transform.parent = player.transform.Find("LeftControllerAnchor").transform;

            // 위치 오른손
            punObject.transform.Find("RightHand T").transform.parent = player.transform.Find("RightControllerAnchor").transform;

            player.transform.position = new Vector3(21.87f, 0.875f, 1.558f);
        }
        else if(playerSetting == PlayerSetting.NFD_Kosuzu)
        {
            // 네트워크 오브젝트 생성
            //GameObject punObject = PhotonNetwork.Instantiate("Miraikomachi", new Vector3(21.87f, 0.875f, 1.558f), Quaternion.identity);
            GameObject punObject = PhotonNetwork.Instantiate("NFD_Kosuzu", Vector3.zero, Quaternion.identity);
            punObject.transform.parent = GameObject.Find("Player").transform;
            PlayerAvatar avatar = punObject.GetComponent<PlayerAvatar>();

            // 위치 머리
            punObject.transform.Find("Head T").transform.parent = player.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;
            //player.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform.localPosition = new Vector3(0, 0.4f, 0);

            // 위치 왼손
            punObject.transform.Find("Left wrist T").transform.parent = player.transform.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform;
            avatar.getLeftHandTarget.localPosition = new Vector3(-0.053f, -0.038f, -0.083f);

            // 위치 오른손
            punObject.transform.Find("Right wrist T").transform.parent = player.transform.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform;
            avatar.getRightHandTarget.localPosition = new Vector3(0.053f, -0.038f, -0.083f);

            player.transform.position = new Vector3(21.87f, 0, 1.558f);
            //punObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        // 손 커서 인디케이터 활성화
        _visualizer.SetActive(true);
        // 이벤트 시스템에 카메라 할당
        _eventSystem.GetComponent<OVRInputModule>().rayTransform = player.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;
        // 유튜브 플레이어 활성화
        _youTubeObj.SetActive(true);
        // 유튜브 플레이어 캔버스에 카메라 할당
        _youtubeCanvas.worldCamera = player.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<Camera>();
        _youtubeKeyboardCanvas.worldCamera = player.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponent<Camera>();
    }
}
