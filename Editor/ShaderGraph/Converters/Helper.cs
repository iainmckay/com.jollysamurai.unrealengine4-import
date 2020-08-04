using System.IO;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;
using BlendMode = JollySamurai.UnrealEngine4.T3D.BlendMode;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters
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
            var textureFileName = "Assets" + Path.ChangeExtension(unrealFileName, "tga");

           return AssetDatabase.LoadAssetAtPath<Texture2D>(textureFileName);
        }
    }
}
