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

    protected AudioSource audioSource;
    protected float beatDuration;

    [SerializeField]
    protected float lastBeatTime;
    [SerializeField]
    protected float nextBeatTime;

    protected GameObject playerObject;
    protected BeatSlimeGameManager gameManager;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        beatDuration = 60f / bpm;
        lastBeatTime = 0f;
        nextBeatTime = beatDuration;
        playerObject = BeatSlimePlayer.GetPlayerInScene().gameObject;
        gameManager = BeatSlimeGameManager.GetGameManagerInScene();
    }

    protected virtual void Update()
    {
        if (audioSource.isPlaying)
        {
            float time = audioSource.time + timeOffset;

            if (time >= nextBeatTime)
            {
                ApplyBeat();
                AccumulateNextBeatTime();
            }
        }
    }

    protected virtual void AccumulateNextBeatTime()
    {
        lastBeatTime = nextBeatTime;
        nextBeatTime += beatDuration;
    }

    public virtual bool MatchNoteType(int inNoteType)
    {
        return true;
    }

    public virtual void ApplyBeat()
    {
        Rigidbody slimeRigidBody = slimeObject.GetComponent<Rigidbody>();

        Vector3 playerDirection2D = playerObject.transform.position - slimeObject.transform.position;
        playerDirection2D.y = 0.0f;
        playerDirection2D.Normalize();
        
        slimeRigidBody.MoveRotation(Quaternion.LookRotation(playerDirection2D));

        //slimeRigidBody.velocity += new Vector3(0f, jumpHeight, 0f);
        //slimeRigidBody.velocity += playerDirection2D * 0.5f;
        slimeRigidBody.AddForce(new Vector3(0f, jumpHeight, 0f), ForceMode.VelocityChange);
        Vector3 velocity2D = new Vector3(slimeRigidBody.velocity.x, 0.0f, slimeRigidBody.velocity.z);
        float randomRotY = Random.Range(-75.0f, 75.0f);
        Quaternion randomRotation = Quaternion.Euler(0.0f, randomRotY, 0.0f);
        playerDirection2D = randomRotation * playerDirection2D;
        if (Vector3.Dot(playerDirection2D, velocity2D) < 15.0f)
        {
            slimeRigidBody.AddForce(playerDirection2D * 1.5f, ForceMode.VelocityChange);
        }
        SlimeEnemy slimeEnemy = slimeObject.GetComponent<SlimeEnemy>();
        slimeEnemy.AttackAction();
    }

    public float AudioTime
    {
        get
        {
            return audioSource.time + timeOffset;
        }
    }

    public bool IsAudioEnd
    {
        get
        {
            return audioSource.time >= audioSource.clip.length;
        }
    }

    public BeatQuality GetHitQuality(float hitTime)
    {
        float delta = Mathf.Abs(hitTime - lastBeatTime);
        if (delta >= beatDuration * 0.35f && delta <= beatDuration * 0.65f)
        {
            return BeatQuality.Excellent; // Excellent
        }
        else if (delta >= beatDuration * 0.15f && delta <= beatDuration * 0.85)
        {
            return BeatQuality.Good; // Good
        }
        else if (delta >= beatDuration * 0.0f && delta <= beatDuration)
        {
            return BeatQuality.Poor; // Poor
        }
        else
        {
            return BeatQuality.Miss; // Miss
        }
    }

    public virtual void StopPlaying()
    {
        audioSource.time = 0.0f;
        audioSource.Stop();
    }
    public virtual void StartPlaying()
    {
        lastBeatTime = 0f;
        nextBeatTime = beatDuration;
        audioSource.time = 0.0f;
        audioSource.Play();
        playerObject.GetComponent<BeatSlimePlayer>().data.Reset();
    }
}
