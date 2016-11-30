using UnityEngine;
using System.Collections.Generic;

public class GObjPool : MonoBehaviour {

    [SerializeField] int _capacity = 10;
    [SerializeField] GameObject _prefab;

    private LinkedList<GameObject> _free;
    private HashSet<GameObject> _inUse;

    private void Start() {
       _free = new LinkedList<GameObject>();
        _inUse = new HashSet<GameObject>();

        Warm(_capacity);
    }

    public GameObject Get() {
        if (_free.Count == 0)
            Warm(_capacity);
        GameObject go = _free.Last.Value;
        go.transform.SetParent(null);
        go.gameObject.SetActive(true);
        _free.RemoveLast();
        _inUse.Add(go);
        return go;
    }

    public void Return(GameObject go) {
        if (!_inUse.Contains(go))
            return;
        _inUse.Remove(go);
        go.transform.SetParent(transform, false);
        go.gameObject.SetActive(false);
        _free.AddLast(go);
    }

    private void Warm(int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject go = Instantiate<GameObject>(_prefab);
            go.transform.SetParent(transform, false);
            go.gameObject.SetActive(false);
            _free.AddLast(go);
        }
    }


	
}
