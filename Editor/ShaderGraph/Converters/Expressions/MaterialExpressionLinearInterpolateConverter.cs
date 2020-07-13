using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
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
                previewExpanded = ! unrealNode.Collapsed,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 3;
        }

        public override void CreateConnections(MaterialExpressionLinearInterpolate unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.A, unrealNode.Name, 0);
            builder.Connect(unrealNode.B, unrealNode.Name, 1);
            builder.Connect(unrealNode.Alpha, unrealNode.Name, 2);
        }
    }
}
