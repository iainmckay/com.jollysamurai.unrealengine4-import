using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

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
            return new MultiplyNode();
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ExpressionReference expressionReference)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionMultiply unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.A?.NodeName, unrealNode.Name, 0, unrealNode.A);
            builder.Connect(unrealNode.B?.NodeName, unrealNode.Name, 1, unrealNode.B);
        }
    }
}
