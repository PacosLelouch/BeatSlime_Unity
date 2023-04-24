using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSlimeGameManager : MonoBehaviour
{
    static public BeatSlimeGameManager GetGameManagerInScene(string name = "BeatSlimeGameManager")
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            return go.GetComponent<BeatSlimeGameManager>();
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver(bool isSongEnd = false)
    {

    }
}
