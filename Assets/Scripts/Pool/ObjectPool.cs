using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    public GameObject Get(Vector3 position, Transform parent)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public void ClearPool()
    {
        foreach (var obj in pool)
        {
            Destroy(obj);
        }
        pool.Clear();
    }
}
