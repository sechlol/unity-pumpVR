using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    [SerializeField] PoolableObject _prefab;
    [SerializeField] int _poolSize = 10;

    public PoolableObject ObjPrefab {
        get { return _prefab; }
        set { _prefab = value; }
    }

    public int Size {
        get { return _poolSize; }
        set { _poolSize = value; }
    }

    private Stack<PoolableObject> _available;
    private int _totalObjects = 0;

    void Awake() {

        _available = new Stack<PoolableObject>();

        if (_prefab != null)
            WarmPool(_prefab, _poolSize);
    }

    public void WarmPool(PoolableObject prefab, int size) {

        _poolSize = size;
        _prefab = prefab;

        for (int i = 0; i < size; i++) {
            AddClone(_prefab);
        }
    }

    public PoolableObject GetObject() {
        return GetObject(_prefab.transform.position, _prefab.transform.rotation);
    }

    public PoolableObject GetObject(Vector3 pos, Quaternion rot) {

        if (_available.Count <= 0)
            AddClone(_prefab);

        PoolableObject go = _available.Pop();

        go.transform.position = pos;
        go.transform.rotation = rot;
        go.transform.localScale = _prefab.transform.localScale;
        go.IsInUse = true;
        go.Refresh();
        go.gameObject.SetActive(true);

        return go;
    }

    public void ReturnObject(PoolableObject clone) {
        if (clone != null) {

            _available.Push(clone);

            clone.IsInUse = false;
            clone.transform.SetParent(transform, false);
            clone.gameObject.SetActive(false);
        }
    }

    private void AddClone(PoolableObject prefab) {

        //instantiate clone
        PoolableObject clone = GameObject.Instantiate(prefab) as PoolableObject;

        //initialize the clone and add it to available pool
        clone.Pool = this;
        clone.IsInUse = false;
        _available.Push(clone);
        _totalObjects++;

        //set initial name and parent
        //clone.name = prefab.name + "_" + _totalObjects;
        clone.transform.SetParent(transform, false);
        clone.gameObject.SetActive(false);
    }
}
