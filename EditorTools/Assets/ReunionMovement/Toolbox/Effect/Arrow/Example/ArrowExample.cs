using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class ArrowExample : MonoBehaviour
    {
        [SerializeField] private Arrow arrow;
        public Arrow Arrow { get => arrow; set => arrow = value; }
        

        public void BeginAttack()
        {
            arrow.SetupAndActivate(transform);
            Log.Debug("BeginAttack");
        }

        public void EndAttack()
        {
            arrow.Deactivate();
            Log.Debug("EndAttack");
        }

        public void Deactivate()
        {
            arrow.Deactivate();
            Log.Debug("Deactivate");
        }
    }
}