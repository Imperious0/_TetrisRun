using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("End Game")]
    [SerializeField]
    private GameObject endGameUI;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI nextLevelBtnText;

    [SerializeField]
    private GameObject endGameFailUI;
    [SerializeField]
    private TextMeshProUGUI endGameFailText;

    [SerializeField]
    private GameObject gameStartUI;

    private void Start()
    {
        levelText.text = "Level " + PlayerPrefs.GetInt("Level", 1) + " Success!";
        nextLevelBtnText.text = "GO Level " + (PlayerPrefs.GetInt("Level", 1) + 1) + " !";
        endGameUI.SetActive(false);

        endGameFailText.text = "Level " + PlayerPrefs.GetInt("Level", 1) + " FAILED!";
        endGameFailUI.SetActive(false);

        if (PlayerPrefs.GetInt("IsFirstTimePlay", 1) == 0)
            gameStartUI.SetActive(false);

    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().endGameEvent += showEndGameUI;
    }
    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().endGameEvent -= showEndGameUI;
    }
    public void showEndGameUI(object sender, PlayerController.endGameEventArgs e)
    {
        if (e.IsGameSuccess)
        {
            this.endGameUI.SetActive(true);
        }
        else
        {
            this.endGameFailUI.SetActive(true);
        }
    }
    public void startGame()
    {
        gameStartUI.SetActive(false);
        endGameFailUI.SetActive(false);
        Camera.main.GetComponent<GameController>().resetGame();
        PlayerPrefs.SetInt("IsFirstTimePlay", 0);
        PlayerPrefs.Save();

    }
    public void startNextGame()
    {
        endGameUI.SetActive(false);
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
        PlayerPrefs.Save();
        Camera.main.GetComponent<GameController>().restartGame();
    }

    public void exitGame()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("IsFirstTimePlay", 1);
        PlayerPrefs.Save();
    }
}
