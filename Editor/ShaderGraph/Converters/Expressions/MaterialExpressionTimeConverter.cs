using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionTimeConverter : GenericConverter<MaterialExpressionTime>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionTime;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionTime unrealNode)
        {
            return new TimeNode() {
                previewExpanded = false,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return TimeNode.OutputSlotId;
        }
    }
}
