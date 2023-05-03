using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using RhythmosEngine;
using System;

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

    protected int nextNoteIndex = -1;
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
    }

    public int CurrentNoteIndex
    {
        get
        {
            return nextNoteIndex - 1;
        }
    }

    public override void ApplyBeat()
    {
        Note note = rhythmosPlayer.rhythm.GetNoteAt(CurrentNoteIndex % rhythmosPlayer.rhythm.NoteCount);
        int noteType = note.layoutIndex;
        
        //Color oldColor = slimeObject.GetComponent<SlimeEnemy>().slimeMesh.GetComponent<SkinnedMeshRenderer>().material.color;
        slimeObject.GetComponent<SlimeEnemy>().slimeMesh.GetComponent<SkinnedMeshRenderer>().material.color = gameManager.noteTypeToColor[noteType];
        //Color newColor = slimeObject.GetComponent<SlimeEnemy>().slimeMesh.GetComponent<SkinnedMeshRenderer>().material.color;
        base.ApplyBeat();
    }

    protected override void AccumulateNextBeatTime()
    {
        if (enableRhythmosDatabase && rhythmosPlayer != null && rhythmosPlayer.rhythm.NoteList() != null)
        {
            lastBeatTime = nextBeatTime;
            // Warning: What if all rest?

            Note note = new Note();
            do
            {
                note = rhythmosPlayer.rhythm.GetNoteAt(nextNoteIndex % rhythmosPlayer.rhythm.NoteCount);
                float noteDuration = 60f / rhythmosPlayer.rhythm.BPM * note.duration;
                nextBeatTime += noteDuration;
                nextNoteIndex = (nextNoteIndex + 1) % rhythmosPlayer.rhythm.NoteCount;
            } while (note.isRest);
        }
        else
        {
            base.AccumulateNextBeatTime();
        }
    }

    public override bool MatchNoteType(int inNoteType)
    {
        if (enableRhythmosDatabase)
        {
            Note note = rhythmosPlayer.rhythm.GetNoteAt(nextNoteIndex % rhythmosPlayer.rhythm.NoteCount);
            int noteType = note.layoutIndex;
            return inNoteType == noteType;
        }
        else
        {
            return base.MatchNoteType(inNoteType);
        }
    }

    public override void StopPlaying()
    {
        if (enableRhythmosDatabase)
        {
            if (rhythmosPlayer != null && rhythmosPlayer.isPlaying)
            {
                rhythmosPlayer.Stop();
                audioSource.time = 0.0f;
                audioSource.Stop();
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
            rhythmosPlayer = rhythmosDatabase.PlayRhythm(rhythmosName, volume: 0, false, false);

            bool ifAllRest = true;
            for (int i = 0; i < rhythmosPlayer.rhythm.NoteCount; ++i)
            {
                if (!rhythmosPlayer.rhythm.GetNoteAt(i).isRest)
                {
                    ifAllRest = false;
                    break;
                }
            }

            if (!ifAllRest)
            {
                nextNoteIndex = 0;
                bpm = rhythmosPlayer.rhythm.BPM;

                nextBeatTime = 0.0f;
                AccumulateNextBeatTime();
                audioSource.Play();
                playerObject.GetComponent<BeatSlimePlayer>().data.Reset();
            }
            else
            {
                Debug.Log("Error: Rhythm note all rest!");
            }
        }
        else
        {
            base.StartPlaying();
        }
    }
}
