﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] GameObject musicSlider;
    [SerializeField] GameObject effectSlider;

    void Start()
    {
        mixer.GetFloat("MusicVolume", out float tempMusicVolume);
        musicSlider.GetComponent<Slider>().value = Mathf.Pow(10, tempMusicVolume / 20);
        mixer.GetFloat("EffectVolume", out float tempEffectVolume);
        effectSlider.GetComponent<Slider>().value = Mathf.Pow(10, tempEffectVolume / 20);
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetEffectVolume(float volume)
    {
        mixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
    }
}
