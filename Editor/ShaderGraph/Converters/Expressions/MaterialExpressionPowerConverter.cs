using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;
using Vector4 = UnityEngine.Vector4;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionPowerConverter : GenericConverter<MaterialExpressionPower>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionPower;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionPower unrealNode)
        {
            var node = new PowerNode() {
                previewExpanded = ! unrealNode.Collapsed
            };

            if(unrealNode.Exponent == null) {
                node.FindInputSlot<DynamicVectorMaterialSlot>(1).value = new Vector4(unrealNode.ConstExponent, 0);
            }

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionPower unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.Value, unrealNode.Name, 0);

            if(unrealNode.Exponent != null) {
                builder.Connect(unrealNode.Exponent, unrealNode.Name, 1);
            }
        }
    }
}
