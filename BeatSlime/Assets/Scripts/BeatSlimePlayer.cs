using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSlimePlayerData
{
    public float maxLife = 100.0f;
    public float life = 100.0f;
    public float score = 0.0f;
    public int combo = 0;

    public void Reset()
    {
        combo = 0;
        score = 0.0f;
        life = maxLife;
    }
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

    public bool RestoreLife(float value = 1.0f)
    {
        if (gameManager.IsPlaying)
        {
            if (data.life < data.maxLife)
            {
                data.life = Mathf.Min(data.life + value, data.maxLife);
                return true;
            }
        }
        return false;
    }

    public bool GetHurt(float value = 5.0f)
    {
        if (gameManager.IsPlaying)
        {
            if (data.life > 0.0f)
            {
                data.life = Mathf.Max(0.0f, data.life - value);
            }
            if (data.life <= 0.0f)
            {
                gameManager.GameOver(false);
            }
            return true;
        }
        return false;
    }

    #region Collision & Trigger
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other != null && other.CompareTag("Slime"))
        {
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
