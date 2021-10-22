using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GameStart()
    {
        int seed = 0;
        for (int i = 0; i < 8; i++)
        {
            seed += Random.Range(1, 10) * (int) Mathf.Pow(10, i);
        }
        Random.InitState(seed);
        Debug.Log("GameManager: Game start, seed: " + seed);
        SceneManager.LoadScene("Start");
    }

    public void GameOver()
    {

    }

    public void NextLevel()
    {
        PlayerManager.Instance.CameraLoadingFadeIn(() => 
        {
            SceneManager.LoadScene("Loading");
        });
        // SceneManager.LoadScene("Loading");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
