using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionLinearInterpolateConverter : GenericConverter<MaterialExpressionLinearInterpolate>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionLinearInterpolate;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionLinearInterpolate unrealNode)
        {
            return new LerpNode {
                previewExpanded = false,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ExpressionReference expressionReference)
        {
            return 3;
        }

        public override void CreateConnections(MaterialExpressionLinearInterpolate unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.A?.NodeName, unrealNode.Name, 0, unrealNode.A);
            builder.Connect(unrealNode.B?.NodeName, unrealNode.Name, 1, unrealNode.B);
            builder.Connect(unrealNode.Alpha?.NodeName, unrealNode.Name, 2, unrealNode.Alpha);
        }
    }
}
