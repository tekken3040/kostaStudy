using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using YoutubeExtractor;
using UnityEngine.UI;

public class VideoPlayerURL : MonoBehaviour
{
    [SerializeField] private VideoPlayer _video;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private string url;
    [SerializeField] private int quality;
    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private Text URL_Text;

    private void Start()
    {
        StartCoroutine(DelayedPlay());
    }

    private void Update()
    {
        _audio.volume = VolumeSlider.value;
    }

    public void Run()
    {
        IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);
        VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == quality);

        if(video.RequiresDecryption)
        {
            DownloadUrlResolver.DecryptDownloadUrl(video);
        }

        _video.url = video.DownloadUrl;
        Debug.Log(video.DownloadUrl);
    }

    private IEnumerator DelayedPlay()
    {
        Debug.Log("Wait 3s");
        yield return new WaitForSeconds(3f);
        Debug.Log("Play");
        Run();
        _video.Play();
    }

    public void OnClickPlay()
    {
        if(URL_Text.text == "")
            return;

        Run();
        _video.Play();
    }
}
