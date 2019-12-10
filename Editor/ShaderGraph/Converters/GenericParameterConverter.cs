using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters
{
    public abstract class GenericParameterConverter<T, U> : UnrealNodeConverter<U>
        where T : AbstractShaderProperty
        where U : ParameterNode
    {
        public sealed override void Convert(Node unrealNode, ShaderGraphBuilder builder)
        {
            var parameterNode = unrealNode as U;

            if(parameterNode == null) {
                // FIXME:
                throw new System.Exception("Unexpected node type");
            }

            var shaderInput = CreateShaderInput(parameterNode, builder);
            var graphNode = CreateNodeForShaderInput(shaderInput, builder, (U) unrealNode);

            builder.RegisterNodeAndPositionOnGraph(graphNode, unrealNode);
        }

        protected virtual AbstractMaterialNode CreateNodeForShaderInput(ShaderInput shaderInput, ShaderGraphBuilder builder, U unrealNode)
        {
            var graphNode = builder.CreateNode<PropertyNode>();
            graphNode.propertyGuid = shaderInput.guid;

            return graphNode;
        }

        protected abstract AbstractShaderProperty CreateShaderInput(U parameterNode, ShaderGraphBuilder builder);
    }
}
