using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextSlider : MonoBehaviour
{
    public TextMeshProUGUI VolumeText;
    private Slider slider;
    GameObject ObjectGameMusic;
    AudioSource audioSource;
    void Start() {
        slider = GetComponent<Slider>();
        slider.value = 100 * PlayerPrefs.GetFloat("volume", 1f);
    }

    public void SetVolumeNumber(float value) {
        ObjectGameMusic = GameObject.FindWithTag("GameMusic");
        audioSource = ObjectGameMusic.GetComponent<AudioSource>();
        audioSource.volume = value / 100f;
        PlayerPrefs.SetFloat("volume", audioSource.volume);
        VolumeText.text = value.ToString();
    }
}
