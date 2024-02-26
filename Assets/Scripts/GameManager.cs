using Arc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] bool isPlayingWithAI = false;
    public bool IsPlayingWithAI { get { return isPlayingWithAI; } set { isPlayingWithAI = value; } }



    [SerializeField] bool isAIPlaying = false;
    public bool IsAIPlaying { get { return isAIPlaying; } set {  isAIPlaying =value; } }

    public void Intialize()
    {
        BoardManager.Instance.Intialize();
    }
}
