using UnityEngine;

public class UIOrientationHandler : MonoBehaviour
{
    public GameObject portraitUI;
    public GameObject landscapeUI;

    private bool isPortrait;

    void Start()
    {
        CheckOrientation();
        UpdateUI();
    }

    void Update()
    {
        bool current = Screen.height >= Screen.width;

        if (current != isPortrait)
        {
            isPortrait = current;
            UpdateUI();
        }
    }

    void CheckOrientation()
    {
        isPortrait = Screen.height >= Screen.width;
    }

    void UpdateUI()
    {
        if (portraitUI != null)
            portraitUI.SetActive(isPortrait);

        if (landscapeUI != null)
            landscapeUI.SetActive(!isPortrait);
    }
}