using UnityEngine;
using UnityEngine.UI;

public class BrushSelector : MonoBehaviour {

    private Image _img;
    private Text _txt;
    private EditorState _state;

	void Start () {
        _img = GetComponent<Image>();
        _txt = GetComponentInChildren<Text>();
        _state = GetComponentInParent<EditorManager>().GetState();
        _state.OnBrushChange += ChangeColor;

        ChangeColor(_state.Brush);
	}
	
    private void ChangeColor(MoveType type) {
        _txt.text = type.ToString();

        switch (type) {
            case MoveType.RIGHT:
                _img.color = Color.red;
                break;
            case MoveType.LEFT:
                _img.color = Color.blue;
                break;
            case MoveType.HEAD:
                _img.color = Color.yellow;
                break;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
            _state.Brush = MoveType.LEFT;
        if (Input.GetKeyDown(KeyCode.W))
            _state.Brush = MoveType.HEAD;
        if (Input.GetKeyDown(KeyCode.E))
            _state.Brush = MoveType.RIGHT;
    }
}
