using UnityEngine;

public class TrackEditor : MonoBehaviour {

    [SerializeField] int DefaultTicksPerBeat = 4;

    private Track _track;

    public Track NewTrack(string name, int bpm, float length) {
        int beats = DefaultTicksPerBeat * Mathf.CeilToInt(length / 60f * (float)bpm);
        return new Track() {
            songName = name,
            ticksPerBeat = DefaultTicksPerBeat,
            BPM = bpm,
            timeLength = length,
            moveList = new MoveGroup[beats]
        };
    }

}
