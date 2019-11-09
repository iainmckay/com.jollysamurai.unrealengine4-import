using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionScalarParameterConverter : GenericParameterConverter<Vector1ShaderProperty, MaterialExpressionScalarParameter>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionScalarParameter;
        }

        protected override AbstractShaderProperty CreateShaderInput(MaterialExpressionScalarParameter parameterNode, ShaderGraphBuilder builder)
        {
            return builder.FindOrCreateProperty<Vector1ShaderProperty>(parameterNode.ParameterName, (p) => {
                p.value = parameterNode.DefaultValue;
            });
        }
    }
}
