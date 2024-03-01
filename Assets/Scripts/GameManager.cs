using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Tutorial tut;

    [SerializeField] Player player;
    [SerializeField] private float neededToWin = 6;
    private float alreadyBroken;

    private bool started = false;
    [SerializeField] private GameObject[] interactiveObjects;
    [SerializeField] TMP_Text currentScore;

    [SerializeField] private GameObject loseMenu;
    [SerializeField] private GameObject winMenu;

    [SerializeField] TMP_Text winScore;
    [SerializeField] TMP_Text winHeightScore;
    [SerializeField] TMP_Text loseHeightScore;

    private float timer;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else Destroy(this);

        foreach (var obj in interactiveObjects)
        {
            obj.SetActive(false);
        }

        if (PlayerPrefs.HasKey("Tut"))
        {
            if (PlayerPrefs.GetInt("Tut") == 1) tut.Close();
            else tut.Init();
        }
        else
        {
            PlayerPrefs.SetInt("Tut", 0);
            tut.Init();
        }

        if (!PlayerPrefs.HasKey("HS"))
        {
            PlayerPrefs.SetInt("HS", 0);
        }
    }

    public void StartGame()
    {
        foreach (var obj in interactiveObjects)
        {
            obj.SetActive(true);
        }

        started = true;
    }

    private void Update()
    {
        if (!started) return;
        timer += Time.deltaTime;
        currentScore.text = "Score: " + ((int)timer).ToString();
    }


    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void Restart()
    {
        loseHeightScore.text += PlayerPrefs.GetInt("HS").ToString();
        loseMenu.SetActive(true);
        winMenu.SetActive(false);
        //StartCoroutine(DelayRestart());
    }

    IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(2);
        RestartScene();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Broken()
    {
        alreadyBroken++;

        if (alreadyBroken >= neededToWin) Won();
    }

    private void Won()
    {
        started = false;
        loseMenu.SetActive(false);
        int hScore = PlayerPrefs.GetInt("HS");
        winScore.text += timer.ToString();
        if (timer > hScore)
        {
            hScore = (int)timer;
            PlayerPrefs.SetInt("HS", (int)timer);
        }

        winHeightScore.text += hScore.ToString();
        winMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
