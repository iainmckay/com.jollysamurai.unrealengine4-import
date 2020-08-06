using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionTextureObjectConverter : GenericConverter<MaterialExpressionTextureObject>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionTextureObject;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionTextureObject unrealNode)
        {
            var node = new SampleTexture2DNode() {
                previewExpanded = false,
            };

            if(unrealNode.Texture != null) {
                // FIXME: don't use unresolved reference directly and don't guess the extension
                node.FindInputSlot<Texture2DInputMaterialSlot>(SampleTexture2DNode.TextureInputId).texture = Helper.LoadTexture(unrealNode.Texture.FileName);
            }

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return SampleTexture2DNode.OutputSlotRGBAId;
        }
    }
}
