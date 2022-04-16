using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private GameObject prefab;
    private Stack<GameObject> objectPool = new Stack<GameObject>();

    public void SetObject(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public int GetLength()
    {
        return objectPool.Count;
    }

    public void Fill(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(prefab) as GameObject;
            obj.gameObject.SetActive(false);

            Push(obj);
        }
    }

    public GameObject Pop()
    {
        GameObject obj;
        if (objectPool.Count > 0)
            obj = objectPool.Pop();
        else
            obj = Object.Instantiate(prefab) as GameObject;

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Push(GameObject obj)
    {
        if (obj != null)
        {
            objectPool.Push(obj);
            obj.gameObject.SetActive(false);
            if (obj.transform.parent != null)
            {
                obj.transform.parent = null;
            }
        }

    }

    public bool IsObjectNull()
    {
        return prefab == null;
    }
}