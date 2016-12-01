using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class UIWheelScrollerInput : MonoBehaviour {

    private bool _isMouseOver = false;
    private InputField _input;

    private void Start() {
        _input = GetComponent<InputField>();
    }

    public Action OnUpdate;

    void Update () {
        if (!_isMouseOver)
            return;

        float w = Input.GetAxis("Mouse ScrollWheel");
        if(w != 0) {
            //negative values not allowed
            int val = Mathf.Max(0, int.Parse(_input.text) + (w > 0 ? 1 : -1));
            _input.text = val.ToString();
            if (OnUpdate != null)
                OnUpdate();
        }  
	}

    public void OnMouseEnter(BaseEventData data) {
        _isMouseOver = true;
    }

    public void OnMouseExit(BaseEventData data) {
        _isMouseOver = false;
    }
}
