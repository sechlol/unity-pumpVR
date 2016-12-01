using System;
using UnityEngine;
using UnityEngine.UI;

public class MoveTile : MonoBehaviour {

    [SerializeField] float SizeRelToParent = 0.05f;
    [SerializeField] int BeatsAdvance = 30;
    [SerializeField] int BeatsAfter = 10;

    private Image _img;
    private RectTransform _parent;
    private PoolableObject _obj;
    private CanvasGroup _cg;
    private Outline _line;

    private EditorState _state;
    private Move _move;
    private int _beat;
    private int _index;

    private void Awake() {
        _img = GetComponent<Image>();
        _img.rectTransform.localPosition = Vector3.zero;
        _cg = GetComponent<CanvasGroup>();
        _line = GetComponent<Outline>();
        _obj = GetComponent<PoolableObject>();
        _line.enabled = false;
    }

    public void AssignMove(EditorState state, int beat, int moveIndex) {

        if(_parent == null)
            _parent = transform.parent.GetComponent<RectTransform>();

        _state = state;
        _beat = beat;
        _index = moveIndex;
        _move = _state.track.moveList[beat].Group[moveIndex];

        _img.rectTransform.anchoredPosition = new Vector2(_parent.rect.width/2f * _move.x, _parent.rect.height /2f * _move.y);
        _img.rectTransform.sizeDelta = new Vector2(_parent.rect.width * SizeRelToParent, _parent.rect.height * SizeRelToParent);
        _img.transform.localScale = _state.beat <= _beat ? Vector3.one * FadeInProgress() : Vector3.one;
        _img.color = ColorByType(_move.t);
        _line.enabled = false;

        Refresh();
    }

    private Color ColorByType(MoveType t) {
        switch (t) {
            case MoveType.LEFT:
                return Color.red;
            case MoveType.RIGHT:
                return Color.blue;
            case MoveType.HEAD:
                return Color.yellow;
            default:
                return Color.black;
        }
    }

    public void Refresh() {

        //show tile growing
        if (_state.beat < _beat ) {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * FadeInProgress(), 0.75f);
            _cg.alpha = FadeInProgress();
            _line.enabled = false;
        }

        //show full tile
        else if(_state.beat == _beat) {
            transform.localScale = Vector2.one;
            _cg.alpha = 1;
            _line.enabled = true;
        }

        //show tile disappearing
        else {
            _cg.alpha = FadeOutProgress();
            _line.enabled = false;
        }
    }

    public void Delete() {
        _obj.ReturnToPool();
    }

    private float FadeInProgress() {
        if (_beat == _state.beat)
            return 1;
        return (1-(float)(_beat - _state.beat) / (float)BeatsAdvance)*0.5f;
    }

    private float FadeOutProgress() {
        return 1-(float)(_state.beat - _beat) / (float)BeatsAfter;
    }

    

}
