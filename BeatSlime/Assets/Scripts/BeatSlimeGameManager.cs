using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSlimeGameManager : MonoBehaviour
{
    static public BeatSlimeGameManager GetGameManagerInScene(string name = "BeatSlimeGameManager")
    {
        return GameObject.Find(name).GetComponent<BeatSlimeGameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
