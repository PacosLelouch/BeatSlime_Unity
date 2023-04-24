using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatController : MonoBehaviour
{
    public float bpm;
    public GameObject slimeObject;
    public float jumpHeight;
    public float timeOffset;

    private AudioSource audioSource;
    private float beatDuration;
    private float lastBeatTime;
    private float nextBeatTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        beatDuration = 60f / bpm;
        lastBeatTime = 0f;
        nextBeatTime = beatDuration;
    }

    void Update()
    {
        float time = audioSource.time + timeOffset;
        if (time >= nextBeatTime)
        {
            slimeObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, jumpHeight, 0f);
            lastBeatTime = nextBeatTime;
            nextBeatTime += beatDuration;
        }
    }

    public int GetHitQuality(float hitTime)
    {
        float delta = Mathf.Abs(hitTime - lastBeatTime);
        if (delta <= beatDuration / 4f)
        {
            return 3; // Excellent
        }
        else if (delta <= beatDuration / 2f)
        {
            return 2; // Good
        }
        else if (delta <= beatDuration)
        {
            return 1; // Poor
        }
        else
        {
            return 0; // Miss
        }
    }

    public void StartPlaying()
    {
        audioSource.Play();
    }
}
