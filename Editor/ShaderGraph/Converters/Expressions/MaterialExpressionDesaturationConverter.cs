using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionDesaturationConverter : GenericConverter<MaterialExpressionDesaturation>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionDesaturation;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionDesaturation unrealNode)
        {
            return new SaturationNode() {
                previewExpanded = false,
            };
        }
    }
}
