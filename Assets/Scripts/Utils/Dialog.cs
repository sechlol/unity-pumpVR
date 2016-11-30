using UnityEngine;
using System;
using UnityEngine.UI;

public class Dialog : MonoBehaviour {

    private Action<int> _callbackBPM;
    [SerializeField] InputField Input;
    [SerializeField] Button SubmitBtn;
    [SerializeField] Text ErrorText;

    void Start() {

        SubmitBtn.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        string validation = Validate(Input.text);

        if(validation.Length != 0) 
            ErrorText.text = validation;
        else {
            _callbackBPM(int.Parse(Input.text));
            Hide();
        }
    }

    private string Validate(string text) {
        int res;
        if (int.TryParse(text, out res) && res > 0)
            return "";

        return "BPM must be a positive integer";
    }

    public void Show(Action<int> callback) {
        gameObject.SetActive(true);
        ErrorText.text = "";
        _callbackBPM = callback;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
