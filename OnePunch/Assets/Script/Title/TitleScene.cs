using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScene : MonoBehaviour
{
    // 싱글 플레이로 시작
    public void OnClickSinglePlay()
    {
        SceneManager.LoadScene("PlayScene");
        Manager.Instance.InitCall();
        return;
    }

    // 멀티플레이로 시작
    public void OnClickMultiPlay()
    {
        // 아직 멀티플레이는 준비가 안되었다는 팝업
        PopupManager.Instance.ShowPopup(Defines.MultyPlayIsNotReady);
        return;
    }
}
