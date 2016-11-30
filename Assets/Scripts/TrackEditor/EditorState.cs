using System.Collections.Generic;
using UnityEngine;

public delegate void TrackLoad(Track newTrack);
public delegate void TrackEdit(int groupIndex);

public class EditorState {

    private int _b;
    private float _t;
    private Track _track;

    public event TrackLoad OnTrackLoad;
    public event TrackEdit OnTrackEdit;

    public List<Move> CurrentMoves {
        get { return _track.moveList[_b].Group; }
        set { _track.moveList[_b].Group = value; }
    }

    public Track track {
        get { return _track; }
        set {
            _t = _b = 0;
            _track = value;
            
            if (OnTrackLoad != null)
                OnTrackLoad(_track);
        }
    }

    public int beat {
        get { return _b; }
        set {
            _b = value;
            _t = track.BeatToTime(_b);
        }
    }

    public float time {
        get { return _t; }
        set {
            _t = value;
            _b = track.TimeToBeat(_t);
        }
    }

    public void AddMove(Vector2 coord, MoveType type) {
        if (CurrentMoves == null)
            CurrentMoves = new List<Move>();

        CurrentMoves.Add(new Move() { x = coord.x, y = coord.y, t = type });
        NotifyChange(_b);
    }

    public void DeleteMove(int index) {
        CurrentMoves.RemoveAt(index);
        NotifyChange(_b);
    }

    public void NotifyChange(int atBeat) {
        if (OnTrackEdit != null)
            OnTrackEdit(atBeat);
    }
}
