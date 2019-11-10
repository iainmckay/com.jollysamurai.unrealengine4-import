using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters
{
    public abstract class GenericFunctionConverter : GenericConverter<MaterialExpressionMaterialFunctionCall>
    {
        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ExpressionReference expressionReference)
        {
            return 2;
        }
        
        public override void CreateConnections(MaterialExpressionMaterialFunctionCall unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            foreach (var functionInput in unrealNode.FunctionInputs) {
                var resolvedFunctionInput = unrealMaterial.ResolveExpressionReference(functionInput);

                if (resolvedFunctionInput != null) {
                    var slotId = GetConnectionIdSlotForFunctionInput(functionInput.Properties.FindPropertyValue("InputName"), resolvedFunctionInput);
                    var inputSlot = builder.FindSlot<MaterialSlot>(resolvedFunctionInput?.Name, unrealNode.Name, slotId, functionInput);
                    
                    builder.Connect(resolvedFunctionInput?.Name, unrealNode.Name, slotId, functionInput);
                }
            }
        }

        protected abstract int GetConnectionIdSlotForFunctionInput(string inputName, T3D.Node functionNode);
    }
}
