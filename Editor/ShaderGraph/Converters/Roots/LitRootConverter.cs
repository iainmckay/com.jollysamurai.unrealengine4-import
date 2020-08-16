using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;

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
            var masterNode = new PBRMasterNode();
            masterNode.twoSided =  new ToggleData(unrealNode.IsTwoSided);
            masterNode.surfaceType = Helper.BlendModeToSurfaceType(unrealNode.BlendMode);
            masterNode.alphaMode = Helper.BlendModeToAlphaMode(unrealNode.BlendMode);
            masterNode.model = Helper.MaterialToModel(unrealNode);

            return masterNode;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return -1;
        }

        public override void CreateConnections(Material unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.BaseColor, unrealNode.Name, PBRMasterNode.AlbedoSlotId);
            builder.Connect(unrealNode.Metallic, unrealNode.Name, PBRMasterNode.MetallicSlotId);
            builder.Connect(unrealNode.Normal, unrealNode.Name, PBRMasterNode.NormalSlotId);
            builder.Connect(unrealNode.OpacityMask, unrealNode.Name, PBRMasterNode.AlphaSlotId);
            builder.Connect(unrealNode.Roughness, unrealNode.Name, PBRMasterNode.SmoothnessSlotId);
            builder.Connect(unrealNode.EmissiveColor, unrealNode.Name, PBRMasterNode.EmissionSlotId);

            if(unrealMaterial.Metallic != null && unrealMaterial.Specular != null) {
                Debug.Log("FIXME: material supports both metallic and specular but we don't");
            }

            if (unrealMaterial.Metallic == null && unrealNode.Specular != null) {
                builder.Connect(unrealNode.Specular, unrealNode.Name, PBRMasterNode.SpecularSlotId);
            }
        }
    }
}
