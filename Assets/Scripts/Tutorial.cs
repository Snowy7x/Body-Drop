using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private int currentIndex = -1;
    [SerializeField] private GameObject[] panels;

    public void Init()
    {
        Next();
    }

    public void Next()
    {
        currentIndex = Mathf.Min(panels.Length-1, currentIndex + 1);
        Open();
    }

    public void Previous()
    {
        currentIndex = Mathf.Max(0, currentIndex - 1);
        Open();
    }

    void Open()
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        panels[currentIndex].SetActive(true);
    }

    public void Close()
    {
        PlayerPrefs.SetInt("Tut", 1);
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        
        GameManager.Instance.StartGame();
    }
}
