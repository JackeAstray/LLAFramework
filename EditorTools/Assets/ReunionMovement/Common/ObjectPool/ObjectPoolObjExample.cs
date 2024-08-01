using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolObjExample : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("OnDespawn", 2.5f);
    }

    public void OnDespawn()
    {
        ObjectPoolMgr.Instance.Despawn("EnemyPool", gameObject);
    }
}