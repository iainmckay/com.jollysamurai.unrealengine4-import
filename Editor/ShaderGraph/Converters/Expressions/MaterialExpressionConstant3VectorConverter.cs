using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionConstant3VectorConverter : GenericConverter<MaterialExpressionConstant3Vector>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionConstant3Vector;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionConstant3Vector unrealNode)
        {
            var node = new Vector3Node() {
                previewExpanded = false,
            };

            node.FindSlot<Vector1MaterialSlot>(Vector3Node.InputSlotXId).value = unrealNode.Constant.X;
            node.FindSlot<Vector1MaterialSlot>(Vector3Node.InputSlotYId).value = unrealNode.Constant.Y;
            node.FindSlot<Vector1MaterialSlot>(Vector3Node.InputSlotZId).value = unrealNode.Constant.Z;

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return Vector3Node.OutputSlotId;
        }
    }
}
