using UnityEngine;
using System;
using System.Collections;

using LitJson.Extensions;

namespace LitJson
{

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    /// <summary>
    /// Unity内建类型拓展
    /// </summary>
    public static class UnityTypeBindings
    {

        static bool registerd;

        static UnityTypeBindings()
        {
            Register();
        }

        public static void Register()
        {

            if (registerd) return;
            registerd = true;


            // 注册Type类型的Exporter
            JsonMapper.RegisterExporter<Type>((v, w) =>
            {
                w.Write(v.FullName);
            });

            JsonMapper.RegisterImporter<string, Type>((s) =>
            {
                return Type.GetType(s);
            });

            // 注册Vector2类型的Exporter
            Action<Vector2, JsonWriter> writeVector2 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            };

            JsonMapper.RegisterExporter<Vector2>((v, w) =>
            {
                writeVector2(v, w);
            });

            // 注册Vector3类型的Exporter
            Action<Vector3, JsonWriter> writeVector3 = (v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            };

            JsonMapper.RegisterExporter<Vector3>((v, w) =>
            {
                writeVector3(v, w);
            });

            // 注册Vector4类型的Exporter
            JsonMapper.RegisterExporter<Vector4>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            // 注册Quaternion类型的Exporter
            JsonMapper.RegisterExporter<Quaternion>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });

            // 注册Color类型的Exporter
            JsonMapper.RegisterExporter<Color>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            // 注册Color32类型的Exporter
            JsonMapper.RegisterExporter<Color32>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });

            // 注册Bounds类型的Exporter
            JsonMapper.RegisterExporter<Bounds>((v, w) =>
            {
                w.WriteObjectStart();

                w.WritePropertyName("center");
                writeVector3(v.center, w);

                w.WritePropertyName("size");
                writeVector3(v.size, w);

                w.WriteObjectEnd();
            });

            // 注册Rect类型的Exporter
            JsonMapper.RegisterExporter<Rect>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("width", v.width);
                w.WriteProperty("height", v.height);
                w.WriteObjectEnd();
            });

            // 注册RectOffset类型的Exporter
            JsonMapper.RegisterExporter<RectOffset>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("top", v.top);
                w.WriteProperty("left", v.left);
                w.WriteProperty("bottom", v.bottom);
                w.WriteProperty("right", v.right);
                w.WriteObjectEnd();
            });

            // 注册Matrix4x4类型的Exporter
            JsonMapper.RegisterExporter<Matrix4x4>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("m00", v.m00);
                w.WriteProperty("m01", v.m01);
                w.WriteProperty("m02", v.m02);
                w.WriteProperty("m03", v.m03);
                w.WriteProperty("m10", v.m10);
                w.WriteProperty("m11", v.m11);
                w.WriteProperty("m12", v.m12);
                w.WriteProperty("m13", v.m13);
                w.WriteProperty("m20", v.m20);
                w.WriteProperty("m21", v.m21);
                w.WriteProperty("m22", v.m22);
                w.WriteProperty("m23", v.m23);
                w.WriteProperty("m30", v.m30);
                w.WriteProperty("m31", v.m31);
                w.WriteProperty("m32", v.m32);
                w.WriteProperty("m33", v.m33);
                w.WriteObjectEnd();
            });

            // 注册LayerMask类型的Exporter
            JsonMapper.RegisterExporter<LayerMask>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("value", v.value);
                w.WriteObjectEnd();
            });

            // 注册AnimationCurve类型的Exporter
            JsonMapper.RegisterExporter<AnimationCurve>((v, w) =>
            {
                w.WriteObjectStart();
                w.WritePropertyName("keys");
                w.WriteArrayStart();
                foreach (var key in v.keys)
                {
                    w.WriteObjectStart();
                    w.WriteProperty("time", key.time);
                    w.WriteProperty("value", key.value);
                    w.WriteProperty("inTangent", key.inTangent);
                    w.WriteProperty("outTangent", key.outTangent);
                    w.WriteObjectEnd();
                }
                w.WriteArrayEnd();
                w.WriteProperty("preWrapMode", v.preWrapMode.ToString());
                w.WriteProperty("postWrapMode", v.postWrapMode.ToString());
                w.WriteObjectEnd();
            });
        }
    }
}
