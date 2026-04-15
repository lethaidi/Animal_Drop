using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Loading : MonoBehaviour
{
    public Slider slider;
    public float loadingTime = 3f; // thời gian load (giây)

    public bool isLoaded = false;

    public AudioSource audioSource;

    void OnEnable()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        slider.value = 0f;
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        float time = 0f;

        while (time < loadingTime)
        {
            time += Time.deltaTime;

            float progress = time / loadingTime;

            slider.value = progress;

            yield return null;
        }

        if (audioSource != null)
        {
            audioSource.Play();
        }

        slider.value = 1f;
        isLoaded = true;

        Debug.Log("Loading Done");

        // tắt object loading (nếu muốn)
        gameObject.SetActive(false);
    }
}