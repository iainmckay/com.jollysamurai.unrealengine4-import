using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using Vector4 = UnityEngine.Vector4;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionDivideConverter : GenericConverter<MaterialExpressionDivide>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionDivide;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionDivide unrealNode)
        {
            var node = new DivideNode() {
                previewExpanded = false,
            };

            if(unrealNode.A == null) {
                node.FindInputSlot<DynamicVectorMaterialSlot>(0).value = new Vector4(unrealNode.ConstA, 0);
            }

            if(unrealNode.B == null) {
                node.FindInputSlot<DynamicVectorMaterialSlot>(1).value = new Vector4(unrealNode.ConstB, 0);
            }

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionDivide unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            if(unrealNode.A != null) {
                builder.Connect(unrealNode.A, unrealNode.Name, 0);
            }

            if(unrealNode.B != null) {
                builder.Connect(unrealNode.B, unrealNode.Name, 1);
            }
        }
    }
}
