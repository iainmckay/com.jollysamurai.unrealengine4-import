using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionConstant2VectorConverter : GenericConverter<MaterialExpressionConstant2Vector>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionConstant2Vector;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionConstant2Vector unrealNode)
        {
            var node = new Vector2Node() {
                previewExpanded = false,
            };

            node.FindSlot<Vector1MaterialSlot>(Vector2Node.InputSlotXId).value = unrealNode.R;
            node.FindSlot<Vector1MaterialSlot>(Vector2Node.InputSlotYId).value = unrealNode.G;

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return Vector2Node.OutputSlotId;
        }
    }
}
