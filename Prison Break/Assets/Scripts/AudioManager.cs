using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> m_Sounds;

    private AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (AudioClip audioClip in m_Sounds)
            if (audioClip.loadState != AudioDataLoadState.Loaded)
                audioClip.LoadAudioData();

        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string soundName)
    {
        foreach (AudioClip audioClip in m_Sounds)
        {
            if (audioClip.name.Equals(soundName))
            {
                m_AudioSource.clip = audioClip;
                m_AudioSource.Play();
                break;
            }
        }
    }
}
