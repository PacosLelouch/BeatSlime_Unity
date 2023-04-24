using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSlimePlayerData
{
    public float maxLife = 100.0f;
    public float life = 100.0f;
    public float score = 0.0f;
    public int combo = 0;
}

public class BeatSlimePlayer : MonoBehaviour
{
    static public BeatSlimePlayer GetPlayerInScene(string name = "BeatSlimePlayer")
    {
        return GameObject.Find(name).GetComponent<BeatSlimePlayer>();
    }

    public BeatSlimePlayerData data = new BeatSlimePlayerData();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
