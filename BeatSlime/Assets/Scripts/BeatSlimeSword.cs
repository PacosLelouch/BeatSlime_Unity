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
    public AudioClip swordHitSoundEffect_Poor = null;
    public AudioClip swordHitSoundEffect_Good = null;
    public GameObject swordObject = null;
    public GameObject windObject = null;
    public GameObject swordPeekObject = null;

    public float maxBackVelocity = 5.0f;
    public float coldDownPerSwordInSecond = 0.05f;

    public BeatSlimeSwordData data = new BeatSlimeSwordData();

    [SerializeField]
    private float remainingColdDownPerSwordInSecond = 0.0f;
    [SerializeField]
    private Vector3 swordPeekVelocity;

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
        if (remainingColdDownPerSwordInSecond > 0.0f)
        {
            remainingColdDownPerSwordInSecond -= Time.deltaTime;
        }
        if (interactable.isHovering != lastHovering) //save on the .tostrings a bit
        {
            lastHovering = interactable.isHovering;
        }

        // Should we add a constraint that it should be some velocity to apply damage?
        //Rigidbody swordRigidBody = GetComponent<Rigidbody>();
        BoxCollider swordCollider = swordObject.GetComponent<BoxCollider>();
        swordPeekVelocity = swordPeekObject.GetComponent<VelocityEstimator>().GetVelocityEstimate();
            //swordRigidBody.GetPointVelocity(swordPeekObject.transform.position);

        if (swordCollider.isTrigger)
        {
            bool isSwordMoving = IsSwordMoving;
            swordCollider.enabled = true;
            windObject.SetActive(CanDamage && isSwordMoving);
        }
        else
        {
            bool isSwordMoving = IsSwordMoving;
            swordCollider.enabled = isSwordMoving;
            windObject.SetActive(isSwordMoving);
        }
    }

    #region State.
    // Can damage slime only when it is attached to hand. 
    public bool CanDamage
    {
        get
        {
            return owningPlayer != null && interactable.attachedToHand != null;
        }
    }
    public bool IsSwordMoving
    {
        get
        {
            return swordPeekVelocity.magnitude > 0.25f;
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
        CapsuleCollider swordCapsuleCollider = swordObject.GetComponent<CapsuleCollider>();

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
            //swordObject.GetComponent<BoxCollider>().enabled = true;
            swordCapsuleCollider.enabled = false;

            gameManager.StartGame();

        }
        else if (isGrabEnding)
        {
            swordCapsuleCollider.enabled = true;
            //swordObject.GetComponent<BoxCollider>().enabled = false;
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
    private void HandleCollisionOrTrigger(Collider other, bool isTrigger)
    {
        if (other != null && other.CompareTag("Slime"))
        {
            if (CanDamage)
            {
                Rigidbody slimeRigidBody = other.GetComponent<Rigidbody>();
                Rigidbody swordRigidBody = GetComponent<Rigidbody>();

                // Add extra force?
                float extraRatio = isTrigger ? 0.5f : 0.1f;
                Vector3 swordPeekVelocity2D = new Vector3(swordPeekVelocity.x, 0.0f, swordPeekVelocity.z);

                Vector3 playerDirection2D = owningPlayer.transform.position - slimeRigidBody.transform.position;
                playerDirection2D.y = 0.0f;
                playerDirection2D.Normalize();

                //if (Vector3.Dot(swordPeekVelocity2D, playerDirection2D) < 0.0f)
                {
                    Vector3 backVelocity = swordPeekVelocity2D * extraRatio;
                    if (backVelocity.magnitude > maxBackVelocity)
                    {
                        backVelocity = Vector3.back.normalized * maxBackVelocity;
                    }
                    slimeRigidBody.AddForce(backVelocity, ForceMode.VelocityChange);
                }

                if (remainingColdDownPerSwordInSecond <= 0.0f)
                {
                    remainingColdDownPerSwordInSecond += coldDownPerSwordInSecond;
                    float hitTime = beatController.AudioTime;
                    BeatQuality quality = beatController.GetHitQuality(hitTime);

                    float addScore = data.scoreDict.GetValueOrDefault(quality, 0.0f);
                    owningPlayer.data.score += addScore;


                    if (quality >= BeatQuality.Good)
                    {
                        ++owningPlayer.data.combo;

                        //haptic feedback when the player has a good hit
                        if (playerHand != null && playerHand.hapticAction.active)
                        {
                            playerHand.TriggerHapticPulse((ushort)0.5);
                        }

                        if (swordHitSoundEffect_Good != null)
                        {
                            AudioSource.PlayClipAtPoint(swordHitSoundEffect_Good, swordObject.transform.position, 0.25f);
                        }
                    }
                    else
                    {
                        if (swordHitSoundEffect_Poor != null)
                        {
                            AudioSource.PlayClipAtPoint(swordHitSoundEffect_Poor, swordObject.transform.position, 0.25f);
                        }
                    }
                }
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        BoxCollider swordCollider = swordObject.GetComponent<BoxCollider>();
        bool isTrigger = swordCollider.isTrigger;
        if (!isTrigger)
        {
            HandleCollisionOrTrigger(other, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BoxCollider swordCollider = swordObject.GetComponent<BoxCollider>();
        bool isTrigger = swordCollider.isTrigger;
        if (isTrigger)
        {
            HandleCollisionOrTrigger(other, true);
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
