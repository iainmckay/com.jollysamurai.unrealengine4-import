using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters
{
    public abstract class GenericConverter<T> : MaterialNodeConverter<T>
        where T : T3D.Node
    {
        public override void Convert(Node unrealNode, ShaderGraphBuilder builder)
        {
            var nodeAsT = unrealNode as T;

            if(nodeAsT == null) {
                // FIXME:
                throw new System.Exception("Unexpected node type");
            }

            var graphNode = CreateNode(builder, nodeAsT);

            builder.RegisterNodeAndPositionOnGraph(graphNode, unrealNode);
        }

        protected abstract AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, T unrealNode);
    }
}
