using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI rankingText;
    public GameObject helpPanel;

    private const string ScoreKeyPrefix = "Score_";
    private const int MaxRanking = 10;

    private void Start()
    {
        UpdateRankingDisplay();
    }

    void UpdateRankingDisplay()
    {
        string display = "[ Best Score ]\n\n";
        bool hasScore = false;

        for (int i = 0; i < MaxRanking; i++)
        {
            int score = PlayerPrefs.GetInt(ScoreKeyPrefix + i, -1);
            if (score >= 0)
            {
                display += $"{i + 1}위    {score} 점\n";
                hasScore = true;
            }
            else
            {
                display += $"{i + 1}위    -\n";
            }
        }

        if (!hasScore)
            display += "\n아직 기록이 없습니다!";

        rankingText.text = display;
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    }

    public void OnHelpButton()
    {
        helpPanel.SetActive(true);
    }

    public void OnHelpCloseButton()
    {
        helpPanel.SetActive(false);
    }

    public static void SaveScore(int newScore)
    {
        int[] scores = new int[MaxRanking];
        for (int i = 0; i < MaxRanking; i++)
            scores[i] = PlayerPrefs.GetInt(ScoreKeyPrefix + i, -1);

        for (int i = 0; i < MaxRanking; i++)
        {
            if (newScore > scores[i])
            {
                for (int j = MaxRanking - 1; j > i; j--)
                    scores[j] = scores[j - 1];
                scores[i] = newScore;
                break;
            }
        }

        for (int i = 0; i < MaxRanking; i++)
            PlayerPrefs.SetInt(ScoreKeyPrefix + i, scores[i]);

        PlayerPrefs.Save();
    }
}
