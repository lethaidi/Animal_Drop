using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("UI")]
    public Slider musicSlider;
    public Slider sfxSlider;

    // PlayerPrefs keys
    const string MUSIC_KEY = "MusicVolume";
    const string SFX_KEY = "SFXVolume";

    // Volume range (dB)
    const float MIN_DB = -70f;
    const float MUSIC_MAX_DB = 0f;
    const float SFX_MAX_DB = 0f;

    void Awake()
    {
        // Khởi tạo giá trị mặc định nếu lần đầu chơi
        if (!PlayerPrefs.HasKey(MUSIC_KEY))
            PlayerPrefs.SetFloat(MUSIC_KEY, 0.5f);

        if (!PlayerPrefs.HasKey(SFX_KEY))
            PlayerPrefs.SetFloat(SFX_KEY, 0.5f);

        PlayerPrefs.Save();
    }

    void Start()
    {
        // Load giá trị từ PlayerPrefs
        float musicVol = PlayerPrefs.GetFloat(MUSIC_KEY);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY);

        // Gán giá trị cho UI Slider
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        // Áp dụng âm thanh ngay khi bắt đầu
        ApplyMusic();
        ApplySFX();
    }

    // ================= MUSIC =================
    public void SetMusic()
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, musicSlider.value);
        PlayerPrefs.Save();
        ApplyMusic();
    }

    void ApplyMusic()
    {
        // Nếu kéo slider về 0, tắt hẳn âm thanh (-80dB)
        if (musicSlider.value <= 0.01f)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        else
        {
            float db = Mathf.Lerp(MIN_DB, MUSIC_MAX_DB, musicSlider.value);
            audioMixer.SetFloat("MusicVolume", db);
        }
    }

    // ================= SFX =================
    public void SetSFX()
    {
        PlayerPrefs.SetFloat(SFX_KEY, sfxSlider.value);
        PlayerPrefs.Save();
        ApplySFX();
    }

    void ApplySFX()
    {
        // Nếu kéo slider về 0, tắt hẳn âm thanh (-80dB)
        if (sfxSlider.value <= 0.01f)
        {
            audioMixer.SetFloat("SFXVolume", -80f);
        }
        else
        {
            float db = Mathf.Lerp(MIN_DB, SFX_MAX_DB, sfxSlider.value);
            audioMixer.SetFloat("SFXVolume", db);
        }
    }
}