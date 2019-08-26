using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "1.0";
    [SerializeField] private string nickName = "Admin";
    [SerializeField] private GameObject _player;
    [SerializeField] PlayerSetting playerSetting = PlayerSetting.PUNObject;
    [SerializeField] GameObject _gun;
    [SerializeField] GameObject _light;

    public enum PlayerSetting
    {
        PUNObject,
        NFD_Kosuzu,
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
        PhotonNetwork.CreateRoom(null, new RoomOptions{MaxPlayers = 2 });
    }

    // 캐릭터 생성
    private void CharacterInit()
    {
        //_gun.SetActive(true);
        //_light.SetActive(true);
        GameObject gun = PhotonNetwork.Instantiate("Handgun_M1911A_Black", new Vector3(21f, 1f, 1.5f), Quaternion.identity);
        GameObject flashLight = PhotonNetwork.Instantiate("Flashlight", new Vector3(21f, 1f, 2f), Quaternion.identity);

        if(playerSetting == PlayerSetting.PUNObject)
        {
            // 바디
            GameObject player = Instantiate(_player);
            player.name = "Player";
            player.transform.position = new Vector3(21.87f, 0.875f, 1.558f);

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
            // 바디
            GameObject player = Instantiate(_player);
            player.name = "Player";
            //player.transform.position = new Vector3(0, 0.689f, 0);
            player.transform.position = Vector3.zero;

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
    }
}
