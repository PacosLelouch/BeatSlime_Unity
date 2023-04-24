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
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            return go.GetComponent<BeatSlimePlayer>();
        }
        return null;
    }

    public BeatSlimePlayerData data = new BeatSlimePlayerData();

    private BeatSlimeGameManager gameManager = null;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = BeatSlimeGameManager.GetGameManagerInScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Collision & Trigger
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other != null && other.CompareTag("Slime"))
        {
            if (data.life > 0.0f)
            {
                data.life -= 5.0f;
            }
            if (data.life <= 0.0f)
            {
                gameManager.GameOver(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
    #endregion
}
