using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionStaticSwitchParameterConverter : GenericParameterConverter<BooleanShaderProperty, MaterialExpressionStaticSwitchParameter>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionStaticSwitchParameter;
        }

        protected override AbstractShaderProperty CreateShaderInput(MaterialExpressionStaticSwitchParameter parameterNode, ShaderGraphBuilder builder)
        {
            return builder.FindOrCreateProperty<BooleanShaderProperty>(parameterNode.ParameterName, (p) => {
                p.value = parameterNode.DefaultValue;
            });
        }

        protected override AbstractMaterialNode CreateNodeForShaderInput(ShaderInput shaderInput, ShaderGraphBuilder builder, MaterialExpressionStaticSwitchParameter unrealNode)
        {
            var propertyNode = base.CreateNodeForShaderInput(shaderInput, builder, unrealNode);
            var branchNode = builder.CreateNode<BranchNode>();

            builder.PositionNodeOnGraph(propertyNode, unrealNode);
            builder.Connect(propertyNode.GetSlotReference(0), branchNode.GetSlotReference(0));

            return branchNode;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 3;
        }

        public override void CreateConnections(MaterialExpressionStaticSwitchParameter unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.A, unrealNode.Name, 1);
            builder.Connect(unrealNode.B, unrealNode.Name, 2);
        }
    }
}
