using System;
using System.IO;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;
using TextureType = UnityEditor.ShaderGraph.TextureType;

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
                var textureFileName = "Assets" + Path.ChangeExtension(parameterNode.Texture.FileName, "tga");

                p.value.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFileName);
            });
        }

        protected override AbstractMaterialNode CreateNodeForShaderInput(ShaderInput shaderInput, ShaderGraphBuilder builder, MaterialExpressionTextureSampleParameter2D unrealNode)
        {
            var propertyNode = base.CreateNodeForShaderInput(shaderInput, builder, unrealNode);
            var sampleNode = builder.CreateNode<SampleTexture2DNode>();

            builder.PositionNodeOnGraph(propertyNode, unrealNode);

            if (unrealNode.SamplerType == SamplerType.Normal) {
              sampleNode.textureType = TextureType.Normal;
            } else if (unrealNode.SamplerType == SamplerType.Default) {
              sampleNode.textureType = TextureType.Default;
            } else {
              // FIXME:
              throw new System.Exception("unhandled texture type");
            }

            builder.Connect(
                propertyNode.GetSlotReference(PropertyNode.OutputSlotId),
                sampleNode.GetSlotReference(SampleTexture2DNode.TextureInputId)
            );

            return sampleNode;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ExpressionReference expressionReference)
        {
            var propertyBag = expressionReference.Properties;

            var hasR = propertyBag.HasProperty("MaskR");
            var hasG = propertyBag.HasProperty("MaskG");
            var hasB = propertyBag.HasProperty("MaskB");
            var hasA = propertyBag.HasProperty("MaskA");

            if (hasR && hasG && hasB && hasA) {
                return SampleTexture2DNode.OutputSlotRGBAId;
            }  else if (hasR && hasG && hasB) {
                // should this be only RGB instead?
                return SampleTexture2DNode.OutputSlotRGBAId;
            } else if (hasR) {
                return SampleTexture2DNode.OutputSlotRId;
            } else if (hasG) {
                return SampleTexture2DNode.OutputSlotGId;
            } else if (hasB) {
                return SampleTexture2DNode.OutputSlotBId;
            }

            // FIXME:
            throw new System.Exception("unhandled scenario");
        }

        public override void CreateConnections(MaterialExpressionTextureSampleParameter2D unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.Coordinates?.NodeName, unrealNode.Name, SampleTexture2DNode.UVInput, unrealNode.Coordinates);
        }
    }
}
