using System;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { LEFT, RIGHT, BOTH, CONTINUOUS }

[Serializable]
public class Track {
    public int BPM;
    public int ticksPerBeat;
    public float timeLength;
    public String songName;
    public List<MoveGroup> moveList;
    
    public int TimeToBeat(float time) {
        float progress = time / timeLength;
        return Mathf.CeilToInt(moveList.Count * progress);
    }

    public float BeatToTime(int beat) {
        float progress = (float)beat / (float)moveList.Count;
        return timeLength * progress;
    }
    
}

[Serializable]
public class MoveGroup {
    public List<Move> Group;
}
 
[Serializable]
public class Move {
    public MoveType t;
    public float x;
    public float y;
}
