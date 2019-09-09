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
        public void Backspace()
        {
            if (URLField.Length > 0)
            {
                //Input = Input.Remove(Input.Length - 1);
                //Input.Remove(Input.Length - 1, 1);
                int temp = URLField.Length-1;
                URLField = urlField.text.Substring(0,temp);
                //URLField.Remove(URLField.Length - 1);
            }
            else
            {
                return;
            }
        }

        public void Clear()
        {
            URLField = "";
        }

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
            youtubePlayer.Play(Input);
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