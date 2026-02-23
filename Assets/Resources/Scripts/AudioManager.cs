using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    private AudioSource _sfxSource;
    private AudioSource _bgmSource; 
    private readonly Dictionary<AudioType, AudioClip> _clips = new();
    public void Init(AudioSource sfxSource, AudioSource bgmSource)
    {
        _sfxSource=sfxSource;
        _bgmSource=bgmSource;

        foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{type}");

            if (clip == null)
            {
                Debug.LogError($"Audio {type} not found");
                continue;
            }
            _clips[type]=clip;
        }
    }

    public void PlaySfx(AudioType audioType, float volume = 1f)
    {
        if (!_clips.ContainsKey(audioType) || !_clips.TryGetValue(audioType, out AudioClip clip))
        {
            Debug.LogError($"Audio {audioType} not found");
            return;
        }
        _sfxSource.PlayOneShot(clip,volume);
    }

    public void PlayBgm(AudioType audioType, float volume = 1f)
    {
        if (!_clips.ContainsKey(audioType) || !_clips.TryGetValue(audioType, out AudioClip clip))
        {
            Debug.LogError($"Audio {audioType} not found");
            return;
        }
        _bgmSource.clip=clip;
        _bgmSource.volume=volume;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void StopBgm()
    {
        _bgmSource.Stop();
    }

    public void ChangeVolume(float volume)
    {
        _bgmSource.volume=volume;
    }
}
