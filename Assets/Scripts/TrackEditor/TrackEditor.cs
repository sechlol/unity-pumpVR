using UnityEngine;
using UnityEngine.EventSystems;

public class TrackEditor : MonoBehaviour {

    [SerializeField] int DefaultTicksPerBeat = 4;

    public MoveType Brush { get; set; }

    private Vector2 _rPosition;
    private Vector2 _rSize;

    void Start() {
        RectTransform r = GetComponent<RectTransform>();
        _rPosition = r.position;
        _rSize = new Vector2(r.rect.width / 2f, r.rect.height / 2f);
    }

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

    public void OnDown(BaseEventData evt) {
        Vector2 localClick = GetCoords(evt);
        Debug.Log("OnDown at " + localClick);
        evt.Use();
    }

    public void OnDrag(BaseEventData evt) {
        Vector2 localClick = GetCoords(evt);
        Debug.Log("OnDrag at " + localClick);
        evt.Use();
    }


    private Vector2 GetCoords(BaseEventData evt) {
        PointerEventData pData = (PointerEventData)evt;
        Vector2 coords = (pData.position - _rPosition);
        coords.x /= _rSize.x;
        coords.y /= _rSize.y;
        return coords;
    }

    public void Init(EditorState _status) {
    }
}
