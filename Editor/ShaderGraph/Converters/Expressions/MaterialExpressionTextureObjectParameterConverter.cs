using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionTextureObjectParameterConverter : GenericParameterConverter<Texture2DShaderProperty, MaterialExpressionTextureObjectParameter>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionTextureObjectParameter;
        }

        protected override AbstractShaderProperty CreateShaderInput(MaterialExpressionTextureObjectParameter parameterNode, ShaderGraphBuilder builder)
        {
            return builder.FindOrCreateProperty<Texture2DShaderProperty>(parameterNode.ParameterName, (p) => {
                // FIXME: don't use unresolved reference directly and don't guess the extension
                p.value.texture = Helper.LoadTexture(parameterNode.DefaultValue.FileName);
            });
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return PropertyNode.OutputSlotId;
        }
    }
}
