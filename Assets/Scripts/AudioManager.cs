using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    private void Awake()=>instance = this;

    private int bgmIndex;

    private void Update()
    {
        if (!bgm[bgmIndex].isPlaying)
            PlayRandomBGM();
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlaySFX(int sfxToPlay)
    {
        if (sfxToPlay < sfx.Length)
        {
            sfx[sfxToPlay].pitch = Random.Range(0.85f, 1.15f);
            sfx[sfxToPlay].Play();
        }
            
    }

    public void StopSFX(int index)
    {
        sfx[index].Stop();
    }

    public void PlayBGM(int index)
    {
        StopBGM();
        bgm[index].Play();
    }

    public void StopBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}
