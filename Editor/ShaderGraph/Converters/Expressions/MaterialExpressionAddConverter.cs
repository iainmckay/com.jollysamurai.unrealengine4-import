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
    public class MaterialExpressionAddConverter : GenericConverter<MaterialExpressionAdd>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionAdd;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionAdd unrealNode)
        {
            var node = new AddNode() {
                previewExpanded = ! unrealNode.Collapsed
            };

            if(unrealNode.A == null) {
                node.FindInputSlot<DynamicVectorMaterialSlot>(0).value = new Vector4(unrealNode.ConstA, 0);
            }

            if(unrealNode.B == null) {
                node.FindInputSlot<DynamicVectorMaterialSlot>(1).value = new Vector4(unrealNode.ConstB, 0);
            }

            return node;
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionAdd unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            if(unrealNode.A != null) {
                builder.Connect(unrealNode.A, unrealNode.Name, 0);
            }

            if(unrealNode.B != null) {
                builder.Connect(unrealNode.B, unrealNode.Name, 1);
            }
        }
    }
}
