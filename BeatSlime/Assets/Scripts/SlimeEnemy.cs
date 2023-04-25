using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeEnemy : MonoBehaviour
{
    public Text debugQualityText;

    private GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = BeatSlimePlayer.GetPlayerInScene().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        BeatController beatController = BeatController.GetBeatControllerInScene();
        float hitTime = beatController.AudioTime;
        BeatQuality quality = beatController.GetHitQuality(hitTime);
        // TODO: Visualize quality...
        if (debugQualityText != null)
        {
            debugQualityText.text = quality.ToString();
            float ratio = (float)quality / (float)BeatQuality.Excellent;
            debugQualityText.color = Color.HSVToRGB(1.0f / 3.0f * ratio, 1.0f, 1.0f);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        
    }
    #endregion
}
