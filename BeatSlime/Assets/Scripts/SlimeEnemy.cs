using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    private GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = BeatSlimePlayer.GetPlayerInScene().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Collision & Trigger.
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other.CompareTag("Sword"))
        {
            Debug.Log("Slime -> Sword");
        }
        else
        {
            GameObject collidedRoot = other.gameObject.transform.root.gameObject;
            if (collidedRoot.CompareTag("Player"))
            {
                collidedRoot.GetComponent<BeatSlimePlayer>().GetHurt();
            }
        }
    }
    #endregion
}
