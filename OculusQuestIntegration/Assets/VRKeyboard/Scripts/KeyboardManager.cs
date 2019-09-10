/***
 * Author: Yunhan Li
 * Any issue please contact yunhn.lee@gmail.com
 ***/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace VRKeyboard.Utils
{
    public class KeyboardManager : MonoBehaviour, IPunObservable
    {
        #region Public Variables
        [Header("User defined")]
        [Tooltip("If the character is uppercase at the initialization")]
        public bool isUppercase = false;
        public int maxInputLength;

        [Header("UI Elements")]
        public Text inputText;

        [Header("Essentials")]
        public Transform keys;

        [Header("YouTube Player")]
        public YoutubePlayer youtubePlayer;
        public InputField urlField;
        #endregion

        #region Private Variables
        private string Input
        {
            get { return inputText.text; }
            set { inputText.text = value; }
        }
        private Key[] keyList;
        private bool capslockFlag;
        private string URLField
        {
            get { return urlField.text; }
            set { urlField.text = value; }
        }
        #endregion

        #region Monobehaviour Callbacks
        private void Awake()
        {
            keyList = keys.GetComponentsInChildren<Key>();
        }

        private void Start()
        {
            foreach (var key in keyList)
            {
                key.OnKeyClicked += GenerateInput;
            }
            capslockFlag = isUppercase;
            CapsLock();
        }
        #endregion

        #region Public Methods
        // 백스페이스
        public void Backspace()
        {
            if (URLField.Length > 0)
            {
                int temp = URLField.Length-1;
                URLField = urlField.text.Substring(0,temp);
            }
            else
            {
                return;
            }
        }

        // 텍스트 전부 지우기
        public void Clear()
        {
            URLField = "";
        }

        // 캡스 락
        public void CapsLock()
        {
            foreach (var key in keyList)
            {
                if (key is Alphabet)
                {
                    key.CapsLock(capslockFlag);
                }
            }
            capslockFlag = !capslockFlag;
        }

        // 시프트
        public void Shift()
        {
            foreach (var key in keyList)
            {
                if (key is Shift)
                {
                    key.ShiftKey();
                }
            }
        }

        public void GenerateInput(string s)
        {
            if (Input.Length > maxInputLength) { return; }
            URLField += s;
        }

        public void Enter()
        {
            //youtubePlayer.Play(Input);
            // 포톤 뷰 객체 얻어오기
            PhotonView photonView = PhotonView.Get(this);
            // 포톤 뷰를 통해 RPC로 메세지(함수) 전송
            // photonView.RPC("함수명", 받을 클라이언트, 파라미터);
            photonView.RPC("OnPlayYouTubePlayer", RpcTarget.All, Input);
        }

        // 포톤 서버로 부터 RPC를 받았을때 처리 부분
        [PunRPC]
        public void OnPlayYouTubePlayer(string url)
        {
            youtubePlayer.Play(url);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext("");
            }
            else
            {
                stream.ReceiveNext();
            }
        }
        #endregion
    }
}