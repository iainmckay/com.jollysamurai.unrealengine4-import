using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Functions
{
    public class MaterialFunctionCheapContrastConverter : GenericConverter<MaterialExpressionMaterialFunctionCall>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionMaterialFunctionCall functionNode && functionNode.MaterialFunction.NodeName == "/Engine/Functions/Engine_MaterialFunctions01/ImageAdjustment/CheapContrast_RGB.CheapContrast_RGB";
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionMaterialFunctionCall unrealNode)
        {
            return new ContrastNode();
        }
    }
}
