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
    }
}
