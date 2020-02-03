using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;
using BlendMode = JollySamurai.UnrealEngine4.T3D.BlendMode;

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
    }
}
