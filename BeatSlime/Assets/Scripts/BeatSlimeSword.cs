using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class BeatSlimeSwordData
{
    public float perfectScore = 100.0f;
}

//-------------------------------------------------------------------------
[RequireComponent(typeof(Interactable))]
public class BeatSlimeSword : MonoBehaviour
{
    public BeatSlimeSwordData data = new BeatSlimeSwordData();

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    private float attachTime;

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

    private Interactable interactable;
    private bool lastHovering = false;

    private BeatSlimePlayer owningPlayer = null;
    //-------------------------------------------------
    void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

        }
        else if (isGrabEnding)
        {
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
        if (other != null && other.CompareTag("Slime"))
        {
            if (owningPlayer != null && CanDamage)
            {
                // TODO: Player add score. Slime get hurt.
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
