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

        public static Vector3 ConvertUnrealVector3(T3D.Vector3 v, bool applyScaling = false, bool applyAbsolute = false)
        {
            applyAbsolute = false;
            var x = applyAbsolute ? Math.Abs(v.X) : v.X;
            var y = applyAbsolute ? Math.Abs(v.Y) : v.Y;
            var z = applyAbsolute ? Math.Abs(v.Z) : v.Z;

            if(applyScaling) {
                x /= 100.0f;
                y /= 100.0f;
                z /= 100.0f;
            }

            return new Vector3(y, z, x);
        }

        public static Quaternion ConvertUnrealRotator(Rotator rotation)
        {
            var q =  Quaternion.Euler(rotation.Pitch, rotation.Yaw, rotation.Roll);

            // ue4 rotators can extend past +/- 360 so let the quaternion wrap before adjusting
            q = Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y - 180f, q.eulerAngles.z);
            // q = new Quaternion(q.x, -q.y, q.z, q.w);

            return q;
        }
    }
}
