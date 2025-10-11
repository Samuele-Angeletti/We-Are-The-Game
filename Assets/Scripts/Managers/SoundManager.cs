using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] float fadeInDuration = 2f;
    [SerializeField] float fadeOutDuration = 2f;
    [SerializeField] List<AudioClip> soundtracks;
    [SerializeField, Range(0f, 1f)] float fadeInMaximumValue;
    AudioSource audioSource;
    int _soundtrackIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(PlaySoundtracksCoroutine());
        FadeIn();
    }

    private IEnumerator PlaySoundtracksCoroutine()
    {
        while (true)
        {
            SetNextSoundtrack();

            while (audioSource.isPlaying)
                yield return null;
        }
    }

    private void SetNextSoundtrack()
    {
        _soundtrackIndex++;
        if (_soundtrackIndex >= soundtracks.Count)
            _soundtrackIndex = 0;

        audioSource.clip = soundtracks[_soundtrackIndex];
        audioSource.Play();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeAudio(true, fadeInDuration));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeAudio(false, fadeOutDuration));
    }

    private IEnumerator FadeAudio(bool fadeIn, float fadeInDuration)
    {
        float startVolume = fadeIn ? 0f : audioSource.volume;
        float endVolume = fadeIn ? fadeInMaximumValue : 0f;
        float elapsedTime = 0f;

        if (fadeIn)
        {
            audioSource.volume = 0f;
            audioSource.Play();
        }

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeInDuration);
            try
            {
                // This is to prevent Unity Editor from freezing
                System.Threading.Thread.Sleep(1);
            }
            catch (Exception) { }
            yield return null;
        }

        audioSource.volume = endVolume;

        if (!fadeIn)
        {
            audioSource.Stop();
        }
    }
}
