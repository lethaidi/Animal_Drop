using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("UI")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    // PlayerPrefs keys
    const string MUSIC_KEY = "MusicEnabled";
    const string SFX_KEY = "SFXEnabled";

    void Awake()
    {
        // Giá trị mặc định: bật âm
        if (!PlayerPrefs.HasKey(MUSIC_KEY))
            PlayerPrefs.SetInt(MUSIC_KEY, 1);

        if (!PlayerPrefs.HasKey(SFX_KEY))
            PlayerPrefs.SetInt(SFX_KEY, 1);

        PlayerPrefs.Save();
    }

    void Start()
    {
        // Load trạng thái
        bool musicOn = PlayerPrefs.GetInt(MUSIC_KEY) == 1;
        bool sfxOn = PlayerPrefs.GetInt(SFX_KEY) == 1;

        // Gán cho Toggle
        musicToggle.isOn = musicOn;
        sfxToggle.isOn = sfxOn;

        // Áp dụng ngay
        ApplyMusic(musicOn);
        ApplySFX(sfxOn);
    }

    // ================= MUSIC =================
    public void SetMusic(bool isOn)
    {
        PlayerPrefs.SetInt(MUSIC_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMusic(isOn);
    }

    void ApplyMusic(bool isOn)
    {
        if (isOn)
            audioMixer.SetFloat("MusicVolume", -20f);   // bật
        else
            audioMixer.SetFloat("MusicVolume", -80f); // tắt
    }

    // ================= SFX =================
    public void SetSFX(bool isOn)
    {
        PlayerPrefs.SetInt(SFX_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();
        ApplySFX(isOn);
    }

    void ApplySFX(bool isOn)
    {
        if (isOn)
            audioMixer.SetFloat("SFXVolume", -10f);   // bật
        else
            audioMixer.SetFloat("SFXVolume", -80f); // tắt
    }
}