using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    Playing,
    GameOver_Win,
    GameOver_Lose
}

public class BeatSlimeGameManager : MonoBehaviour
{
    public Color[] noteTypeToColor = new Color[]
    {
        Color.HSVToRGB(200.0f / 360.0f, 50.0f / 100.0f, 90.0f / 100.0f),
        Color.HSVToRGB(100.0f / 360.0f, 50.0f / 100.0f, 90.0f / 100.0f),
    };

    static public BeatSlimeGameManager GetGameManagerInScene(string name = "BeatSlimeGameManager")
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            return go.GetComponent<BeatSlimeGameManager>();
        }
        return null;
    }

    public GameState gameState = GameState.None;

    private BeatController beatController = null;

    // Start is called before the first frame update
    void Start()
    {
        beatController = BeatController.GetBeatControllerInScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlaying && beatController.IsAudioEnd)
        {
            GameOver(true);
        }
    }

    public bool IsPlaying
    {
        get
        {
            return gameState == GameState.Playing;
        }
    }

    public bool StartGame()
    {
        if (!IsPlaying)
        {
            gameState = GameState.Playing;
            beatController.StartPlaying();
            return true;
        }
        return false;
    }
    public void GameOver(bool isSongEnd = false)
    {
        gameState = isSongEnd ? GameState.GameOver_Win : GameState.GameOver_Lose;
        beatController.StopPlaying();
        // TODO: Record...
    }
}
