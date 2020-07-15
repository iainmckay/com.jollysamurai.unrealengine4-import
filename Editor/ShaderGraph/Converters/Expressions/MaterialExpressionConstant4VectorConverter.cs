using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionConstant4VectorConverter : GenericConverter<MaterialExpressionConstant4Vector>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionConstant4Vector;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionConstant4Vector unrealNode)
        {
            var node = new Vector4Node() {
                previewExpanded = false,
            };

            node.FindSlot<Vector1MaterialSlot>(Vector4Node.InputSlotXId).value = unrealNode.Constant.X;
            node.FindSlot<Vector1MaterialSlot>(Vector4Node.InputSlotYId).value = unrealNode.Constant.Y;
            node.FindSlot<Vector1MaterialSlot>(Vector4Node.InputSlotZId).value = unrealNode.Constant.Z;
            node.FindSlot<Vector1MaterialSlot>(Vector4Node.InputSlotWId).value = unrealNode.Constant.A;

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return Vector4Node.OutputSlotId;
        }
    }
}
