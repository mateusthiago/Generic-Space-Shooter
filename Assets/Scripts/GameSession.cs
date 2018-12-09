using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSession : MonoBehaviour
{
    
    int score;
    int hiScore;

    void Awake()
    {
        SetUpSingleton();
    }

    void SetUpSingleton()
    {
        var mPlayers = FindObjectsOfType(GetType());

        if (mPlayers.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start ()
    {
        score = 0;        
    }

    public void AddScore(int addScore)
    {
        score += addScore;
        FindObjectOfType<Text>().text = score.ToString();
        if (score > hiScore)
        {
            hiScore = score;            
        }        
    }

    public int GetHiScore()
    {
        return hiScore;
    }

    public void ResetScore()
    {
        score = 0;
    }

    void Update ()
    {
		
	}
}
