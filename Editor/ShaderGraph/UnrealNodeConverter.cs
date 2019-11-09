using UnityEditor.ShaderGraph;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph
{
    public abstract class UnrealNodeConverter
    {
        public abstract bool CanConvert(T3D.Node unrealNode);

        public abstract void Convert(T3D.Node unrealNode, ShaderGraphBuilder builder);
    }
}
