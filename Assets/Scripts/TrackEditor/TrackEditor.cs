using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackEditor : MonoBehaviour {

    [SerializeField] int TicksPerBeat = 4;
    [SerializeField] float OverlapBoundSize = 0.05f;

    private EditorState _state;
    private Vector2 _rPosition;
    private Vector2 _rSize;

    void Start() {
        RectTransform r = GetComponent<RectTransform>();
        _rPosition = r.position;
        _rSize = new Vector2(r.rect.width / 2f, r.rect.height / 2f);
    }

    public void Init(EditorState state) {
        _state = state;
    }

    public Track NewTrack(string name, int bpm, float length) {
        int beats = TicksPerBeat * Mathf.CeilToInt(length / 60f * (float)bpm);
        return new Track() {
            songName = name,
            ticksPerBeat = TicksPerBeat,
            BPM = bpm,
            timeLength = length,
            moveList = new List<MoveGroup>(new MoveGroup[beats])
        };
    }

    public void OnDown(BaseEventData evt) {
        Vector2 localClick = GetCoords(evt);
        
        //Left Click: add move
        if (Input.GetMouseButton(0)) {
            _state.AddMove(localClick, _state.Brush);
        }
        //Right Click: delete move
        else if (Input.GetMouseButton(1)) {
            int index = GetOverlappingMove(localClick);
            if (index >= 0)
                _state.DeleteMove(index);
        }
    }

    public void OnDrag(BaseEventData evt) {
        Vector2 localClick = GetCoords(evt);
        
        //Left Click: add move if not overlapping
        if (Input.GetMouseButton(0) && GetOverlappingMove(localClick) == -1) {
            _state.AddMove(localClick, _state.Brush);
        }
    }

    private int GetOverlappingMove(Vector2 point) {
        if (_state.CurrentMoves != null) {
            for (int i = 0; i < _state.CurrentMoves.Count; i++) {
                float xDiff = Mathf.Abs(_state.CurrentMoves[i].x - point.x);
                float yDiff = Mathf.Abs(_state.CurrentMoves[i].y - point.y);

                if (xDiff <= OverlapBoundSize && yDiff <= OverlapBoundSize)
                    return i;
            }
        }
        return -1;
    }

    private Vector2 GetCoords(BaseEventData evt) {
        PointerEventData pData = (PointerEventData)evt;
        Vector2 coords = (pData.position - _rPosition);
        coords.x /= _rSize.x;
        coords.y /= _rSize.y;
        return coords;
    }

    
}
