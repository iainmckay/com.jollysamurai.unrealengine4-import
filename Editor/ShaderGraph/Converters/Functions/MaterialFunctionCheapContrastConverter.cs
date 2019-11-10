using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Functions
{
    public class MaterialFunctionCheapContrastConverter : GenericFunctionConverter
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionMaterialFunctionCall functionNode && functionNode.MaterialFunction.NodeName == "/Engine/Functions/Engine_MaterialFunctions01/ImageAdjustment/CheapContrast_RGB.CheapContrast_RGB";
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionMaterialFunctionCall unrealNode)
        {
            return new ContrastNode();
        }

        protected override int GetConnectionIdSlotForFunctionInput(string inputName, Node functionNode)
        {
            switch (inputName) {
                case "In":
                    return 0;

                case "Contrast":
                    return 1;
            }

            // FIXME:
            throw new Exception("unhandled function call input");
        }
    }
}
