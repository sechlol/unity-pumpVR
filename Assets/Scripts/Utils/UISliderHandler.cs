using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISliderHandler : MonoBehaviour, IPointerDownHandler, IDragHandler {
    private Slider _slider;

    public Action<float> OnInteract;

    void Start() {
        _slider = GetComponent<Slider>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(OnInteract != null)
            OnInteract(_slider.value);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if (OnInteract != null)
            OnInteract(_slider.value);
    }
}
