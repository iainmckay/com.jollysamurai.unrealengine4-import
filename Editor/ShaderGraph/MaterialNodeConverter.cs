using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph
{
    public abstract class MaterialNodeConverter<T> : MaterialNodeConverter where T : T3D.Node
    {
        public virtual void CreateConnections(T unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
        }

        public override void CreateConnections(Node unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            var unrealNodeAsT = (T) unrealNode;

            if(unrealNodeAsT != null) {
                CreateConnections(unrealNodeAsT, unrealMaterial, builder);
            }
        }
    }

    public abstract class MaterialNodeConverter
    {
        public abstract bool CanConvert(T3D.Node unrealNode);
        public abstract void Convert(T3D.Node unrealNode, ShaderGraphBuilder builder);

        public abstract int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag);

        public abstract void CreateConnections(T3D.Node unrealNode, Material unrealMaterial, ShaderGraphBuilder builder);
    }
}
