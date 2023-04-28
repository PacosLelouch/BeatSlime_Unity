using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using RhythmosEngine;

public class RhythmosBeatController : BeatController
{
    static public RhythmosBeatController GetRhythmosBeatControllerInScene(string name = "BeatController")
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            return go.GetComponent<RhythmosBeatController>();
        }
        return null;
    }

    public bool enableRhythmosDatabase = true;
    public TextAsset rhythmosFile;
    public string rhythmosName = "BeatSlime";
    protected RhythmosDatabase rhythmosDatabase;
    protected RhythmosPlayer rhythmosPlayer;
    protected float accumulateBeatAndRestTime = 0.0f;

    protected void Awake()
    {
        if (rhythmosFile != null && enableRhythmosDatabase)
        {
            rhythmosDatabase = new RhythmosDatabase(rhythmosFile);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (audioSource.isPlaying && enableRhythmosDatabase)
        {
            Note note = rhythmosPlayer.GetCurrentNote();
            // TODO
            //rhythmosPlayer.
            nextBeatTime = note.isRest ? note.duration : float.MaxValue;
        }
    }

    public override void StopPlaying()
    {
        if (enableRhythmosDatabase)
        {
            if (rhythmosPlayer != null && rhythmosPlayer.isPlaying)
            {
                rhythmosPlayer.Stop();
            }
        }
        else
        {
            base.StopPlaying();
        }
    }

    public override void StartPlaying()
    {
        if (enableRhythmosDatabase)
        {
            if (rhythmosPlayer != null && rhythmosPlayer.isPlaying)
            {
                rhythmosPlayer.Stop();
            }
            rhythmosPlayer = rhythmosDatabase.PlayRhythm(rhythmosName, volume: 0);
            Note note = rhythmosPlayer.GetCurrentNote();

            lastBeatTime = 0.0f;
            accumulateBeatAndRestTime = note.duration;
            nextBeatTime = note.isRest ? note.duration : float.MaxValue;
            playerObject.GetComponent<BeatSlimePlayer>().data.Reset();
        }
        else
        {
            base.StartPlaying();
        }
    }
}
