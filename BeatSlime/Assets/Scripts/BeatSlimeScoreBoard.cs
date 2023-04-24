using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSlimeScoreBoard : MonoBehaviour
{
    public Text textScore;
    public Text textCombo;
    public Text textLife;
    private BeatSlimePlayer player;

    // Start is called before the first frame update
    void Start()
    {
        player = BeatSlimePlayer.GetPlayerInScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (textScore != null && player != null && player.data != null)
        {
            textScore.text = "Score: " + player.data.score.ToString();
        }
        if (textCombo != null && player != null && player.data != null)
        {
            textCombo.text = "Combo: " + player.data.combo.ToString();
        }
        if (textLife != null && player != null && player.data != null)
        {
            textLife.text = "Life: " + player.data.life.ToString() + "/" + player.data.maxLife.ToString();
        }
    }
}
