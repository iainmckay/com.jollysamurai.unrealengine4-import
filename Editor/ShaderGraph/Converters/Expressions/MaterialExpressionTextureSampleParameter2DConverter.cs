using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionTextureSampleParameter2DConverter : GenericParameterConverter<Texture2DShaderProperty, MaterialExpressionTextureSampleParameter2D>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionTextureSampleParameter2D;
        }

        protected override AbstractShaderProperty CreateShaderInput(MaterialExpressionTextureSampleParameter2D parameterNode, ShaderGraphBuilder builder)
        {
            return builder.FindOrCreateProperty<Texture2DShaderProperty>(parameterNode.ParameterName, (p) => {
                // FIXME: don't use unresolved reference directly and don't guess the extension
                p.value.texture = Helper.LoadTexture(parameterNode.Texture.FileName);
            });
        }

        protected override AbstractMaterialNode CreateNodeForShaderInput(ShaderInput shaderInput, ShaderGraphBuilder builder, MaterialExpressionTextureSampleParameter2D unrealNode)
        {
            var propertyNode = base.CreateNodeForShaderInput(shaderInput, builder, unrealNode);
            var sampleNode = builder.CreateNode<SampleTexture2DNode>();
            sampleNode.textureType = TextureType.Default;

            builder.PositionNodeOnGraph(propertyNode, unrealNode);
            builder.Connect(propertyNode.GetSlotReference(PropertyNode.OutputSlotId), sampleNode.GetSlotReference(SampleTexture2DNode.TextureInputId));

            return sampleNode;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            var hasR = propertyBag.HasProperty("MaskR");
            var hasG = propertyBag.HasProperty("MaskG");
            var hasB = propertyBag.HasProperty("MaskB");
            var hasA = propertyBag.HasProperty("MaskA");

            if(hasR && hasG && hasB && hasA) {
                return SampleTexture2DNode.OutputSlotRGBAId;
            } else if(hasR && hasG && hasB) {
                // should this be only RGB instead?
                return SampleTexture2DNode.OutputSlotRGBAId;
            } else if(hasR) {
                return SampleTexture2DNode.OutputSlotRId;
            } else if(hasG) {
                return SampleTexture2DNode.OutputSlotGId;
            } else if(hasB) {
                return SampleTexture2DNode.OutputSlotBId;
            }

            // FIXME:
            throw new System.Exception("unhandled scenario");
        }

        public override void CreateConnections(MaterialExpressionTextureSampleParameter2D unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.Coordinates, unrealNode.Name, SampleTexture2DNode.UVInput);
        }
    }
}
