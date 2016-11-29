using System;
using UnityEngine;

public enum MoveType { LEFT, RIGHT, BOTH, CONTINUOUS }

[Serializable]
public class Track {
    public int BPM;
    public int ticksPerBeat;
    public float timeLength;
    public String songName;
    public MoveGroup[] moveList;
    
    public int TimeToBeat(float time) {
        float progress = time / timeLength;
        return Mathf.CeilToInt(moveList.Length * progress);
    }

    public float BeatToTime(int beat) {
        float progress = (float)beat / (float)moveList.Length;
        return timeLength * progress;
    }
}

[Serializable]
public class MoveGroup {
    Move[] Group;
}

[Serializable]
public class Move {
    public MoveType type;
    public float posX;
    public float posY;
}
