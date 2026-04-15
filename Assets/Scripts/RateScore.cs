using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RateScore : MonoBehaviour
{
    public static RateScore Instance;

    public GameManager gameManager;
    public TextMeshProUGUI score;
    public Sprite[] rates = new Sprite[5];
    public Image rateShow;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void OnEnable()
    {
        Rate();
    }

    public void Rate()
    {
        int scoreInt = gameManager.highestScore;

        if (score != null)
        {
            score.text = scoreInt.ToString();
        }

        if (scoreInt >= 4000)
        {
            rateShow.sprite = rates[4];
        }
        else if (scoreInt >= 3000)
        {
            rateShow.sprite = rates[3];
        }
        else if (scoreInt >= 2000)
        {
            rateShow.sprite = rates[2];
        }
        else if (scoreInt >= 1000)
        {
            rateShow.sprite = rates[1];
        }
        else
        {
            rateShow.sprite = rates[0];
        }
    }
}
