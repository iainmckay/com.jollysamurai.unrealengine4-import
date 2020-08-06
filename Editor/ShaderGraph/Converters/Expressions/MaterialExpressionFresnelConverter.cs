using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionFresnelConverter : GenericConverter<MaterialExpressionFresnel>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionFresnel;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionFresnel unrealNode)
        {
            var node = new FresnelNode() {
                previewExpanded = false,
            };

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 3;
        }

        public override void CreateConnections(MaterialExpressionFresnel unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            if(unrealNode.Normal != null) {
                builder.Connect(unrealNode.Normal, unrealNode.Name, 0);
            }

            if(unrealNode.BaseReflectFractionIn != null) {
                Debug.Log("FIXME: unhandled fresnel baseReflectFractionIn/baseReflectFraction");
            }

            if(unrealNode.ExponentIn != null) {
                Debug.Log("FIXME: unhandled fresnel exponentIn/exponent");
            }

            if(unrealNode.Power != null) {
                builder.Connect(unrealNode.Power, unrealNode.Name, 2);
            }
        }
    }
}
