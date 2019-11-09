using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
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
                var textureFileName = parameterNode.Texture.FileName;
                textureFileName = textureFileName.Substring(0, textureFileName.LastIndexOf('.')).Substring(5);

                p.value.texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets" + textureFileName + ".tga");
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
              throw new Exception("unhandled texture type");
            }

            builder.Connect(
                propertyNode.GetSlotReference(PropertyNode.OutputSlotId),
                sampleNode.GetSlotReference(SampleTexture2DNode.TextureInputId)
            );

            return sampleNode;
        }
    }
}
