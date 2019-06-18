using UnityEngine;
using System.Collections;

public class EffectVolume : MonoBehaviour
{
    public void Awake()
    {
        if(this.GetComponent<AudioSource>() != null)
            this.GetComponent<AudioSource>().volume = this.GetComponent<AudioSource>().volume*SoundManager.Instance.GetcEffPlayer().volume;
    }
}