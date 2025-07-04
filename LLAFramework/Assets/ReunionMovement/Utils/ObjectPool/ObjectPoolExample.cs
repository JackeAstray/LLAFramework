using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LLAFramework.Example
{
    public class ObjectPoolExample : MonoBehaviour
    {
        public GameObject prefab;

        public List<GameObject> objects = new List<GameObject>();

        int count = 0;

        public float time = 0;
        public float timeMax = 4f;

        private void Start()
        {
            ObjectPoolMgr.Instance.RegisterSpawnPool("EnemyPool", prefab, OnSpawn, OnDespawn, 5);
            count = 0;
        }

        private void Update()
        {
            time += Time.deltaTime;

            if (time >= timeMax)
            {
                time = 0;

                GameObject go = ObjectPoolMgr.Instance.Spawn("EnemyPool");
                if (go != null)
                {
                    go.transform.position = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0);
                    go.name = "Enemy" + count;
                    count++;
                }
            }
        }

        public void OnSpawn(GameObject go)
        {
            go.SetActive(true);
        }

        public void OnDespawn(GameObject go)
        {
            go.SetActive(false);
        }
    }
}
