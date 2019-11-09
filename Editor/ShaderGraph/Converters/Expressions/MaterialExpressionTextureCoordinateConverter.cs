using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionTextureCoordinateConverter : GenericConverter<MaterialExpressionTextureCoordinate>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionTextureCoordinate;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionTextureCoordinate unrealNode)
        {
            return new UVNode {
                uvChannel = (UVChannel) unrealNode.CoordinateIndex,
                previewExpanded = ! unrealNode.Collapsed,
            };
        }
    }
}
