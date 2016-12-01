using System.Collections.Generic;
using UnityEngine;

public class TrackVisualizer : MonoBehaviour {

    [SerializeField] int BeatsAdvance = 30;
    [SerializeField] int BeatsAfter = 10;

    private EditorState _state;
    private int _lastBeatUpdate = -1;
    private HashSet<Move> _prevShown;
    private HashSet<Move> _tempMoves;
    private Dictionary<Move, MoveTile> _tiles;
    private ObjectPool _pool;

    public void Init(EditorState state) {
        _state = state;
        _tiles = new Dictionary<Move, MoveTile>();
        _pool = GetComponent<ObjectPool>();
        _prevShown = new HashSet<Move>();
        _tempMoves = new HashSet<Move>();

        _state.OnTrackEdit += (int groupIndex) => UpdateView();
    }

    void Update() {
        if (_state == null || _state.beat == _lastBeatUpdate)
            return;

        UpdateView();
        _lastBeatUpdate = _state.beat;
    }

    public void UpdateView() {

        _tempMoves.Clear();

        //get start index for inspecting the moves list
        int index = Mathf.Max(_state.beat - BeatsAfter, 0);

        //never exceed list size
        int count = Mathf.Min(_state.beat + BeatsAdvance, _state.track.moveList.Count) - index;

        List<MoveGroup> groups = _state.track.moveList;
        List<Move> list;
        Move m;

        for (int i= index; i<index+count-1; i++) {
            list = groups[i].Group;
            for(int j=0; j<list.Count; j++) {
                m = list[j];
                _tempMoves.Add(m);

                //Move is already shown, just update it
                if (_tiles.ContainsKey(m)) {
                    _tiles[m].Refresh();
                }

                //Move is not shown, add it
                else {
                    MoveTile tile = _pool.GetObject().GetComponent<MoveTile>();
                    tile.AssignMove(_state, i, j);
                    _tiles.Add(m, tile);
                }
            }
        }

        _prevShown.ExceptWith(_tempMoves);
        foreach(Move move in _prevShown) {
            MoveTile tile = _tiles[move];
            _tiles.Remove(move);
            tile.Delete();
        }
        _prevShown = new HashSet<Move>(_tempMoves);
    }
}
