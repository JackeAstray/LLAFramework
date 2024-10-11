using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Example
{
    public class CSGUsageExample : MonoBehaviour
    {
        public GameObject lhs;

        public GameObject rhs;

        public CSG.BooleanOp Operation;

        public Material material;

        /// <summary>
        /// 执行
        /// </summary>
        public void Perform()
        {
            Model result = CSG.Perform(Operation, lhs, rhs);
            var composite = new GameObject();
            composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
            result.Materials.Add(material);
            composite.AddComponent<MeshRenderer>().sharedMaterials = result.Materials.ToArray();
            composite.name = Operation.ToString() + " Object";
        }
    }
}