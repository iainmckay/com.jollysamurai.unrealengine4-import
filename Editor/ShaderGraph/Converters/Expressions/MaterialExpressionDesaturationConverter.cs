using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
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
                previewExpanded = ! unrealNode.Collapsed,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionDesaturation unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.Input, unrealNode.Name, 0);
            builder.Connect(unrealNode.Fraction, unrealNode.Name, 1);
        }
    }
}
