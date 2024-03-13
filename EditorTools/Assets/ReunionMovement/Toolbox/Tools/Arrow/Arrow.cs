using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class Arrow : MonoBehaviour
    {
        public enum ArrowType 
        { 
            UI,
            World
        }

        public Transform Origin { get { return origin; } set { origin = value; } }

        [SerializeField] private float baseHeight;
        [SerializeField] private RectTransform baseRect;
        [SerializeField] private Transform origin;
        [SerializeField] private bool startsActive;
        [SerializeField] private ArrowType type;

        private RectTransform myRect;
        private Canvas canvas;
        private Camera mainCamera;
        private bool isActive;

        private void Awake()
        {
            myRect = (RectTransform)transform;
            canvas = GetComponentInParent<Canvas>();
            mainCamera = Camera.main;
            SetActive(startsActive);
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }
            Setup();
        }

        private void Setup()
        {
            if (type == ArrowType.UI)
            {
                SetupUI();
            }
            else
            {
                SetupWorld();
            }
        }

        public void SetupUI()
        {
            if (origin == null)
                return;
            Vector2 originPosOnScreen = origin.position;
            myRect.anchoredPosition = origin.GetComponent<RectTransform>().anchoredPosition;
            Vector2 differenceToMouse = Input.mousePosition - (Vector3)originPosOnScreen;
            differenceToMouse.Scale(new Vector2(1f / myRect.localScale.x, 1f / myRect.localScale.y));
            transform.up = differenceToMouse;
            baseRect.anchorMax = new Vector2(baseRect.anchorMax.x, differenceToMouse.magnitude / canvas.scaleFactor / baseHeight);
        }

        public void SetupWorld()
        {
            if (origin == null)
                return;
            Vector2 originPosOnScreen = mainCamera.WorldToScreenPoint(origin.position);
            myRect.anchoredPosition = new Vector2(originPosOnScreen.x - Screen.width / 2, originPosOnScreen.y - Screen.height / 2) / canvas.scaleFactor;
            Vector2 differenceToMouse = Input.mousePosition - (Vector3)originPosOnScreen;
            differenceToMouse.Scale(new Vector2(1f / myRect.localScale.x, 1f / myRect.localScale.y));
            transform.up = differenceToMouse;
            baseRect.anchorMax = new Vector2(baseRect.anchorMax.x, differenceToMouse.magnitude / canvas.scaleFactor / baseHeight);
        }

        private void SetActive(bool b)
        {
            isActive = b;
            if (b)
                Setup();
            baseRect.gameObject.SetActive(b);
        }

        public void Activate() => SetActive(true);
        public void Deactivate() => SetActive(false);
        public void SetupAndActivate(Transform origin)
        {
            Origin = origin;
            Activate();
        }
    }
}