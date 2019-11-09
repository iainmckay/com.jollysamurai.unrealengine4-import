using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters
{
    public abstract class GenericConverter<T> : UnrealNodeConverter
        where T : T3D.Node
    {
        public sealed override void Convert(Node unrealNode, ShaderGraphBuilder builder)
        {
            var nodeAsT = unrealNode as T;

            if(nodeAsT == null) {
                // FIXME:
                throw new Exception("Unexpected node type");
            }

            var graphNode = CreateNode(builder, nodeAsT);

            builder.RegisterNodeAndPositionOnGraph(graphNode, unrealNode);
        }

        protected abstract AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, T unrealNode);
    }
}
