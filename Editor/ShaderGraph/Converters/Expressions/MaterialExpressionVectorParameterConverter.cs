using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionVectorParameterConverter : GenericParameterConverter<Vector4ShaderProperty, MaterialExpressionVectorParameter>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionVectorParameter;
        }

        protected override AbstractShaderProperty CreateShaderInput(MaterialExpressionVectorParameter parameterNode, ShaderGraphBuilder builder)
        {
            return builder.FindOrCreateProperty<Vector4ShaderProperty>(parameterNode.ParameterName, (p) => {
                p.value = new UnityEngine.Vector4(
                    parameterNode.DefaultValue.X,
                    parameterNode.DefaultValue.Y,
                    parameterNode.DefaultValue.Z,
                    parameterNode.DefaultValue.A
                );
            });
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            var hasR = propertyBag.HasProperty("MaskR");
            var hasG = propertyBag.HasProperty("MaskG");
            var hasB = propertyBag.HasProperty("MaskB");
            var hasA = propertyBag.HasProperty("MaskA");

            if (hasA || ! hasR || ! hasG || ! hasB) {
                // FIXME:
                throw new System.Exception("unhandled vector parameter mask");
            }

            return PropertyNode.OutputSlotId;
        }
    }
}
