using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters
{
    public abstract class GenericFunctionConverter : GenericConverter<MaterialExpressionMaterialFunctionCall>
    {
        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionMaterialFunctionCall unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            foreach (var functionInput in unrealNode.FunctionInputs) {
                var inputPropertyBag = ValueUtil.ParseAttributeList(functionInput.FindPropertyValue("Input"));
                var expressionValue = ValueUtil.ParseExpressionReference(inputPropertyBag.FindPropertyValue("Expression"));
                var resolvedFunctionInput = unrealMaterial.ResolveExpressionReference(expressionValue);

                if(resolvedFunctionInput != null) {
                    var slotId = GetConnectionIdSlotForFunctionInput(inputPropertyBag.FindPropertyValue("InputName"), resolvedFunctionInput);
                    var inputSlot = builder.FindSlot<MaterialSlot>(resolvedFunctionInput?.Name, unrealNode.Name, slotId, inputPropertyBag);

                    builder.Connect(resolvedFunctionInput?.Name, unrealNode.Name, slotId, inputPropertyBag);
                }
            }
        }

        protected abstract int GetConnectionIdSlotForFunctionInput(string inputName, T3D.Node functionNode);
    }
}
