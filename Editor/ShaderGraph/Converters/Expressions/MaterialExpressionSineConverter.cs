using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionSineConverter : GenericConverter<MaterialExpressionSine>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionSine;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionSine unrealNode)
        {
            return new SineNode() {
                previewExpanded = false,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 1;
        }

        public override void CreateConnections(MaterialExpressionSine unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.Input, unrealNode.Name, 0);
        }
    }
}
