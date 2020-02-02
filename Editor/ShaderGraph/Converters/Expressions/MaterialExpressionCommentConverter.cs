using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionCommentConverter : UnrealNodeConverter<MaterialExpressionComment>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionComment;
        }

        public override void Convert(Node unrealNode, ShaderGraphBuilder builder)
        {
            var commentNode = unrealNode as MaterialExpressionComment;

            if(null == commentNode) {
                return;
            }

            builder.AddGroup(commentNode.Text, new Vector2(commentNode.EditorX, commentNode.EditorY), new Vector2(commentNode.SizeX, commentNode.SizeY));
        }

        public override int GetConnectionSlotId(AbstractMaterialNode @from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return -1;
        }
    }
}
