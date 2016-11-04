using System;

public enum MoveType { LEFT, RIGHT, BOTH, CONTINUOUS}

[Serializable]
public class Move {
    public MoveType type;
    public float posX;
    public float posY;
    public float time;
    public float timeEnd;
}
