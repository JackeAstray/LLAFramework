using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameLogic
{
    /// <summary>
    /// 碰撞器绘制
    /// </summary>
    public class ColliderGizmo : MonoBehaviour
    {
#if UNITY_EDITOR
        public Presets Preset;

        public Color customWireColor;
        public Color customFillColor;
        public Color customCenterColor;

        public float alpha = 1.0f;
        public Color wireColor = new Color(.6f, .6f, 1f, .5f);
        public Color fillColor = new Color(.6f, .7f, 1f, .1f);
        public Color centerColor = new Color(.6f, .7f, 1f, .7f);

        public bool drawFill = true;
        public bool drawWire = true;
        private bool needDrawWire;
        private bool needDrawFill;
        public bool drawCenter;

        /// <summary>
        /// 碰撞体中心标记的半径
        /// </summary>
        public float centerMarkerRadius = 1.0f;

        public bool includeChildColliders;

#if UNITY_AI_ENABLED
        //导航网格
		private NavMeshObstacle edgeColliders2D;
#endif

#if UNITY_PHYSICS2D_ENABLED
        //边缘碰撞器-2D物理
		private List<EdgeCollider2D> edgeColliders2D;
		//盒子碰撞器-2D物理
        private List<BoxCollider2D> boxColliders2D;
		//圆形碰撞器-2D物理
        private List<CircleCollider2D> circleColliders2D;
#endif

#if UNITY_PHYSICS_ENABLED
        //盒子碰撞器-3D物理
        private List<BoxCollider> boxColliders;
        //球碰撞器-3D物理
        private List<SphereCollider> sphereColliders;
        //球碰撞器-3D物理
		private List<MeshCollider> meshColliders;
#endif

        private readonly HashSet<Transform> withColliders = new HashSet<Transform>();

        private Color wireGizmoColor;
        private Color fillGizmoColor;
        private Color centerGizmoColor;

        private bool initialized;


        private void OnDrawGizmos()
        {
            if (!enabled) return;
            if (!initialized) Refresh();

            DrawColliders();
        }

        #region Refresh

        public void Refresh()
        {
            initialized = true;

            wireGizmoColor.r = wireColor.r;
            wireGizmoColor.g = wireColor.g;
            wireGizmoColor.b = wireColor.b;
            wireGizmoColor.a = wireColor.a * alpha;

            fillGizmoColor.r = fillColor.r;
            fillGizmoColor.g = fillColor.g;
            fillGizmoColor.b = fillColor.b;
            fillGizmoColor.a = fillColor.a * alpha;

            centerGizmoColor.r = centerColor.r;
            centerGizmoColor.g = centerColor.g;
            centerGizmoColor.b = centerColor.b;
            centerGizmoColor.a = centerColor.a * alpha;

            needDrawWire = drawWire;
            needDrawFill = drawFill;

            withColliders.Clear();

#if UNITY_AI_ENABLED
			edgeColliders2D = gameObject.GetComponent<NavMeshObstacle>();
#endif

#if UNITY_PHYSICS2D_ENABLED
			if (edgeColliders2D != null) edgeColliders2D.Clear();
			if (boxColliders2D != null) boxColliders2D.Clear();
			if (circleColliders2D != null) circleColliders2D.Clear();

			Collider2D[] colliders2d = includeChildColliders ? gameObject.GetComponentsInChildren<Collider2D>() : gameObject.GetComponents<Collider2D>();

			for (var i = 0; i < colliders2d.Length; i++)
			{
				var c = colliders2d[i];

				var box2d = c as BoxCollider2D;
				if (box2d != null)
				{
					if (boxColliders2D == null) boxColliders2D = new List<BoxCollider2D>();
					boxColliders2D.Add(box2d);
					withColliders.Add(box2d.transform);
					continue;
				}

				var edge = c as EdgeCollider2D;
				if (edge != null)
				{
					if (edgeColliders2D == null) edgeColliders2D = new List<EdgeCollider2D>();
					edgeColliders2D.Add(edge);
					withColliders.Add(edge.transform);
					continue;
				}

				var circle2d = c as CircleCollider2D;
				if (circle2d != null)
				{
					if (circleColliders2D == null) circleColliders2D = new List<CircleCollider2D>();
					circleColliders2D.Add(circle2d);
					withColliders.Add(circle2d.transform);
				}
			}
#endif

#if UNITY_PHYSICS_ENABLED
            if (boxColliders != null) boxColliders.Clear();
			if (sphereColliders != null) sphereColliders.Clear();
			if (meshColliders != null) meshColliders.Clear();

			Collider[] colliders = includeChildColliders ? gameObject.GetComponentsInChildren<Collider>() : gameObject.GetComponents<Collider>();

			for (var i = 0; i < colliders.Length; i++)
			{
				var c = colliders[i];

				var box = c as BoxCollider;
				if (box != null)
				{
					if (boxColliders == null) boxColliders = new List<BoxCollider>();
					boxColliders.Add(box);
					withColliders.Add(box.transform);
					continue;
				}

				var sphere = c as SphereCollider;
				if (sphere != null)
				{
					if (sphereColliders == null) sphereColliders = new List<SphereCollider>();
					sphereColliders.Add(sphere);
					withColliders.Add(sphere.transform);
				}

				var mesh = c as MeshCollider;
				if (mesh != null)
				{
					if (meshColliders == null) meshColliders = new List<MeshCollider>();
					meshColliders.Add(mesh);
					withColliders.Add(mesh.transform);
				}
			}
#endif
        }

        #endregion


        #region Drawers

#if UNITY_PHYSICS2D_ENABLED

		private void DrawEdgeCollider2D(EdgeCollider2D coll)
		{
			var target = coll.transform;
			var lossyScale = target.lossyScale;
			var position = target.position;

			Gizmos.color = wireColor;
			Vector3 previous = Vector2.zero;
			bool first = true;
			for (int i = 0; i < coll.points.Length; i++)
			{
				var collPoint = coll.points[i];
				Vector3 pos = new Vector3(collPoint.x * lossyScale.x, collPoint.y * lossyScale.y, 0);
				Vector3 rotated = target.rotation * pos;

				if (first) first = false;
				else
				{
					Gizmos.color = wireGizmoColor;
					Gizmos.DrawLine(position + previous, position + rotated);
				}

				previous = rotated;

				DrawColliderGizmo(target.position + rotated, .05f);
			}
		}

		private void DrawBoxCollider2D(BoxCollider2D coll)
		{
			var target = coll.transform;
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			DrawColliderGizmo(coll.offset, coll.size);
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void DrawCircleCollider2D(CircleCollider2D coll)
		{
			var target = coll.transform;
			var offset = coll.offset;
			var scale = target.lossyScale;
			DrawColliderGizmo(target.position + new Vector3(offset.x, offset.y, 0.0f), coll.radius * Mathf.Max(scale.x, scale.y));
		}

#endif

#if UNITY_PHYSICS_ENABLED

		private void DrawBoxCollider(BoxCollider coll)
		{
			var target = coll.transform;
			Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
			DrawColliderGizmo(coll.center, coll.size);
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void DrawSphereCollider(SphereCollider coll)
		{
			var target = coll.transform;
			var scale = target.lossyScale;
			var center = coll.center;
			var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
			DrawColliderGizmo(target.position + new Vector3(center.x, center.y, 0.0f), coll.radius * max);
		}

		private void DrawMeshCollider(MeshCollider coll)
		{
			var target = coll.transform;

			if (drawWire)
			{
				Gizmos.color = wireGizmoColor;
				Gizmos.DrawWireMesh(coll.sharedMesh, target.position, target.rotation, target.localScale * 1.01f);
			}

			if (drawFill)
			{
				Gizmos.color = fillGizmoColor;
				Gizmos.DrawMesh(coll.sharedMesh, target.position, target.rotation, target.localScale * 1.01f);
			}
		}

#endif

#if UNITY_AI_ENABLED

		private void DrawNavMeshObstacle(NavMeshObstacle obstacle)
		{
			var target = obstacle.transform;

			if (obstacle.shape == NavMeshObstacleShape.Box)
			{
				Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, target.lossyScale);
				DrawColliderGizmo(obstacle.center, obstacle.size);
				Gizmos.matrix = Matrix4x4.identity;
			}
			else
			{
				var scale = target.lossyScale;
				var center = obstacle.center;
				var max = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)); // to not use Mathf.Max version with params[]
				DrawColliderGizmo(target.position + new Vector3(center.x, center.y, 0.0f), obstacle.radius * max);
			}
		}

#endif
        private void DrawColliders()
        {
            if (drawCenter)
            {
                Gizmos.color = centerGizmoColor;
                foreach (var withCollider in withColliders)
                {
                    Gizmos.DrawSphere(withCollider.position, centerMarkerRadius);
                }
            }

            if (!needDrawWire && !needDrawFill) return;

#if UNITY_AI_ENABLED
			if (edgeColliders2D != null) DrawNavMeshObstacle(edgeColliders2D);
#endif

#if UNITY_PHYSICS2D_ENABLED
			if (edgeColliders2D != null)
			{
				foreach (var edge in edgeColliders2D)
				{
					if (edge == null) continue;
					DrawEdgeCollider2D(edge);
				}
			}

			if (boxColliders2D != null)
			{
				foreach (var box in boxColliders2D)
				{
					if (box == null) continue;
					DrawBoxCollider2D(box);
				}
			}

			if (circleColliders2D != null)
			{
				foreach (var circle in circleColliders2D)
				{
					if (circle == null) continue;
					DrawCircleCollider2D(circle);
				}
			}
#endif

#if UNITY_PHYSICS_ENABLED
            if (boxColliders != null)
			{
				foreach (var box in boxColliders)
				{
					if (box == null) continue;
					DrawBoxCollider(box);
				}
			}

			if (sphereColliders != null)
			{
				foreach (var sphere in sphereColliders)
				{
					if (sphere == null) continue;
					DrawSphereCollider(sphere);
				}
			}

			if (meshColliders != null)
			{
				foreach (var mesh in meshColliders)
				{
					if (mesh == null) continue;
					DrawMeshCollider(mesh);
				}
			}
#endif
        }


        private void DrawColliderGizmo(Vector3 position, Vector3 size)
        {
            if (drawWire)
            {
                Gizmos.color = wireGizmoColor;
                Gizmos.DrawWireCube(position, size);
            }

            if (drawFill)
            {
                Gizmos.color = fillGizmoColor;
                Gizmos.DrawCube(position, size);
            }
        }

        private void DrawColliderGizmo(Vector3 position, float radius)
        {
            if (drawWire)
            {
                Gizmos.color = wireGizmoColor;
                Gizmos.DrawWireSphere(position, radius);
            }

            if (drawFill)
            {
                Gizmos.color = fillGizmoColor;
                Gizmos.DrawSphere(position, radius);
            }
        }

        #endregion


        #region Change Preset

        public enum Presets
        {
            Custom,
            Red,
            Blue,
            Green,
            Purple,
            Yellow,
            Aqua,
            White,
            Lilac,
            DirtySand
        }

        public void ChangePreset(Presets preset)
        {
            Preset = preset;

            switch (Preset)
            {
                case Presets.Red:
                    wireColor = new Color32(143, 0, 21, 202);
                    fillColor = new Color32(218, 0, 0, 37);
                    centerColor = new Color32(135, 36, 36, 172);
                    break;

                case Presets.Blue:
                    wireColor = new Color32(0, 116, 214, 202);
                    fillColor = new Color32(0, 110, 218, 37);
                    centerColor = new Color32(57, 160, 221, 172);
                    break;

                case Presets.Green:
                    wireColor = new Color32(153, 255, 187, 128);
                    fillColor = new Color32(153, 255, 187, 62);
                    centerColor = new Color32(153, 255, 187, 172);
                    break;

                case Presets.Purple:
                    wireColor = new Color32(138, 138, 234, 128);
                    fillColor = new Color32(173, 178, 255, 26);
                    centerColor = new Color32(153, 178, 255, 172);
                    break;

                case Presets.Yellow:
                    wireColor = new Color32(255, 231, 35, 128);
                    fillColor = new Color32(255, 252, 153, 100);
                    centerColor = new Color32(255, 242, 84, 172);
                    break;

                case Presets.DirtySand:
                    wireColor = new Color32(255, 170, 0, 60);
                    fillColor = new Color32(180, 160, 80, 175);
                    centerColor = new Color32(255, 242, 84, 172);
                    break;

                case Presets.Aqua:
                    wireColor = new Color32(255, 255, 255, 120);
                    fillColor = new Color32(0, 230, 255, 140);
                    centerColor = new Color32(255, 255, 255, 120);
                    break;

                case Presets.White:
                    wireColor = new Color32(255, 255, 255, 130);
                    fillColor = new Color32(255, 255, 255, 130);
                    centerColor = new Color32(255, 255, 255, 130);
                    break;

                case Presets.Lilac:
                    wireColor = new Color32(255, 255, 255, 255);
                    fillColor = new Color32(160, 190, 255, 140);
                    centerColor = new Color32(255, 255, 255, 130);
                    break;


                case Presets.Custom:
                    wireColor = customWireColor;
                    fillColor = customFillColor;
                    centerColor = customCenterColor;
                    break;
            }

            Refresh();
        }

        #endregion

#endif
    }
}


#if UNITY_EDITOR

namespace GameLogic.Internal
{
    [CustomEditor(typeof(ColliderGizmo)), CanEditMultipleObjects]
    public class ColliderGizmoEditor : Editor
    {
        private SerializedProperty enabledProperty;
        private SerializedProperty alphaProperty;
        private SerializedProperty drawWireProperty;
        private SerializedProperty wireColorProperty;
        private SerializedProperty drawFillProperty;
        private SerializedProperty fillColorProperty;
        private SerializedProperty drawCenterProperty;
        private SerializedProperty centerColorProperty;
        private SerializedProperty centerRadiusProperty;

        private SerializedProperty includeChilds;

        private new ColliderGizmo target;

        private int collidersCount;

        private void OnEnable()
        {
            target = base.target as ColliderGizmo;

            enabledProperty = serializedObject.FindProperty("m_Enabled");
            alphaProperty = serializedObject.FindProperty("alpha");

            drawWireProperty = serializedObject.FindProperty("drawWire");
            wireColorProperty = serializedObject.FindProperty("wireColor");

            drawFillProperty = serializedObject.FindProperty("drawFill");
            fillColorProperty = serializedObject.FindProperty("fillColor");

            drawCenterProperty = serializedObject.FindProperty("drawCenter");
            centerColorProperty = serializedObject.FindProperty("centerColor");
            centerRadiusProperty = serializedObject.FindProperty("centerMarkerRadius");

            includeChilds = serializedObject.FindProperty("includeChildColliders");

            collidersCount = CollidersCount();
        }


        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "CG_State");

            EditorGUILayout.PropertyField(enabledProperty);

            EditorGUI.BeginChangeCheck();
            target.Preset = (ColliderGizmo.Presets)EditorGUILayout.EnumPopup("Color Preset", target.Preset);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var singleTarget in targets)
                {
                    var gizmo = (ColliderGizmo)singleTarget;
                    gizmo.ChangePreset(target.Preset);
                    EditorUtility.SetDirty(gizmo);
                }
            }

            alphaProperty.floatValue = EditorGUILayout.Slider("Overall Transparency", alphaProperty.floatValue, 0, 1);


            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(drawWireProperty);
                if (drawWireProperty.boolValue) EditorGUILayout.PropertyField(wireColorProperty, new GUIContent(""));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(drawFillProperty);
                if (drawFillProperty.boolValue) EditorGUILayout.PropertyField(fillColorProperty, new GUIContent(""));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(drawCenterProperty);
                if (drawCenterProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(centerColorProperty, GUIContent.none);
                    EditorGUILayout.PropertyField(centerRadiusProperty);
                }
            }


            if (EditorGUI.EndChangeCheck())
            {
                var presetProp = serializedObject.FindProperty("Preset");
                var customWireColor = serializedObject.FindProperty("customWireColor");
                var customFillColor = serializedObject.FindProperty("customFillColor");
                var customCenterColor = serializedObject.FindProperty("customCenterColor");

                presetProp.enumValueIndex = (int)ColliderGizmo.Presets.Custom;
                customWireColor.colorValue = wireColorProperty.colorValue;
                customFillColor.colorValue = fillColorProperty.colorValue;
                customCenterColor.colorValue = centerColorProperty.colorValue;
            }

            EditorGUILayout.PropertyField(includeChilds);

            int collidersCountCheck = CollidersCount();
            bool collidersCountChanged = collidersCountCheck != collidersCount;
            collidersCount = collidersCountCheck;

            if (GUI.changed || collidersCountChanged)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);

                target.Refresh();
            }
        }

        private int CollidersCount()
        {
            int result = 0;

            if (includeChilds.boolValue)
            {
#if UNITY_PHYSICS_ENABLED
				result += target.gameObject.GetComponentsInChildren<Collider>().Length;
#endif
#if UNITY_PHYSICS2D_ENABLED
				result += _target.gameObject.GetComponentsInChildren<Collider2D>().Length;
#endif
                return result;
            }

#if UNITY_PHYSICS_ENABLED
			result += target.gameObject.GetComponents<Collider>().Length;
#endif
#if UNITY_PHYSICS2D_ENABLED
			result += _target.gameObject.GetComponents<Collider2D>().Length;
#endif
            return result;
        }
    }
}

#endif