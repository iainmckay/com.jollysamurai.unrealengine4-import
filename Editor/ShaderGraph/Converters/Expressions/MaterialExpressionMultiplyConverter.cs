using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEngine;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;
using Vector4 = UnityEngine.Vector4;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionMultiplyConverter : GenericConverter<MaterialExpressionMultiply>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionMultiply;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionMultiply unrealNode)
        {
            var node = new MultiplyNode() {
                previewExpanded =  ! unrealNode.Collapsed
            };

            if(unrealNode.A == null) {
                node.FindInputSlot<DynamicValueMaterialSlot>(0).value = new Matrix4x4(new Vector4(unrealNode.ConstA, 0), Vector4.zero, Vector4.zero, Vector4.zero);
            }

            if(unrealNode.B == null) {
                node.FindInputSlot<DynamicValueMaterialSlot>(1).value = new Matrix4x4(new Vector4(unrealNode.ConstB, 0), Vector4.zero, Vector4.zero, Vector4.zero);
            }

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionMultiply unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
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
