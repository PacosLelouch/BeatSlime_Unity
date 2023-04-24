using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeatQuality
{
    Miss,
    Poor,
    Good,
    Excellent
}

public class BeatController : MonoBehaviour
{
    static public BeatController GetBeatControllerInScene(string name = "BeatController")
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            return go.GetComponent<BeatController>();
        }
        return null;
    }

    public float bpm;
    public GameObject slimeObject;
    public float jumpHeight;
    public float timeOffset;

    private AudioSource audioSource;
    private float beatDuration;
    private float lastBeatTime;
    private float nextBeatTime;

    private GameObject playerObject;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        beatDuration = 60f / bpm;
        lastBeatTime = 0f;
        nextBeatTime = beatDuration;
        playerObject = BeatSlimePlayer.GetPlayerInScene().gameObject;
    }

    void Update()
    {
        if (audioSource.isPlaying)
        {
            float time = audioSource.time + timeOffset;
            Rigidbody slimeRigidBody = slimeObject.GetComponent<Rigidbody>();
            if (time >= nextBeatTime)
            {
                slimeRigidBody.velocity = new Vector3(0f, jumpHeight, 0f);
                lastBeatTime = nextBeatTime;
                nextBeatTime += beatDuration;
            }
            Vector3 playerDirection2D = playerObject.transform.position - slimeObject.transform.position;
            playerDirection2D.y = 0.0f;
            playerDirection2D.Normalize();

            slimeRigidBody.MoveRotation(Quaternion.LookRotation(playerDirection2D));
            slimeRigidBody.velocity += playerDirection2D * 0.05f;
        }
    }

    public float AudioTime
    {
        get
        {
            return audioSource.time + timeOffset;
        }
    }

    public BeatQuality GetHitQuality(float hitTime)
    {
        float delta = Mathf.Abs(hitTime - lastBeatTime);
        if (delta <= beatDuration / 4f)
        {
            return BeatQuality.Excellent; // Excellent
        }
        else if (delta <= beatDuration / 2f)
        {
            return BeatQuality.Good; // Good
        }
        else if (delta <= beatDuration)
        {
            return BeatQuality.Poor; // Poor
        }
        else
        {
            return BeatQuality.Miss; // Miss
        }
    }

    public void StopPlaying()
    {
        audioSource.time = 0.0f;
        audioSource.Stop();
    }
    public void StartPlaying()
    {
        audioSource.time = 0.0f;
        audioSource.Play();
    }
}
