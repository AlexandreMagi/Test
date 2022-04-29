using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("BestScore") && PlayerPrefs.HasKey("BestTime"))
        {
            var bestTime = PlayerPrefs.GetInt("BestTime");
            string secText = bestTime % 60 > 9 ? "" + bestTime % 60 : "0" + bestTime % 60;

            scoreText.text = $"Best moves : {PlayerPrefs.GetInt("BestScore")} | Best time : {Mathf.Floor(bestTime / 60)}:{secText}";
        }
        else
        {
            scoreText.text = "";
        }
    }
}
