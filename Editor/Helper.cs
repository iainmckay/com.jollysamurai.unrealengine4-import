using System;
using System.IO;
using JollySamurai.UnrealEngine4.T3D;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;
using BlendMode = JollySamurai.UnrealEngine4.T3D.BlendMode;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;
using Vector3 = UnityEngine.Vector3;
using Vector4 = JollySamurai.UnrealEngine4.T3D.Vector4;

namespace JollySamurai.UnrealEngine4.Import
{
    public class Helper
    {
        public static SurfaceType BlendModeToSurfaceType(BlendMode blendMode)
        {
            if(blendMode == BlendMode.Translucent) {
                return SurfaceType.Transparent;
            }

            return SurfaceType.Opaque;
        }

        public static AlphaMode BlendModeToAlphaMode(BlendMode blendMode)
        {
            switch (blendMode) {
                case BlendMode.Additive:
                    return AlphaMode.Additive;
                case BlendMode.Modulate:
                    return AlphaMode.Multiply;
                case BlendMode.Masked:
                    Debug.Log("FIXME: masked blend mode not supported");

                    break;
            }

            return AlphaMode.Alpha;
        }

        public static PBRMasterNode.Model MaterialToModel(Material material)
        {
            if(material.Metallic != null) {
                return PBRMasterNode.Model.Metallic;
            } else if(material.Specular != null) {
                return PBRMasterNode.Model.Specular;
            }

            return PBRMasterNode.Model.Metallic;
        }

        public static Texture2D LoadTexture(string unrealFileName)
        {
            // FIXME: hard coded TGA extension and directly loading assets, there must be a quicker way to do this
            var textureFileName = "Assets" + Path.ChangeExtension(unrealFileName, "TGA");

           return AssetDatabase.LoadAssetAtPath<Texture2D>(textureFileName);
        }

        public static string GetMeshPath(string unrealFileName)
        {
            return "Assets" + Path.ChangeExtension(unrealFileName, "FBX");
        }

        public static Mesh LoadMesh(string unrealFileName)
        {
            // FIXME: hard coded FBX extension and directly loading assets, there must be a quicker way to do this
            return AssetDatabase.LoadAssetAtPath<Mesh>(GetMeshPath(unrealFileName));
        }

        public static string GetMaterialPath(string unrealFileName)
        {
            return "Assets" + Path.ChangeExtension(unrealFileName, "mat");
        }

        public static UnityEngine.Material LoadMaterial(string unrealFileName)
        {
            // FIXME: hard coded material extension and directly loading assets, there must be a quicker way to do this
            return AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(GetMaterialPath(unrealFileName));
        }

        public static Vector3 ConvertUnrealVector3(T3D.Vector3 v, bool applyScaling = false)
        {
            var x = v.X;
            var y = v.Y;
            var z = v.Z;

            if(applyScaling) {
                x = ScaleUnit(x);
                y = ScaleUnit(y);
                z = ScaleUnit(z);
            }

            return new Vector3(y, z, x);
        }

        public static Quaternion ConvertUnrealRotator(Rotator rotation)
        {
            var q =  Quaternion.Euler(rotation.Pitch, rotation.Yaw, rotation.Roll);
            q = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y - 180f, q.eulerAngles.z);

            return q;
        }

        public static float ScaleUnit(float value)
        {
            return value / 100.0f;
        }

        public static Color ConvertUnrealColor(Vector4 value)
        {
            return new Color(value.X / 255f, value.Y / 255f, value.Z / 255f, value.A / 255f);
        }
    }
}
