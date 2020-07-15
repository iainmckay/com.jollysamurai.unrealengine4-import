using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionConstantConverter : GenericConverter<MaterialExpressionConstant>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionConstant;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionConstant unrealNode)
        {
            var node = new Vector1Node() {
                previewExpanded = false
            };

            node.FindSlot<Vector1MaterialSlot>(Vector1Node.InputSlotXId).value = unrealNode.R;

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return Vector1Node.OutputSlotId;
        }
    }
}
