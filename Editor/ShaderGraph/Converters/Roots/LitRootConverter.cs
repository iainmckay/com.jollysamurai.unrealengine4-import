using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Roots
{
    public class LitRootConverter : GenericConverter<Material>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is Material rootNode && rootNode.ShadingModel == ShadingModel.DefaultLit;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, Material unrealNode)
        {
            return new PBRMasterNode();
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ExpressionReference expressionReference)
        {
            return -1;
        }

        public override void CreateConnections(Material unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.BaseColor?.NodeName, unrealNode.Name, PBRMasterNode.AlbedoSlotId, unrealNode.BaseColor);
            builder.Connect(unrealNode.Metallic?.NodeName, unrealNode.Name, PBRMasterNode.MetallicSlotId, unrealNode.Metallic);
            builder.Connect(unrealNode.Normal?.NodeName, unrealNode.Name, PBRMasterNode.NormalSlotId, unrealNode.Normal);
            builder.Connect(unrealNode.Roughness?.NodeName, unrealNode.Name, PBRMasterNode.SmoothnessSlotId, unrealNode.Roughness);

            if(unrealNode.Specular != null) {
                // FIXME: support specular or at least raise a warning?
                // builder.Connect(unrealNode.Specular?.NodeName, unrealNode.Name, PBRMasterNode.SpecularSlotId, unrealNode.Specular);
            }
        }
    }
}
