using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int lives;
    private int currentLevel;
    private int nextLevel;
    private int score;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    private void LoadLevel(int index)
    {
        currentLevel = index;

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.cullingMask = 0;
        }

        Invoke(nameof(LoadScene), 0.5f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(currentLevel);
    }

    private void NewGame()
    {
        lives = 2;
        score = 0;
        nextLevel = 1;
        LoadLevel(1);
    }

    public void LevelComplete()
    {
        score += 1000;
        nextLevel = nextLevel += 1;

        if (nextLevel >= SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(1);
        }
        else
        {
            LoadLevel(nextLevel);
        }
    }

    public void LevelFailed()
    {
        lives--;
        if (lives <= -1)
        {
            NewGame();
        }
        else
        {
            LoadLevel(currentLevel);
        }
    }
}
