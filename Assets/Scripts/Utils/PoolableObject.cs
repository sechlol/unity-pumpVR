using UnityEngine;

public class PoolableObject : MonoBehaviour {

    public ObjectPool Pool { get; set; }
    public bool IsInUse { get; set; }
    [SerializeField]
    bool AutoDestroy;
    [SerializeField]
    float DestroyAfter = 0;

    virtual public void Refresh() {
        if (AutoDestroy)
            Invoke("ReturnToPool", DestroyAfter);
    }

    virtual public void ReturnToPool() {
        if (Pool != null)
            Pool.ReturnObject(this);
        else
            Destroy(gameObject);
    }
}
