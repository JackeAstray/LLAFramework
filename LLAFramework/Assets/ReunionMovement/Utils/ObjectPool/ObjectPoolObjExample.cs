using LLAFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.Example
{
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
}