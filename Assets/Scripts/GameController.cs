using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject longRoad;
    [SerializeField]
    private GameObject roadPrefab;
    [SerializeField]
    private GameObject[] obstaclePrefabs;

    [SerializeField]
    private Vector3 startPosition = Vector3.zero;

    [SerializeField]
    private GameSettings gSettings;

    int gameDifficulty = 1;

    int obstacleCount = 0;
    int obstacleCounter = 0;

    List<GameObject> obstacles;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(101104 + PlayerPrefs.GetInt("Level", 1)); 
        //Recreate Sky color
        if (RenderSettings.skybox.HasProperty("_Tint"))
            RenderSettings.skybox.SetColor("_Tint", Random.ColorHSV(0.6f, 0.85f, 0.25f, 0.5f, 0.75f, 1f));
        else if (RenderSettings.skybox.HasProperty("_SkyTint"))
            RenderSettings.skybox.SetColor("_SkyTint", Random.ColorHSV(0.6f, 0.85f, 0.25f, 0.5f, 0.75f, 1f));

        gameDifficulty = Random.Range(1, gSettings.GameMaxDifficulty);
        obstacleCount = Random.Range(gameDifficulty * 5, gameDifficulty * 10);
        obstacles = new List<GameObject>();

        createObstacles();

        sendNextBlock();

    }

    private void createObstacles() 
    {
        int realCounter = 0;
        float obstacleOffset = GameObject.FindGameObjectsWithTag("Road").Length * roadPrefab.transform.localScale.x;

        for (int i = 0; i < obstacleCount; i++)
        {
            obstacleOffset = createRoad(obstacleOffset);
            realCounter++;

            obstacles.Add(Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]));

            Vector3 obstacleSetter = obstacles[i].transform.position;
            obstacleOffset += obstacles[i].GetComponent<BlockHandler>().WidthOfBlock / 2f;
            obstacleSetter.x = startPosition.x + obstacleOffset;
            obstacleOffset += obstacles[i].GetComponent<BlockHandler>().WidthOfBlock / 2f;

            obstacles[i].transform.position = obstacleSetter;
            
            realCounter++;
            for (int j = 0; j < Random.Range(1, 3); j++)
            {
                obstacleOffset = createRoad(obstacleOffset);
                realCounter++;
            }
        }

        //Resize Bottom Road
        longRoad.transform.localScale = new Vector3(obstacleOffset + startPosition.x * 2f, 1f, 1f);
        longRoad.transform.position = new Vector3(longRoad.transform.localScale.x / 2f, 0, 0);

        //Finish Platform Created
        GameObject finish = Instantiate(roadPrefab);

        Vector3 finishSetter = finish.transform.position;
        obstacleOffset += finish.transform.localScale.x / 2f;
        finishSetter.x = startPosition.x + obstacleOffset;
        obstacleOffset += finish.transform.localScale.x / 2f;

        finish.transform.position = finishSetter;
        finish.tag = "Finish";
    }
    private float createRoad(float obstacleOffset) 
    {
        GameObject go = Instantiate(roadPrefab);

        Vector3 roadBrokerSetter = go.transform.position;
        obstacleOffset += go.transform.localScale.x / 2f;
        roadBrokerSetter.x = startPosition.x + obstacleOffset;
        obstacleOffset += go.transform.localScale.x / 2f;

        go.transform.position = roadBrokerSetter;

        return obstacleOffset;
    }
    public void sendNextBlock() 
    {
        if(obstacleCounter < obstacleCount) 
        {
            obstacles[obstacleCounter].GetComponent<BlockHandler>().setMyTurn(true);
            obstacleCounter++;
        }
    }
    public void resetGame()
    {
        obstacleCounter = 0;
        for (int i = 0; i < obstacleCount; i++) 
        {
            obstacles[i].GetComponent<BlockHandler>().resetBlock();
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Restart();
        this.GetComponent<MotionCapture>().resetMotion();
        sendNextBlock();
    }
    public void restartGame() 
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
