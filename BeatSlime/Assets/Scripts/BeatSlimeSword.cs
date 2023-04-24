using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class BeatSlimeSwordData
{
    public readonly Dictionary<BeatQuality, float> scoreDict = new Dictionary<BeatQuality, float>
    {
        { BeatQuality.Miss, -50.0f },
        { BeatQuality.Poor, 0.0f },
        { BeatQuality.Good, 50.0f },
        { BeatQuality.Excellent, 100.0f }
    };
}

//-------------------------------------------------------------------------
[RequireComponent(typeof(Interactable))]
public class BeatSlimeSword : MonoBehaviour
{
    public GameObject swordObject = null;

    public BeatSlimeSwordData data = new BeatSlimeSwordData();

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    private float attachTime;

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

    private Hand playerHand;

    private Interactable interactable;
    private bool lastHovering = false;

    private BeatSlimeGameManager gameManager = null;
    private BeatSlimePlayer owningPlayer = null;
    private BeatController beatController = null;
    //-------------------------------------------------
    void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        beatController = BeatController.GetBeatControllerInScene();
        gameManager = BeatSlimeGameManager.GetGameManagerInScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable.isHovering != lastHovering) //save on the .tostrings a bit
        {
            lastHovering = interactable.isHovering;
        }
    }

    #region State.
    // Can damage slime only when it is attached to hand. 
    public bool CanDamage
    {
        get
        {
            return interactable.attachedToHand != null;
        }
    }
    #endregion

    #region Interactable.
    //-------------------------------------------------
    // Called every Update() while a Hand is hovering over this object
    //-------------------------------------------------
    private void HandHoverUpdate(Hand hand)
    {
        playerHand = hand;
        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

        BeatSlimePlayer playerScript = hand.transform.root.GetComponent<BeatSlimePlayer>();

        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            // Save our position/rotation so that we can restore it when we detach
            oldPosition = transform.position;
            oldRotation = transform.rotation;

            // Call this to continue receiving HandHoverUpdate messages,
            // and prevent the hand from hovering over anything else
            hand.HoverLock(interactable);

            // Attach this object to the hand
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);

            owningPlayer = playerScript;
            swordObject.GetComponent<BoxCollider>().enabled = true;

            gameManager.StartGame();

        }
        else if (isGrabEnding)
        {
            swordObject.GetComponent<BoxCollider>().enabled = false;
            owningPlayer = null;

            // Detach this object from the hand
            hand.DetachObject(gameObject);

            // Call this to undo HoverLock
            hand.HoverUnlock(interactable);

            // Restore position/rotation
            //transform.position = oldPosition;
            //transform.rotation = oldRotation;
        }
    }
    #endregion

    #region Collision & trigger.
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        /*if (other != null && other.CompareTag("Slime"))
        {
            if (owningPlayer != null && CanDamage)
            {
                float hitTime = beatController.AudioTime;
                BeatQuality quality = beatController.GetHitQuality(hitTime);

                //float ratio = (float)quality / (float)BeatQuality.Excellent;
                //if (ratio > 0)
                //{
                //    owningPlayer.data.score += data.perfectScore * ratio;
                //}
                float addScore = data.scoreDict.GetValueOrDefault(quality, 0.0f);
                owningPlayer.data.score += addScore;

                if (quality >= BeatQuality.Good)
                {
                    ++owningPlayer.data.combo;
                }
            }
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Slime"))
        {
            if (owningPlayer != null && CanDamage)
            {
                float hitTime = beatController.AudioTime;
                BeatQuality quality = beatController.GetHitQuality(hitTime);

                //float ratio = (float)quality / (float)BeatQuality.Excellent;
                //if (ratio > 0)
                //{
                //    owningPlayer.data.score += data.perfectScore * ratio;
                //}
                float addScore = data.scoreDict.GetValueOrDefault(quality, 0.0f);
                owningPlayer.data.score += addScore;

                if (quality >= BeatQuality.Good)
                {
                    ++owningPlayer.data.combo;

                    //haptic feedback when the player has a good hit
                    if (playerHand!= null)
                    {
                        playerHand.TriggerHapticPulse((ushort)0.5);
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
    #endregion
}
