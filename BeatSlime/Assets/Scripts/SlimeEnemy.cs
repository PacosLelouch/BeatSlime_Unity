using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SlimeEnemyQualityDebugLevel
{
    NoDebug,
    GE_Good,
    All
}

public class SlimeEnemy : MonoBehaviour
{
    public SlimeEnemyQualityDebugLevel qualityDebugLevel = SlimeEnemyQualityDebugLevel.GE_Good;
    public Text debugQualityText;
    public GameObject slimeMesh;
    public Animator slimeAnimator;

    private GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = BeatSlimePlayer.GetPlayerInScene().gameObject;
        slimeAnimator = GetComponent<Animator>();
    }

    public void AttackAction()
    {
        // Add animations in this function.
        //slimeAnimator.SetTrigger("Attack");
    }

    public void GetHurt(BeatQuality quality)
    {
        // Add animations in this function. 
        slimeAnimator.SetTrigger("GetHit");
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
            if (qualityDebugLevel == SlimeEnemyQualityDebugLevel.All ||
                qualityDebugLevel == SlimeEnemyQualityDebugLevel.GE_Good && quality >= BeatQuality.Good)
            {
                debugQualityText.text = quality.ToString();
                float ratio = (float)quality / (float)BeatQuality.Excellent;
                debugQualityText.color = Color.HSVToRGB(1.0f / 3.0f * ratio, 1.0f, 1.0f);
            }
            else
            {
                debugQualityText.text = "";
            }
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
