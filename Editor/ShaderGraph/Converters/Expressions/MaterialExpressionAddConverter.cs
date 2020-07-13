using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

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
            return new AddNode() {
                previewExpanded = ! unrealNode.Collapsed
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 2;
        }

        public override void CreateConnections(MaterialExpressionAdd unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.A, unrealNode.Name, 0);
            builder.Connect(unrealNode.B, unrealNode.Name, 1);
        }
    }
}
