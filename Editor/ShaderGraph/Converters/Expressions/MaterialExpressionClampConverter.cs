using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionClampConverter : GenericConverter<MaterialExpressionClamp>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionClamp;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionClamp unrealNode)
        {
            return new ClampNode() {
                previewExpanded = false,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ExpressionReference expressionReference)
        {
            return 3;
        }

        public override void CreateConnections(MaterialExpressionClamp unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.Input?.NodeName, unrealNode.Name, 0, unrealNode.Input);
            builder.Connect(unrealNode.Min?.NodeName, unrealNode.Name, 1, unrealNode.Min);
            builder.Connect(unrealNode.Max?.NodeName, unrealNode.Name, 2, unrealNode.Max);
        }
    }
}
